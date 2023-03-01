using Cysharp.Threading.Tasks;

namespace ET
{
    public class CodeLoaderComponent : Singleton<CodeLoaderComponent>
    {
        public struct CodeStartAsync
        {
        }

        public struct CodeLoadHotfixAsync
        {
        }
        
        public async UniTask StartAsync()
        {
            await EventSystem.Instance.Invoke<CodeStartAsync, UniTask>(new CodeStartAsync());
        }
        
        // 热重载调用该方法
        public async UniTask LoadHotfixAsync()
        {
            await EventSystem.Instance.Invoke<CodeLoadHotfixAsync, UniTask>(new CodeLoadHotfixAsync());
        }
    }
}
