using Cysharp.Threading.Tasks;

namespace ET.Client
{
    [Event(SceneType.Client)]
    public class AppStartInitFinish_CreateLoginUI : AEvent<EventType.AppStartInitFinish>
    {
        protected override async UniTask Run(Scene scene, EventType.AppStartInitFinish args)
        {
            await UIComponent.Instance.OpenUIFormAsync(UGFUIFormId.UILogin);
        }
    }
}