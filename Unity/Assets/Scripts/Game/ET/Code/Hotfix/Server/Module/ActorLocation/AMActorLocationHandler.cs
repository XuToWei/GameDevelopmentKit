using System;
using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [EnableClass]
    public abstract class AMActorLocationHandler<E, Message> : IMActorHandler
        where E : Entity where Message : class, IActorLocationMessage
    {
        protected abstract UniTask Run(E entity, Message message);

        public async UniTask Handle(Entity entity, int fromProcess, object actorMessage)
        {
            if (actorMessage is not Message message)
            {
                Log.Error($"消息类型转换错误: {actorMessage.GetType().FullName} to {typeof(Message).Name}");
                return;
            }

            if (entity is not E e)
            {
                Log.Error($"Actor类型转换错误: {entity.GetType().Name} to {typeof(E).Name} --{typeof(Message).Name}");
                return;
            }

            ActorResponse response = new() { RpcId = message.RpcId };
            ActorHandleHelper.Reply(fromProcess, response);

            await this.Run(e, message);
        }

        public Type GetRequestType()
        {
            return typeof(Message);
        }

        public Type GetResponseType()
        {
            return typeof(ActorResponse);
        }
    }
}