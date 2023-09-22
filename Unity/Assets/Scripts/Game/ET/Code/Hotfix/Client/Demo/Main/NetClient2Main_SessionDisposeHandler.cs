using Cysharp.Threading.Tasks;

namespace ET.Client
{
    [MessageHandler(SceneType.All)]
    public class NetClient2Main_SessionDisposeHandler: MessageHandler<Scene, NetClient2Main_SessionDispose>
    {
        protected override async UniTask Run(Scene entity, NetClient2Main_SessionDispose message)
        {
<<<<<<< HEAD
            entity.Fiber.Error($"session dispose, error: {message.Error}");
            await UniTask.CompletedTask;
=======
            Log.Error($"session dispose, error: {message.Error}");
            await ETTask.CompletedTask;
>>>>>>> 7d37d33dfbf69d664e224d4387156fcf2fda4f70
        }
    }
}