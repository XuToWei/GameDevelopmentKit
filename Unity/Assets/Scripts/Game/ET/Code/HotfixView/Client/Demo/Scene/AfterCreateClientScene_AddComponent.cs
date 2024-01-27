using Cysharp.Threading.Tasks;

namespace ET.Client
{
    [Event(SceneType.Demo)]
    public class AfterCreateClientScene_AddComponent: AEvent<Scene, AfterCreateClientScene>
    {
        protected override async UniTask Run(Scene scene, AfterCreateClientScene args)
        {
            scene.AddComponent<UGFUIComponent>();
            scene.AddComponent<UGFEntityComponent>();
            await UniTask.CompletedTask;
        }
    }
}