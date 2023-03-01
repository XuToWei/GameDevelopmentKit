using Cysharp.Threading.Tasks;

namespace ET
{
    [ConsoleHandler(ConsoleMode.ReloadDll)]
    public class ReloadDllConsoleHandler : IConsoleHandler
    {
        public async UniTask Run(ModeContex contex, string content)
        {
            switch (content)
            {
                case ConsoleMode.ReloadDll:
                    contex.Parent.RemoveComponent<ModeContex>();

                    await CodeLoaderComponent.Instance.LoadHotfixAsync();

                    EventSystem.Instance.Load();
                    break;
            }

            await UniTask.CompletedTask;
        }
    }
}