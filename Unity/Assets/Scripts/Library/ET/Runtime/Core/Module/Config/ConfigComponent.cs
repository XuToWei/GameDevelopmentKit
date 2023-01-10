namespace ET
{
    public class ConfigComponent : Singleton<ConfigComponent>
    {
        public struct LoadAll
        {
        }
        
        public struct LoadOne
        {
            public string ConfigName
            {
                private set;
                get;
            }

            public LoadOne(string configName)
            {
                this.ConfigName = configName;
            }
        }
        
        public async ETTask LoadAllAsync()
        {
            await EventSystem.Instance.Invoke<LoadAll, ETTask>(0, new LoadAll());
        }

        public async ETTask LoadOneAsync(string configName)
        {
            await EventSystem.Instance.Invoke<LoadOne, ETTask>(0, new LoadOne(configName));
        }
    }
}