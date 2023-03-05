using Cysharp.Threading.Tasks;

namespace ET.Client
{
    [Event(SceneType.Client)]
    public class LoginFinish_RemoveLoginUI : AEvent<EventType.LoginFinish>
    {
        protected override async UniTask Run(Scene scene, EventType.LoginFinish args)
        {
            await UniTask.CompletedTask;
            UIComponent.Instance.CloseUIForm(UGFUIFormId.UILogin);
        }
    }
}