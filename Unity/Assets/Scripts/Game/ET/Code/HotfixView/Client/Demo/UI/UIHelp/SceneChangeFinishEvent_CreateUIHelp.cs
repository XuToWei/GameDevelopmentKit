using Cysharp.Threading.Tasks;

namespace ET.Client
{
    [Event(SceneType.Current)]
    public class SceneChangeFinishEvent_CreateUIHelp : AEvent<EventType.SceneChangeFinish>
    {
        protected override async UniTask Run(Scene scene, EventType.SceneChangeFinish args)
        {
            await UIComponent.Instance.OpenUIFormAsync(UGFUIFormId.UIHelp);
        }
    }
}
