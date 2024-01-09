using Cysharp.Threading.Tasks;

namespace ET
{
    public class ConfigComponent : Singleton<ConfigComponent>, ISingletonAwake<IConfigReader>
    {
        private IConfigReader iConfigReader;

        public void Awake(IConfigReader iConfigReader)
        {
            this.iConfigReader = iConfigReader;
        }

        public struct LoadAll
        {
        }
        
        public struct ReloadOne
        {
            public string ConfigName
            {
                private set;
                get;
            }

            public ReloadOne(string configName)
            {
                this.ConfigName = configName;
            }
        }
        
        public struct ReloadAll
        {
        }

        public async UniTask LoadAllAsync()
        {
            await EventSystem.Instance.Invoke<LoadAll, UniTask>(new LoadAll());
        }

        public async UniTask ReloadOneAsync(string configName)
        {
            await EventSystem.Instance.Invoke<ReloadOne, UniTask>(new ReloadOne(configName));
        }
        
        public async UniTask ReloadAllAsync()
        {
            await EventSystem.Instance.Invoke<ReloadAll, UniTask>(new ReloadAll());
        }

        public UniTask<byte[]> ReadBytesAsync(string file)
        {
            return iConfigReader.ReadBytesAsync(file);
        }

        public UniTask<string> ReadTextAsync(string file)
        {
            return iConfigReader.ReadTextAsync(file);
        }
    }
}