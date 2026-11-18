#if UNITY_EDITOR || !UNITY_WEBGL || !UNITY_ET
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using Cysharp.Threading.Tasks;

namespace ET
{
    public class WService: AService
    {
        private long idGenerater = 200000000;
        
        private HttpListener httpListener;
        
        private readonly Dictionary<long, WChannel> channels = new Dictionary<long, WChannel>();

        public ThreadSynchronizationContext ThreadSynchronizationContext;

        public WService(IEnumerable<string> prefixes, NetworkProtocol protocol)
        {
            if (protocol != NetworkProtocol.Websocket)
            {
                throw new NotSupportedException($"Unsupported WebSocket protocol: {protocol}.");
            }
            this.ThreadSynchronizationContext = new ThreadSynchronizationContext();
            
            this.httpListener = new HttpListener();

            StartAccept(prefixes).Forget();
        }
        
        public WService(NetworkProtocol protocol)
        {
            if (protocol != NetworkProtocol.Websocket)
            {
                throw new NotSupportedException($"Unsupported WebSocket protocol: {protocol}.");
            }
            this.ServiceType = ServiceType.Outer;
            this.ThreadSynchronizationContext = new ThreadSynchronizationContext();
        }
        
        private long GetId
        {
            get
            {
                return ++this.idGenerater;
            }
        }
        
        public override void Create(long id, IPEndPoint ipEndpoint)
        {
            throw new NotSupportedException("IP endpoints are not supported by the WebSocket transport.");
        }

        public override void Create(long id, string address)
        {
            WChannel channel = new(id, address, this);
            this.channels[channel.Id] = channel;
        }

        public override void Update()
        {
            this.ThreadSynchronizationContext.Update();
        }

        public override void Remove(long id, int error = 0)
        {
            WChannel channel;
            if (!this.channels.TryGetValue(id, out channel))
            {
                return;
            }

            channel.Error = error;

            this.channels.Remove(id);
            channel.Dispose();
        }

        public override bool IsDisposed()
        {
            return this.ThreadSynchronizationContext == null;
        }

        protected void Get(long id, IPEndPoint ipEndPoint)
        {
            if (!this.channels.TryGetValue(id, out _))
            {
                this.Create(id, ipEndPoint);
            }
        }
        
        public WChannel Get(long id)
        {
            WChannel channel = null;
            this.channels.TryGetValue(id, out channel);
            return channel;
        }

        public override void Dispose()
        {
            base.Dispose();
            
            this.ThreadSynchronizationContext = null;
            this.httpListener?.Close();
            this.httpListener = null;
        }

        private async UniTaskVoid StartAccept(IEnumerable<string> prefixs)
        {
            try
            {
                foreach (string prefix in prefixs)
                {
                    this.httpListener.Prefixes.Add(prefix);
                }
                
                httpListener.Start();

                while (true)
                {
                    try
                    {
                        HttpListenerContext httpListenerContext = await this.httpListener.GetContextAsync();

                        HttpListenerWebSocketContext webSocketContext = await httpListenerContext.AcceptWebSocketAsync(null);

                        WChannel channel = new WChannel(this.GetId, webSocketContext, this);
                        channel.RemoteAddress = httpListenerContext.Request.RemoteEndPoint;
                        this.channels[channel.Id] = channel;

                        this.AcceptCallback(channel.Id, channel.RemoteAddress);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
            }
            catch (HttpListenerException e)
            {
                if (e.ErrorCode == 5)
                {
                    throw new Exception($"CMD管理员中输入: netsh http add urlacl url=http://*:8080/ user=Everyone   {prefixs.ToList().ListToString()}", e);
                }

                Log.Error(e);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public override void Send(long channelId, MemoryBuffer memoryBuffer)
        {
            this.channels.TryGetValue(channelId, out WChannel channel);
            if (channel == null)
            {
                return;
            }
            channel.Send(memoryBuffer);
        }
    }
}
#endif