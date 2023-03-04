using Cysharp.Threading.Tasks;

namespace ET
{
    public class CodeLoaderComponent : Singleton<CodeLoaderComponent>, ICodeLoader
    {
        private ICodeLoader codeLoader;

        public void SetCodeLoader(ICodeLoader loader)
        {
            this.codeLoader = loader;
        }
        
        public async UniTask StartAsync()
        {
            await this.codeLoader.StartAsync();
        }
        
        // 热重载调用该方法
        public async UniTask LoadHotfixAsync()
        {
            await this.codeLoader.LoadHotfixAsync();
        }
    }
}
