namespace ET
{
    [EntitySystemOf(typeof(GlobalComponent))]
    [FriendOf(typeof(GlobalComponent))]
    public static partial class GlobalComponentSystem
    {
        [EntitySystem]
        private static void Awake(this GlobalComponent self)
        {
            self.OnAwake();
        }
    }
    
    [ComponentOf(typeof(Scene))]
    public class GlobalComponent : Entity, IAwake
    {
        /// <summary>
        /// 更改设置启动的AppType
        /// </summary>
        public AppType AppType { get; private set; }

        public void OnAwake()
        {
            AppType = AppType.Demo;
        }
    }
}
