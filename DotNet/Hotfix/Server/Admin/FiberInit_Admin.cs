using System;
using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [Invoke((long)SceneType.Admin)]
    public class FiberInit_Admin : AInvokeHandler<FiberInit, UniTask>
    {
        public override async UniTask Handle(FiberInit fiberInit)
        {
            Scene root = fiberInit.Fiber.Root;
            root.AddComponent<MailBoxComponent, MailBoxType>(MailBoxType.UnOrderedMessage);
            root.AddComponent<TimerComponent>();
            root.AddComponent<ProcessInnerSender>();
            root.AddComponent<MessageSender>();

            // 初始化 ActorRpcBridge，通过 MessageSender 透明处理跨进程通信
            var fiber = fiberInit.Fiber;
            Func<int, int, IRequest, UniTask<IResponse>> callFunc = (processId, fiberId, request) =>
            {
                var tcs = AutoResetUniTaskCompletionSource<IResponse>.Create();
                fiber.ThreadSynchronizationContext.Post(async () =>
                {
                    try
                    {
                        var messageSender = root.GetComponent<MessageSender>();
                        var actorId = new ActorId(processId, fiberId);
                        var resp = await messageSender.Call(actorId, request);
                        tcs.TrySetResult(resp);
                    }
                    catch (Exception e)
                    {
                        tcs.TrySetException(e);
                    }
                });
                return tcs.Task;
            };
            World.Instance.AddSingleton<ActorRpcBridge, Func<int, int, IRequest, UniTask<IResponse>>>(callFunc);

            // 启动 Admin Web 服务器
            World.Instance.AddSingleton<AdminComponent>();
            await AdminComponent.Instance.StartAsync();
        }
    }
}
