using Cysharp.Threading.Tasks;

namespace ET.Client
{
    [Event(SceneType.LockStep)]
    public class AfterCreateClientScene_LSAddComponent: AEvent<Scene, AfterCreateClientScene>
    {
        protected override async UniTask Run(Scene scene, AfterCreateClientScene args)
        {
            scene.AddComponent<UIComponent>();
            scene.AddComponent<UGFEntityComponent>();
            await UniTask.CompletedTask;
        }
    }
}