using System;
using Cysharp.Threading.Tasks;

namespace ET.Server
{
    public interface IMActorHandler
    {
        UniTask Handle(Entity entity, int fromProcess, object actorMessage);
        Type GetRequestType();
        Type GetResponseType();
    }
}