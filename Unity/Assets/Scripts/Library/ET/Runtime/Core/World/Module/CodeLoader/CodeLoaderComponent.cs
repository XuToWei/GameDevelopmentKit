using Cysharp.Threading.Tasks;

namespace ET
{
    public class CodeLoaderComponent : Singleton<CodeLoaderComponent>, ISingletonAwake<ICodeLoader>
    {
        private ICodeLoader iCodeLoader;

        public void Awake(ICodeLoader iCodeLoader)
        {
            this.iCodeLoader = iCodeLoader;
        }

        public async UniTask StartAsync()
        {
            await this.iCodeLoader.StartAsync();
        }
        
        // 热重载调用该方法
        public async UniTask ReloadAsync()
        {
            await this.iCodeLoader.ReloadAsync();
        }
    }
}
