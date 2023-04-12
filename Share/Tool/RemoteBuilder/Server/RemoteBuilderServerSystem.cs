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
                self.AddComponent<RemoteBuilder>();
                self.StartAsync().Forget();
            }
        }

        public static async UniTaskVoid StartAsync(this RemoteBuilderServer self)
        {
            await self.GetComponent<RemoteBuilder>().StartAsync();
            Root.Instance.Scene.AddComponent<NetInnerComponent, IPEndPoint>(NetworkHelper.ToIPEndPoint(
                $"{Tables.Instance.RemoteBuilderConfig.ServerInnerIP}:{Tables.Instance.RemoteBuilderConfig.ServerPort}"));
        }
    }
}