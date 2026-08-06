using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [Invoke((long)SceneType.SurvivorRoomManager)]
    public sealed class FiberInit_SurvivorRoomManager: AInvokeHandler<FiberInit, UniTask>
    {
        public override async UniTask Handle(FiberInit fiberInit)
        {
            Scene root = fiberInit.Fiber.Root;
            root.AddComponent<MailBoxComponent, MailBoxType>(MailBoxType.UnOrderedMessage);
            root.AddComponent<TimerComponent>();
            root.AddComponent<CoroutineLockComponent>();
            root.AddComponent<ProcessInnerSender>();
            root.AddComponent<MessageSender>();
            root.AddComponent<SurvivorRoomManagerComponent>();
            root.AddComponent<LocationProxyComponent>();
            root.AddComponent<MessageLocationSenderComponent>();
            await UniTask.CompletedTask;
        }
    }
}
