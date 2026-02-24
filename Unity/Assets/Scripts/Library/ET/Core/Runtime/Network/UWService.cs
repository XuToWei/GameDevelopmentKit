#if UNITY_ET
using System.Collections.Generic;
using System.Net;

namespace ET
{
    public class WService: AService
    {
        private long idGenerater = 200000000;

        private readonly Dictionary<long, WChannel> channels = new Dictionary<long, WChannel>();

        public ThreadSynchronizationContext ThreadSynchronizationContext;

        public WService()
        {
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
            WChannel channel = new(id, ipEndpoint, this);
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