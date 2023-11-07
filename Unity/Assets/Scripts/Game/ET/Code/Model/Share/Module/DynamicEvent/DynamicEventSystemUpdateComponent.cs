using Cysharp.Threading.Tasks;

namespace ET
{
    [ComponentOf(typeof(Scene))]
    public sealed class DynamicEventSystemUpdateComponent : Entity, IAwake, IUpdate
    {
        
    }
    
    [EntitySystemOf(typeof(DynamicEventSystemUpdateComponent))]
    [FriendOf(typeof(DynamicEventSystemUpdateComponent))]
    static partial class DynamicEventSystemUpdateComponentSystem
    {
        [EntitySystem]
        private static void Awake(this DynamicEventSystemUpdateComponent self)
        {
            
        }
        
        [EntitySystem]
        private static void Update(this DynamicEventSystemUpdateComponent self)
        {
            DynamicEventSystem.Instance.Update();
        }
    }
    
    [Event(SceneType.Main)]
    public class DynamicEventSystemUpdate_InitShare : AEvent<Scene, EntryEvent1>
    {
        protected override async UniTask Run(Scene scene, EntryEvent1 a)
        {
            scene.AddComponent<DynamicEventSystemUpdateComponent>();
            await UniTask.CompletedTask;
        }
    }
}
