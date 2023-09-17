using Cysharp.Threading.Tasks;

namespace ET
{
    [ConsoleHandler(ConsoleMode.ReloadDll)]
    public class ReloadDllConsoleHandler: IConsoleHandler
    {
        public async UniTask Run(Fiber fiber, ModeContex contex, string content)
        {
            await CodeLoaderComponent.Instance.ReloadAsync();
        }
    }
}