using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [Invoke((long)SceneType.SurvivorRoom)]
    public sealed class FiberInit_SurvivorRoom: AInvokeHandler<FiberInit, UniTask>
    {
        public override async UniTask Handle(FiberInit fiberInit)
        {
            fiberInit.Fiber.Root.AddComponent<MailBoxComponent, MailBoxType>(MailBoxType.UnOrderedMessage);
            fiberInit.Fiber.Root.AddComponent<TimerComponent>();
            fiberInit.Fiber.Root.AddComponent<CoroutineLockComponent>();
            fiberInit.Fiber.Root.AddComponent<ProcessInnerSender>();
            fiberInit.Fiber.Root.AddComponent<MessageSender>();
            fiberInit.Fiber.Root.AddComponent<LocationProxyComponent>();
            fiberInit.Fiber.Root.AddComponent<MessageLocationSenderComponent>();
            await UniTask.CompletedTask;
        }
    }
}
