namespace ET.Client
{
    [EntitySystemOf(typeof(TestComponent))]
    [FriendOf(typeof(TestComponent))]
    public static partial class TestComponentSystem
    {
        [EntitySystem]
        private static void Awake(this TestComponent self)
        {
            
        }
        
        [EntitySystem]
        private static void Update(this TestComponent self)
        {
            //Log.Debug("Test:TestReload");
        }
    }
}
