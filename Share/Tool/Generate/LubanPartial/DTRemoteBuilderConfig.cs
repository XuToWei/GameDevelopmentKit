using System.Net;

namespace ET
{
    public partial class DTRemoteBuilderConfig
    {
        public IPEndPoint ServerInnerIPOutPort
        {
            get;
            private set;
        }
        
        partial void PostInit()
        {
            ServerInnerIPOutPort = NetworkHelper.ToIPEndPoint($"{this.ServerInnerIP}:{this.ServerOuterPort}");
        }
    }
}