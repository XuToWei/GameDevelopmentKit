using System;
using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [Event(SceneType.Process)]
    public class NetInnerComponentOnReadEvent: AEvent<Scene, NetInnerComponentOnRead>
    {
        protected override async UniTask Run(Scene scene, NetInnerComponentOnRead args)
        {
            try
            {
                long actorId = args.ActorId;
                object message = args.Message;
                
                if (message is IActorResponse iActorResponse)
                {
                    ActorMessageSenderComponent.Instance.HandleIActorResponse(iActorResponse);
                    return;
                }
                
                InstanceIdStruct instanceIdStruct = new(actorId);
                int fromProcess = instanceIdStruct.Process;
                instanceIdStruct.Process = Options.Instance.Process;
                long realActorId = instanceIdStruct.ToLong();
                
                // 收到actor消息,放入actor队列
                switch (message)
                {
                    case IActorRequest iActorRequest:
                    {
                        await ActorMessageDispatcherComponent.Instance.HandleIActorRequest(fromProcess, realActorId, iActorRequest);
                        break;
                    }
                    case IActorMessage iActorMessage:
                    {
                        await ActorMessageDispatcherComponent.Instance.HandleIActorMessage(fromProcess, realActorId, iActorMessage);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"InnerMessageDispatcher error: {args.Message.GetType().FullName}\n{e}");
            }
        }
    }
}