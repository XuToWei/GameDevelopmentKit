using System;
using Cysharp.Threading.Tasks;

namespace ET
{
    /// <summary>
    /// Actor RPC 桥接器，用于 Admin/Agent 进程通过 ET Actor 系统发送跨进程 RPC 请求
    /// </summary>
    public class ActorRpcBridge : Singleton<ActorRpcBridge>, ISingletonAwake<Func<int, int, IRequest, UniTask<IResponse>>>
    {
        private Func<int, int, IRequest, UniTask<IResponse>> callFunc;

        public void Awake(Func<int, int, IRequest, UniTask<IResponse>> callFunc)
        {
            this.callFunc = callFunc;
        }

        public UniTask<IResponse> CallAsync(int processId, int fiberId, IRequest request)
        {
            return this.callFunc(processId, fiberId, request);
        }
    }
}
