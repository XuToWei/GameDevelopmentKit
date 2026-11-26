using System.Net.Sockets;

namespace ET.Client
{
    [ComponentOf(typeof(Scene))]
    public class RouterAddressComponent: Entity, IAwake<string>
    {
        public AddressFamily AddressFamily;
        public string Address;
        public HttpGetRouterResponse Info;
        public int RouterIndex;
    }
}
