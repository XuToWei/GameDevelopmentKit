using System.Net;
using System.Net.Sockets;
using Cysharp.Threading.Tasks;

namespace ET.Client
{
    public static class RemoteBuilderClientSystem
    {
        public class AwakeSystem : AwakeSystem<RemoteBuilderServer>
        {
            protected override void Awake(RemoteBuilderServer self)
            {
    
            }
        }
    }
}