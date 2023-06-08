namespace ET
{
    public class GlobalComponent : Singleton<GlobalComponent>, ISingletonAwake
    {
        /// <summary>
        /// 更改设置启动的AppType
        /// </summary>
        public AppType AppType { get; private set; }

        public void Awake()
        {
            AppType = AppType.LockStep;
        }
    }
}
