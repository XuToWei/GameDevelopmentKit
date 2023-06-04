using Cysharp.Threading.Tasks;

namespace ET.Client
{
    [Event(SceneType.LockStep)]
    public class AfterCreateClientScene_LSAddComponent: AEvent<Scene, EventType.AfterCreateClientScene>
    {
        protected override async UniTask Run(Scene scene, EventType.AfterCreateClientScene args)
        {
            scene.AddComponent<EntityComponent>();
            scene.AddComponent<UIComponent>();
            await UniTask.CompletedTask;
        }
    }
}