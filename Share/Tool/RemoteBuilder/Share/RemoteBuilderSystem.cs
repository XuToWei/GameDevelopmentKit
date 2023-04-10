using Cysharp.Threading.Tasks;

namespace ET
{
    public static class RemoteBuilderSystem
    {
        public class AwakeSystem : AwakeSystem<RemoteBuilder>
        {
            protected override void Awake(RemoteBuilder self)
            {
                
            }
        }
        
        public static async UniTask StartAsync(this RemoteBuilder self)
        {
            await ConfigComponent.Instance.LoadAllAsync();
        }
    }
}