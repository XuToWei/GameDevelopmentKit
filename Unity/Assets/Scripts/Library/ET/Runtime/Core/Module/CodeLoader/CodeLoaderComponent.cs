using Cysharp.Threading.Tasks;

namespace ET
{
    public class CodeLoaderComponent : Singleton<CodeLoaderComponent>, ICodeLoader
    {
        private ICodeLoader iCodeLoader;

        public ICodeLoader ICodeLoader
        {
            set
            {
                this.iCodeLoader = value;
            }
        }

        public async UniTask StartAsync()
        {
            await this.iCodeLoader.StartAsync();
        }
        
        // 热重载调用该方法
        public async UniTask LoadHotfixAsync()
        {
            await this.iCodeLoader.LoadHotfixAsync();
        }
    }
}
