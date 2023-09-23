using Cysharp.Threading.Tasks;

namespace ET.Client
{
    [MessageHandler(SceneType.All)]
    public class NetClient2Main_SessionDisposeHandler: MessageHandler<Scene, NetClient2Main_SessionDispose>
    {
        protected override async UniTask Run(Scene entity, NetClient2Main_SessionDispose message)
        {
            Log.Error($"session dispose, error: {message.Error}");
            await UniTask.CompletedTask;
        }
    }
}