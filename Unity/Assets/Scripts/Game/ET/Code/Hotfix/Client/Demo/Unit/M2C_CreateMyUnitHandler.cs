using Cysharp.Threading.Tasks;

namespace ET.Client
{
    [MessageHandler(SceneType.Client)]
    public class M2C_CreateMyUnitHandler : AMHandler<M2C_CreateMyUnit>
    {
        protected override async UniTask Run(Session session, M2C_CreateMyUnit message)
        {
            // 通知场景切换协程继续往下走
            session.DomainScene().GetComponent<ObjectWait>().Notify(new Wait_CreateMyUnit() { Message = message });
            await UniTask.CompletedTask;
        }
    }
}