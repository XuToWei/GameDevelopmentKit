using Cysharp.Threading.Tasks;

namespace ET.Client
{
    [Event(SceneType.Current)]
    public class AfterCreateCurrentScene_AddComponent: AEvent<Scene, AfterCreateCurrentScene>
    {
        protected override async UniTask Run(Scene scene, AfterCreateCurrentScene args)
        {
            scene.AddComponent<UGFUIComponent>();
            scene.AddComponent<UGFEntityComponent>();
            await UniTask.CompletedTask;
        }
    }
}