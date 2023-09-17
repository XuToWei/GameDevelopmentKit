using System;
using Cysharp.Threading.Tasks;

namespace ET.Server
{
    public static partial class LocationProxyComponentSystem
    {
        private static ActorId GetLocationSceneId(long key)
        {
            return Tables.Instance.DTStartSceneConfig.LocationConfig.ActorId;
        }

        public static async UniTask Add(this LocationProxyComponent self, int type, long key, ActorId actorId)
        {
            Fiber fiber = self.Fiber();
            fiber.Info($"location proxy add {key}, {actorId} {TimeInfo.Instance.ServerNow()}");
            await fiber.Root.GetComponent<MessageSender>().Call(GetLocationSceneId(key),
                new ObjectAddRequest() { Type = type, Key = key, ActorId = actorId });
        }

        public static async UniTask Lock(this LocationProxyComponent self, int type, long key, ActorId actorId, int time = 60000)
        {
            Fiber fiber = self.Fiber();
            fiber.Info($"location proxy lock {key}, {actorId} {TimeInfo.Instance.ServerNow()}");
            await fiber.Root.GetComponent<MessageSender>().Call(GetLocationSceneId(key),
                new ObjectLockRequest() { Type = type, Key = key, ActorId = actorId, Time = time });
        }

        public static async UniTask UnLock(this LocationProxyComponent self, int type, long key, ActorId oldActorId, ActorId newActorId)
        {
            Fiber fiber = self.Fiber();
            fiber.Info($"location proxy unlock {key}, {newActorId} {TimeInfo.Instance.ServerNow()}");
            await fiber.Root.GetComponent<MessageSender>().Call(GetLocationSceneId(key),
                new ObjectUnLockRequest() { Type = type, Key = key, OldActorId = oldActorId, NewActorId = newActorId });
        }

        public static async UniTask Remove(this LocationProxyComponent self, int type, long key)
        {
            Fiber fiber = self.Fiber();
            fiber.Info($"location proxy add {key}, {TimeInfo.Instance.ServerNow()}");
            await fiber.Root.GetComponent<MessageSender>().Call(GetLocationSceneId(key),
                new ObjectRemoveRequest() { Type = type, Key = key });
        }

        public static async UniTask<ActorId> Get(this LocationProxyComponent self, int type, long key)
        {
            if (key == 0)
            {
                throw new Exception($"get location key 0");
            }

            // location server配置到共享区，一个大战区可以配置N多个location server,这里暂时为1
            ObjectGetResponse response =
                    (ObjectGetResponse) await self.Root().GetComponent<MessageSender>().Call(GetLocationSceneId(key),
                        new ObjectGetRequest() { Type = type, Key = key });
            return response.ActorId;
        }

        public static async UniTask AddLocation(this Entity self, int type)
        {
            await self.Root().GetComponent<LocationProxyComponent>().Add(type, self.Id, self.GetActorId());
        }

        public static async UniTask RemoveLocation(this Entity self, int type)
        {
            await self.Root().GetComponent<LocationProxyComponent>().Remove(type, self.Id);
        }
    }
}