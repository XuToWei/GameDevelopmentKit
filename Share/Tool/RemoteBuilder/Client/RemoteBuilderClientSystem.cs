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
                self.AddComponent<RemoteBuilder>();
                self.StartAsync().Forget();
            }
        }

        public static async UniTaskVoid StartAsync(this RemoteBuilderServer self)
        {
            await self.GetComponent<RemoteBuilder>().StartAsync();
            IPAddress ipAddress = NetworkHelper.GetHostAddress(ToolConfig.Instance.RemoteBuilderConfig.ServerInnerIP);
            Root.Instance.Scene.AddComponent<NetClientComponent, AddressFamily>(ipAddress.AddressFamily);
        }
    }
}