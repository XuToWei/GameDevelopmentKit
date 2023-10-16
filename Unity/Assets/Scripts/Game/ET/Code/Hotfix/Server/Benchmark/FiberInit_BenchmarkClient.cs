using System.Net.Sockets;
using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [Invoke((long)SceneType.BenchmarkClient)]
    public class FiberInit_BenchmarkClient: AInvokeHandler<FiberInit, UniTask>
    {
        public override async UniTask Handle(FiberInit fiberInit)
        {
            Scene root = fiberInit.Fiber.Root;
            //root.AddComponent<MailBoxComponent, MailBoxType>(MailBoxType.UnOrderedMessage);
            //root.AddComponent<TimerComponent>();
            //root.AddComponent<CoroutineLockComponent>();
            //root.AddComponent<ActorInnerComponent>();
            //root.AddComponent<PlayerComponent>();
            //root.AddComponent<GateSessionKeyComponent>();
            //root.AddComponent<LocationProxyComponent>();
            //root.AddComponent<ActorLocationSenderComponent>();
            root.AddComponent<NetComponent, AddressFamily, NetworkProtocol>(AddressFamily.InterNetwork, NetworkProtocol.UDP);
            root.AddComponent<BenchmarkClientComponent>();
            await UniTask.CompletedTask;
        }
    }
}