using System.Net;
using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [Invoke((long)SceneType.Router)]
    public class FiberInit_Router: AInvokeHandler<FiberInit, UniTask>
    {
        public override async UniTask Handle(FiberInit fiberInit)
        {
            Scene root = fiberInit.Fiber.Root;
            var startSceneConfig = Tables.Instance.DTStartSceneConfig.Get(Options.Instance.StartConfig, (int)root.Id);
            
            // 开发期间使用OuterIPPort，云服务器因为本机没有OuterIP，所以要改成InnerIPPort，然后在云防火墙中端口映射到InnerIPPort
            root.AddComponent<RouterComponent, IPEndPoint, string>(startSceneConfig.OuterIPPort, startSceneConfig.StartProcessConfig.InnerIP);
            Log.Console($"Router create: {root.Fiber.Id}");
            await UniTask.CompletedTask;
        }
    }
}