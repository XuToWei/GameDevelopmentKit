using Cysharp.Threading.Tasks;

namespace ET.Client
{
    [Event(SceneType.Current)]
    public class SceneChangeFinishEvent_CreateUIHelp : AEvent<Scene, SceneChangeFinish>
    {
        protected override async UniTask Run(Scene scene, SceneChangeFinish args)
        {
            await scene.GetComponent<UGFUIComponent>().OpenUIFormAsync(UGFUIFormId.UIHelp);
        }
    }
}
