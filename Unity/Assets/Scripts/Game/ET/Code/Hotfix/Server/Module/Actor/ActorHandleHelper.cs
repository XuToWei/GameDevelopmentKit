using System;
using Cysharp.Threading.Tasks;

namespace ET.Server
{
    public static partial class ActorHandleHelper
    {
        public static void Reply(int fromProcess, IActorResponse response)
        {
            if (fromProcess == Options.Instance.Process) // 返回消息是同一个进程
            {
                async UniTask HandleMessageInNextFrame()
                {
                    await TimerComponent.Instance.WaitFrameAsync();
                    NetInnerComponent.Instance.HandleMessage(0, response);
                }
                HandleMessageInNextFrame().Forget();
                return;
            }

            Session replySession = NetInnerComponent.Instance.Get(fromProcess);
            replySession.Send(response);
        }
    }
}