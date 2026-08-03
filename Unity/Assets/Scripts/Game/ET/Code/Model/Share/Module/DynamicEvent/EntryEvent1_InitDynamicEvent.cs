using Cysharp.Threading.Tasks;

namespace ET
{
    [EnableClass]
    [Event(SceneType.Main)]
    public class EntryEvent1_InitDynamicEvent : AEvent<Scene, EntryEvent1>
    {
        protected override async UniTask Run(Scene scene, EntryEvent1 a)
        {
            scene.AddComponent<DynamicEventComponent>();
            await UniTask.CompletedTask;
        }
    }
}
