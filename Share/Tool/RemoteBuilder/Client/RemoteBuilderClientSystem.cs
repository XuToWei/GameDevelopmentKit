using System.Net;
using System.Net.Sockets;
using Cysharp.Threading.Tasks;

namespace ET.Client
{
    public static class RemoteBuilderClientSystem
    {
        public class AwakeSystem : AwakeSystem<RemoteBuilderClient>
        {
            protected override void Awake(RemoteBuilderClient self)
            {
            }
        }
        
        public class DestroySystem : DestroySystem<RemoteBuilderClient>
        {
            protected override void Destroy(RemoteBuilderClient self)
            {
            }
        }

        public static async UniTask InitAsync(this RemoteBuilderClient self)
        {
           
            Scene scene = self.DomainScene();
            scene.AddComponent<NetClientComponent, AddressFamily>(NetworkHelper
                    .GetHostAddress(Tables.Instance.DTRemoteBuilderConfig.ServerInnerIP).AddressFamily);
            
            self.Session = scene.GetComponent<NetClientComponent>().Create(Tables.Instance.DTRemoteBuilderConfig.ServerInnerIPOutPort);

            scene.AddComponent<ConsoleComponent>();
            
            EventSystem.Instance.Publish(scene, new EventType.OnClientInitFinish());
            
            await UniTask.CompletedTask;
        }
    }
}