namespace ET
{
    [EntitySystemOf(typeof(DynamicEventComponent))]
    [FriendOf(typeof(DynamicEventComponent))]
    public static partial class DynamicEventComponentSystem
    {
        [EntitySystem]
        private static void Awake(this DynamicEventComponent self)
        {
            DynamicEventSystem.Instance.RegisterEntity(self.Parent);
        }

        [EntitySystem]
        private static void Destroy(this DynamicEventComponent self)
        {
            DynamicEventSystem.Instance.UnRegisterEntity(self.Parent);
        }
    }
    
    [ComponentOf]
    public sealed class DynamicEventComponent : Entity, IAwake, IDestroy
    {
        
    }
}