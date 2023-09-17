using System;
using Cysharp.Threading.Tasks;

namespace ET
{
    public interface IMHandler
    {
        UniTask Handle(Entity entity, Address fromAddress, MessageObject actorMessage);
        Type GetRequestType();
        Type GetResponseType();
    }
}