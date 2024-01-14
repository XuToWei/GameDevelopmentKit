using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace ET
{
    public readonly struct RpcInfo
    {
        public Type RequestType { get; }
        
        private readonly AutoResetUniTaskCompletionSourcePlus<IResponse> tcs;

        public RpcInfo(Type requestType)
        {
            this.RequestType = requestType;
            
            this.tcs = AutoResetUniTaskCompletionSourcePlus<IResponse>.Create();
        }

        public void SetResult(IResponse response)
        {
            this.tcs.TrySetResult(response);
        }

        public void SetException(Exception exception)
        {
            this.tcs.TrySetException(exception);
        }

        public UniTask<IResponse> Wait()
        {
            return this.tcs.Task;
        }

        public void AddOnCancelAction(Action cancelAction)
        {
            this.tcs.AddOnCancelAction(cancelAction);
        }

        public void AttachCancellation(CancellationToken token)
        {
            this.tcs.AttachCancellation(token);
        }
    }
    
    [EntitySystemOf(typeof(Session))]
    [FriendOf(typeof(Session))]
    public static partial class SessionSystem
    {
        [EntitySystem]
        private static void Awake(this Session self, AService aService)
        {
            self.AService = aService;
            long timeNow = TimeInfo.Instance.ClientNow();
            self.LastRecvTime = timeNow;
            self.LastSendTime = timeNow;

            self.requestCallbacks.Clear();
            
            Log.Info($"session create: zone: {self.Zone()} id: {self.Id} {timeNow} ");
        }
        
        [EntitySystem]
        private static void Destroy(this Session self)
        {
            self.AService.Remove(self.Id, self.Error);
            
            foreach (RpcInfo responseCallback in self.requestCallbacks.Values.ToArray())
            {
                responseCallback.SetException(new RpcException(self.Error, $"session dispose: {self.Id} {self.RemoteAddress}"));
            }

            Log.Info($"session dispose: {self.RemoteAddress} id: {self.Id} ErrorCode: {self.Error}, please see ErrorCode.cs! {TimeInfo.Instance.ClientNow()}");
            
            self.requestCallbacks.Clear();
        }
        
        public static void OnResponse(this Session self, IResponse response)
        {
            if (!self.requestCallbacks.Remove(response.RpcId, out RpcInfo action))
            {
                return;
            }
            action.SetResult(response);
        }
        
        public static UniTask<IResponse> Call(this Session self, IRequest request, CancellationToken token)
        {
            int rpcId = ++self.RpcId;
            RpcInfo rpcInfo = new(request.GetType());
            self.requestCallbacks[rpcId] = rpcInfo;
            request.RpcId = rpcId;

            self.Send(request);
            
            void CancelAction()
            {
                if (!self.requestCallbacks.Remove(rpcId, out RpcInfo action))
                {
                    return;
                }

                Type responseType = OpcodeType.Instance.GetResponseType(action.RequestType);
                IResponse response = (IResponse) Activator.CreateInstance(responseType);
                response.Error = ErrorCore.ERR_Cancel;
                action.SetResult(response);
            }

            rpcInfo.AddOnCancelAction(CancelAction);
            rpcInfo.AttachCancellation(token);
            return rpcInfo.Wait();
        }

        public static async UniTask<IResponse> Call(this Session self, IRequest request, int time = 0)
        {
            int rpcId = ++self.RpcId;
            RpcInfo rpcInfo = new(request.GetType());
            self.requestCallbacks[rpcId] = rpcInfo;
            request.RpcId = rpcId;
            self.Send(request);

            if (time > 0)
            {
                async UniTaskVoid Timeout()
                {
                    await self.Root().GetComponent<TimerComponent>().WaitAsync(time);
                    if (!self.requestCallbacks.TryGetValue(rpcId, out RpcInfo action))
                    {
                        return;
                    }

                    if (!self.requestCallbacks.Remove(rpcId))
                    {
                        return;
                    }
                    
                    action.SetException(new Exception($"session call timeout: {action.RequestType.FullName} {time}"));
                }
                
                Timeout().Forget();
            }

            return await rpcInfo.Wait();
        }

        public static void Send(this Session self, IMessage message)
        {
            self.Send(default, message);
        }
        
        public static void Send(this Session self, ActorId actorId, IMessage message)
        {
            self.LastSendTime = TimeInfo.Instance.ClientNow();
            LogMsg.Instance.Debug(self.Fiber(), message);

            (ushort opcode, MemoryBuffer memoryBuffer) = MessageSerializeHelper.ToMemoryBuffer(self.AService, actorId, message);
            self.AService.Send(self.Id, memoryBuffer);
        }
    }

    [ChildOf]
    public sealed class Session: Entity, IAwake<AService>, IDestroy
    {
        public AService AService { get; set; }
        
        public int RpcId
        {
            get;
            set;
        }

        public readonly Dictionary<int, RpcInfo> requestCallbacks = new();
        
        public long LastRecvTime
        {
            get;
            set;
        }

        public long LastSendTime
        {
            get;
            set;
        }

        public int Error
        {
            get;
            set;
        }

        public IPEndPoint RemoteAddress
        {
            get;
            set;
        }
    }
}