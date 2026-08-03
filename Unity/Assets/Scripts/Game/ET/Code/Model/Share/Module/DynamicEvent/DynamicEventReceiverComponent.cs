namespace ET
{
    [EntitySystemOf(typeof(DynamicEventReceiverComponent))]
    [FriendOf(typeof(DynamicEventReceiverComponent))]
    public static partial class DynamicEventReceiverComponentSystem
    {
        [EntitySystem]
        private static void Awake(this DynamicEventReceiverComponent self)
        {
            self.Root().GetComponent<DynamicEventComponent>().RegisterEntity(self.Parent);
        }

        [EntitySystem]
        private static void Destroy(this DynamicEventReceiverComponent self)
        {
            self.Root().GetComponent<DynamicEventComponent>()?.UnRegisterEntity(self.Parent);
        }
    }
    
    [ComponentOf]
    public sealed class DynamicEventReceiverComponent : Entity, IAwake, IDestroy
    {
        
    }
}
