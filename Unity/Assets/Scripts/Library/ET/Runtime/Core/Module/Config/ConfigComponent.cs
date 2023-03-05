using Cysharp.Threading.Tasks;

namespace ET
{
    public class ConfigComponent : Singleton<ConfigComponent>, ISingletonAwake
    {
        public struct Init
        {
        }
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
        
        public struct ReloadAll
        {
        }
        
        public void Awake()
        {
            EventSystem.Instance.Invoke<Init>(new Init());
        }
        
        public async UniTask LoadAllAsync()
        {
            await EventSystem.Instance.Invoke<LoadAll, UniTask>(new LoadAll());
        }

        public async UniTask LoadOneAsync(string configName)
        {
            await EventSystem.Instance.Invoke<LoadOne, UniTask>(new LoadOne(configName));
        }
        
        public async UniTask ReloadAllAsync()
        {
            await EventSystem.Instance.Invoke<ReloadAll, UniTask>(new ReloadAll());
        }
    }
}