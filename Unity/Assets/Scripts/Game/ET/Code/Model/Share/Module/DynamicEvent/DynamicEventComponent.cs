namespace ET
{
    public static class DynamicEventComponentSystem
    {
        [ObjectSystem]
        public class DynamicEventAwakeSystem: AwakeSystem<DynamicEventComponent>
        {
            protected override void Awake(DynamicEventComponent self)
            {
                DynamicEventWatcherComponent.Instance.Register(self.Parent);
            }
        }

        [ObjectSystem]
        public class DynamicEventDestroySystem : DestroySystem<DynamicEventComponent>
        {
            protected override void Destroy(DynamicEventComponent self)
            {
                DynamicEventWatcherComponent.Instance.UnRegister(self.Parent);
            }
        }
    }
    
    [ComponentOf]
    public sealed class DynamicEventComponent: Entity, IAwake, IDestroy
    {
        
    }
}