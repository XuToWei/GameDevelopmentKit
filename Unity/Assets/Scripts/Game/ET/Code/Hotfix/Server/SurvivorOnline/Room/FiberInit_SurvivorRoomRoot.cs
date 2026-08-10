using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [Invoke((long)SceneType.SurvivorRoomRoot)]
    public sealed class FiberInit_SurvivorRoomRoot: AInvokeHandler<FiberInit, UniTask>
    {
        public override UniTask Handle(FiberInit fiberInit)
        {
            fiberInit.Fiber.Root.AddComponent<MailBoxComponent, MailBoxType>(MailBoxType.UnOrderedMessage);
            fiberInit.Fiber.Root.AddComponent<TimerComponent>();
            fiberInit.Fiber.Root.AddComponent<CoroutineLockComponent>();
            fiberInit.Fiber.Root.AddComponent<ProcessInnerSender>();
            fiberInit.Fiber.Root.AddComponent<MessageSender>();
            fiberInit.Fiber.Root.AddComponent<LocationProxyComponent>();
            fiberInit.Fiber.Root.AddComponent<MessageLocationSenderComponent>();
            return UniTask.CompletedTask;
        }
    }
}
