using System;
using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [MessageHandler(SceneType.Admin)]
    public class S2Admin_ProcessStatusReportHandler : HandlerObject, IMHandler
    {
        public async UniTask Handle(Entity entity, Address fromAddress, MessageObject actorMessage)
        {
            if (actorMessage is not S2Admin_ProcessStatusReport message)
            {
                return;
            }

            AdminComponent.Instance?.AdminActorService?.HandleStatusReport(
                message.ProcessId,
                message.Status,
                message.MemoryUsage,
                message.FiberCount);

            await UniTask.CompletedTask;
        }

        public Type GetRequestType()
        {
            return typeof(S2Admin_ProcessStatusReport);
        }

        public Type GetResponseType()
        {
            return null;
        }
    }
}
