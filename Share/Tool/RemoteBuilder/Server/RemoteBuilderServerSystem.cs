using Cysharp.Threading.Tasks;

namespace ET
{
    public static class RemoteBuilderServerSystem
    {
        public class AwakeSystem : AwakeSystem<RemoteBuilderServer>
        {
            protected override void Awake(RemoteBuilderServer self)
            {
                self.AddComponent<RemoteBuilder>();
                self.StartAsync().Forget();
            }
        }

        public static async UniTaskVoid StartAsync(this RemoteBuilderServer self)
        {
            await self.GetComponent<RemoteBuilder>().StartAsync();
            self.AddComponent<MailBoxComponent, MailboxType>(MailboxType.UnOrderMessageDispatcher);
        }
    }
}