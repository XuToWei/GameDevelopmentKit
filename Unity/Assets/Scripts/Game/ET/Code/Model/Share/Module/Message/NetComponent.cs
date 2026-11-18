using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace ET
{
    public struct NetComponentOnRead
    {
        public Session Session;
        public object Message;
    }
    
    [ComponentOf(typeof(Scene))]
    public class NetComponent: Entity, IAwake<IPEndPoint, NetworkProtocol>, IAwake<AddressFamily, NetworkProtocol>, IAwake<NetworkProtocol>, IAwake<IEnumerable<string>, NetworkProtocol>, IDestroy, IUpdate
    {
        public AService AService { get; set; }
    }
}