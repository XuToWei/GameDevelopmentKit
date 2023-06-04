using Cysharp.Threading.Tasks;

namespace ET.Client
{
    [MessageHandler(SceneType.LockStep)]
    public class G2C_ReconnectHandler: MessageHandler<G2C_Reconnect>
    {
        protected override async UniTask Run(Session session, G2C_Reconnect message)
        {
            await LSSceneChangeHelper.SceneChangeToReconnect(session.ClientScene(), message);
            await UniTask.CompletedTask;
        }
    }
}