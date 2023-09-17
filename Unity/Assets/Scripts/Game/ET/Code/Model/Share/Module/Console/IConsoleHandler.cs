using Cysharp.Threading.Tasks;

namespace ET
{
    public interface IConsoleHandler
    {
        UniTask Run(Fiber fiber, ModeContex contex, string content);
    }
}