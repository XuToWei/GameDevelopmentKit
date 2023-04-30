using System.Net;
using Cysharp.Threading.Tasks;

namespace ET.Server
{
    public static class RemoteBuilderServerSystem
    {
        public class AwakeSystem : AwakeSystem<RemoteBuilderServer>
        {
            protected override void Awake(RemoteBuilderServer self)
            {
                self.IsBuilding = false;
            }
        }
        
        public class DestroySystem : DestroySystem<RemoteBuilderServer>
        {
            protected override void Destroy(RemoteBuilderServer self)
            {
                self.IsBuilding = false;
            }
        }

        public static async UniTask InitAsync(this RemoteBuilderServer self)
        {
            Scene scene = self.DomainScene();
            await UniTask.CompletedTask;
            scene.AddComponent<NetServerComponent, IPEndPoint>(Tables.Instance.DTRemoteBuilderConfig.ServerInnerIPOutPort);
        }
    }
}