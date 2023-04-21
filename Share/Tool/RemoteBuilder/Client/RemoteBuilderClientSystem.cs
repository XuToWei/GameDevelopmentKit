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
            ClientSceneManagerComponent clientSceneManagerComponent = Root.Instance.Scene.AddComponent<ClientSceneManagerComponent>();
            IPAddress ipAddress = NetworkHelper.GetHostAddress(ToolConfig.Instance.RemoteBuilderConfig.ServerInnerIP);
            NetClientComponent netClientComponent = clientSceneManagerComponent.AddComponent<NetClientComponent, AddressFamily>(ipAddress.AddressFamily);
            Session session = netClientComponent.Create(NetworkHelper.ToIPEndPoint(ToolConfig.Instance.RemoteBuilderConfig.ServerInnerIP));
            session.AddComponent<PingComponent>();
            session.AddComponent<RouterCheckComponent>();

            

            await TimerComponent.Instance.WaitAsync(10000);
            
            
        }
    }
}