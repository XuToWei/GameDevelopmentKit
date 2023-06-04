using Cysharp.Threading.Tasks;

namespace ET.Client
{
    [Event(SceneType.Current)]
    public class AfterCreateCurrentScene_AddComponent: AEvent<Scene, EventType.AfterCreateCurrentScene>
    {
        protected override async UniTask Run(Scene scene, EventType.AfterCreateCurrentScene args)
        {
            await UniTask.CompletedTask;
        }
    }
}