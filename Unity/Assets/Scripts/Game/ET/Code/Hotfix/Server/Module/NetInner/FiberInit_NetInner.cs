using System.Net;
using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [Invoke((long)SceneType.NetInner)]
    public class FiberInit_NetInner: AInvokeHandler<FiberInit, UniTask>
    {
        public override async UniTask Handle(FiberInit fiberInit)
        {
            Scene root = fiberInit.Fiber.Root;
            root.AddComponent<MailBoxComponent, MailBoxType>(MailBoxType.UnOrderedMessage);
            root.AddComponent<TimerComponent>();
            root.AddComponent<CoroutineLockComponent>();
            var startProcessConfig = Tables.Instance.DTStartProcessConfig.Get(Options.Instance.StartConfig, fiberInit.Fiber.Process);
            root.AddComponent<ProcessOuterSender, IPEndPoint>(startProcessConfig.IPEndPoint);
            root.AddComponent<ProcessInnerSender>();

            await UniTask.CompletedTask;
        }
    }
}