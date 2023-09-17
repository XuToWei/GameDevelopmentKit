using Cysharp.Threading.Tasks;

namespace ET
{
    public interface ICodeLoader
    { 
        UniTask StartAsync();

        // 热重载调用该方法
        UniTask ReloadAsync();
    }
}
