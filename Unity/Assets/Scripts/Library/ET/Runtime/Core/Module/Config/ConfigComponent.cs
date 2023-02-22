using Cysharp.Threading.Tasks;

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
        
        public async UniTask LoadAllAsync()
        {
            await EventSystem.Instance.Invoke<LoadAll, UniTask>(0, new LoadAll());
        }

        public async UniTask LoadOneAsync(string configName)
        {
            await EventSystem.Instance.Invoke<LoadOne, UniTask>(0, new LoadOne(configName));
        }
    }
}