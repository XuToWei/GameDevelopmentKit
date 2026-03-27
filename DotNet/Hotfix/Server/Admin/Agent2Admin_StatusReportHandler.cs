using System;
using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [MessageHandler(SceneType.Admin)]
    public class Agent2Admin_StatusReportHandler : HandlerObject, IMHandler
    {
        public async UniTask Handle(Entity entity, Address fromAddress, MessageObject actorMessage)
        {
            if (actorMessage is not Agent2Admin_StatusReport message)
            {
                return;
            }

            AdminComponent.Instance?.AgentActorService?.MarkAgentAlive(message.AgentProcessId);

            await UniTask.CompletedTask;
        }

        public Type GetRequestType()
        {
            return typeof(Agent2Admin_StatusReport);
        }

        public Type GetResponseType()
        {
            return null;
        }
    }
}
