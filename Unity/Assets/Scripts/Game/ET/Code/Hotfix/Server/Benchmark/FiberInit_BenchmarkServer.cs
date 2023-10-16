using System.Net;
using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [Invoke((long)SceneType.BenchmarkServer)]
    public class FiberInit_BenchmarkServer: AInvokeHandler<FiberInit, UniTask>
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
            root.AddComponent<NetComponent, IPEndPoint, NetworkProtocol>(Tables.Instance.DTStartSceneConfig.Benchmark.OuterIPPort, NetworkProtocol.UDP);
            root.AddComponent<BenchmarkServerComponent>();
            await UniTask.CompletedTask;
        }
    }
}