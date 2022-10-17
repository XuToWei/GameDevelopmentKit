namespace ET
{
    /// <summary>
    /// Config组件会扫描所有的有ConfigAttribute标签的配置,加载进来
    /// </summary>
    public class ConfigComponent: Singleton<ConfigComponent>
    {
        public struct LoadLuban
        {
        }
        
        public struct LoadLubanAsync
        {
            public int Id { get; set; }
        }

        public void Load()
        {
            ISingleton singleton = EventSystem.Instance.Invoke<LoadLuban, ISingleton>(0, new LoadLuban());
            singleton.Register();
        }

        public async ETTask LoadAsync()
        {
            ISingleton singleton = await EventSystem.Instance.Invoke<LoadLubanAsync, ETTask<ISingleton>>(0, new LoadLubanAsync());
            singleton.Register();
        }
    }
}