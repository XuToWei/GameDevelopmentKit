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
        public readonly IRequest Request;
        public readonly AutoResetUniTaskCompletionSource<IResponse> Tcs;

        public RpcInfo(IRequest request)
        {
            this.Request = request;
            this.Tcs = AutoResetUniTaskCompletionSource<IResponse>.Create();
        }
    }
    
    [FriendOf(typeof(Session))]
    public static partial class SessionSystem
    {
        [EntitySystem]
        private class SessionAwakeSystem : AwakeSystem<Session, int>
        {
            protected override void Awake(Session self, int serviceId)
            {
                self.ServiceId = serviceId;
                long timeNow = TimeHelper.ClientNow();
                self.LastRecvTime = timeNow;
                self.LastSendTime = timeNow;

                self.requestCallbacks.Clear();
            
                Log.Info($"session create: zone: {self.DomainZone()} id: {self.Id} {timeNow} ");
            }
        }

        [EntitySystem]
        private class SessionDestroySystem : DestroySystem<Session>
        {
            protected override void Destroy(Session self)
            {
                NetServices.Instance.RemoveChannel(self.ServiceId, self.Id, self.Error);
            
                foreach (RpcInfo responseCallback in self.requestCallbacks.Values.ToArray())
                {
                    responseCallback.Tcs.TrySetException(new RpcException(self.Error, $"session dispose: {self.Id} {self.RemoteAddress}"));
                }

                Log.Info($"session dispose: {self.RemoteAddress} id: {self.Id} ErrorCode: {self.Error}, please see ErrorCode.cs! {TimeHelper.ClientNow()}");
            
                self.requestCallbacks.Clear();
            }
        }

        public static void OnResponse(this Session self, IResponse response)
        {
            if (!self.requestCallbacks.TryGetValue(response.RpcId, out var action))
            {
                return;
            }

            self.requestCallbacks.Remove(response.RpcId);
            if (ErrorCore.IsRpcNeedThrowException(response.Error))
            {
                action.Tcs.TrySetException(new Exception($"Rpc error, request: {action.Request} response: {response}"));
                return;
            }
            action.Tcs.TrySetResult(response);
        }
        
        public static async UniTask<IResponse> Call(this Session self, IRequest request, CancellationTokenSource cts)
        {
            int rpcId = ++Session.RpcId;
            RpcInfo rpcInfo = new RpcInfo(request);
            self.requestCallbacks[rpcId] = rpcInfo;
            request.RpcId = rpcId;

            self.Send(request);
            
            void CancelAction()
            {
                if (!self.requestCallbacks.TryGetValue(rpcId, out RpcInfo action))
                {
                    return;
                }

                self.requestCallbacks.Remove(rpcId);
                Type responseType = OpcodeTypeComponent.Instance.GetResponseType(action.Request.GetType());
                IResponse response = (IResponse) Activator.CreateInstance(responseType);
                response.Error = ErrorCore.ERR_Cancel;
                action.Tcs.TrySetResult(response);
            }

            IResponse ret;
            CancellationTokenRegistration? ctr = null;
            try
            {
                ctr = cts?.Token.Register(CancelAction);
                ret = await rpcInfo.Tcs.Task;
            }
            finally
            {
                ctr?.Dispose();
            }
            return ret;
        }

        public static async UniTask<IResponse> Call(this Session self, IRequest request)
        {
            int rpcId = ++Session.RpcId;
            RpcInfo rpcInfo = new RpcInfo(request);
            self.requestCallbacks[rpcId] = rpcInfo;
            request.RpcId = rpcId;
            self.Send(request);
            return await rpcInfo.Tcs.Task;
        }

        public static void Send(this Session self, IMessage message)
        {
            self.Send(0, message);
        }
        
        public static void Send(this Session self, long actorId, IMessage message)
        {
            self.LastSendTime = TimeHelper.ClientNow();
            self.LogMsg(message);
            NetServices.Instance.SendMessage(self.ServiceId, self.Id, actorId, message as MessageObject);
        }
    }

    [ChildOf]
    public sealed class Session: Entity, IAwake<int>, IDestroy
    {
        public int ServiceId { get; set; }
        
        public static int RpcId
        {
            get;
            set;
        }

        public readonly Dictionary<int, RpcInfo> requestCallbacks = new Dictionary<int, RpcInfo>();
        
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