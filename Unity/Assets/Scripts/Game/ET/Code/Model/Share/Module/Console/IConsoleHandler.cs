using Cysharp.Threading.Tasks;

namespace ET
{
    public interface IConsoleHandler
    {
        UniTask Run(ModeContex contex, string content);
    }
}