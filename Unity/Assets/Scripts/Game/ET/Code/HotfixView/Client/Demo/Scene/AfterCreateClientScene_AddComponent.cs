using Cysharp.Threading.Tasks;

namespace ET.Client
{
    [Event(SceneType.Demo)]
    public class AfterCreateClientScene_AddComponent: AEvent<Scene, EventType.AfterCreateClientScene>
    {
        protected override async UniTask Run(Scene scene, EventType.AfterCreateClientScene args)
        {
            scene.AddComponent<EntityComponent>();
            scene.AddComponent<UIComponent>();
            await UniTask.CompletedTask;
        }
    }
}