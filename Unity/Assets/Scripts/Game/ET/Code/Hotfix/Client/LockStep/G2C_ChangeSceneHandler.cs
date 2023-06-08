using Cysharp.Threading.Tasks;

namespace ET.Client
{
    [MessageHandler(SceneType.LockStep)]
    public class Match2G_NotifyMatchSuccessHandler: MessageHandler<Match2G_NotifyMatchSuccess>
    {
        protected override async UniTask Run(Session session, Match2G_NotifyMatchSuccess message)
        {
            await LSSceneChangeHelper.SceneChangeTo(session.DomainScene(), "Map1", message.InstanceId);
        }
    }
}