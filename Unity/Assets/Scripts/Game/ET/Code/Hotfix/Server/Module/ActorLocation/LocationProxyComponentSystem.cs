using System;
using Cysharp.Threading.Tasks;

namespace ET.Server
{
    public static partial class LocationProxyComponentSystem
    {
        [EntitySystem]
        private class LocationProxyComponentAwakeSystem : AwakeSystem<LocationProxyComponent>
        {
            protected override void Awake(LocationProxyComponent self)
            {
                LocationProxyComponent.Instance = self;
            }
        }

        [EntitySystem]
        private class LocationProxyComponentDestroySystem : DestroySystem<LocationProxyComponent>
        {
            protected override void Destroy(LocationProxyComponent self)
            {
                LocationProxyComponent.Instance = null;
            }
        }

        private static long GetLocationSceneId(long key)
        {
            return Tables.Instance.DTStartSceneConfig.LocationConfig.InstanceId;
        }

        public static async UniTask Add(this LocationProxyComponent self, int type, long key, long instanceId)
        {
            Log.Info($"location proxy add {key}, {instanceId} {TimeHelper.ServerNow()}");
            await ActorMessageSenderComponent.Instance.Call(GetLocationSceneId(key),
                new ObjectAddRequest() { Type = type, Key = key, InstanceId = instanceId });
        }

        public static async UniTask Lock(this LocationProxyComponent self, int type, long key, long instanceId, int time = 60000)
        {
            Log.Info($"location proxy lock {key}, {instanceId} {TimeHelper.ServerNow()}");
            await ActorMessageSenderComponent.Instance.Call(GetLocationSceneId(key),
                new ObjectLockRequest() { Type = type, Key = key, InstanceId = instanceId, Time = time });
        }

        public static async UniTask UnLock(this LocationProxyComponent self, int type, long key, long oldInstanceId, long instanceId)
        {
            Log.Info($"location proxy unlock {key}, {instanceId} {TimeHelper.ServerNow()}");
            await ActorMessageSenderComponent.Instance.Call(GetLocationSceneId(key),
                new ObjectUnLockRequest() { Type = type, Key = key, OldInstanceId = oldInstanceId, InstanceId = instanceId });
        }

        public static async UniTask Remove(this LocationProxyComponent self, int type, long key)
        {
            Log.Info($"location proxy add {key}, {TimeHelper.ServerNow()}");
            await ActorMessageSenderComponent.Instance.Call(GetLocationSceneId(key),
                new ObjectRemoveRequest() { Type = type, Key = key });
        }

        public static async UniTask<long> Get(this LocationProxyComponent self, int type, long key)
        {
            if (key == 0)
            {
                throw new Exception($"get location key 0");
            }

            // location server配置到共享区，一个大战区可以配置N多个location server,这里暂时为1
            ObjectGetResponse response =
                    (ObjectGetResponse) await ActorMessageSenderComponent.Instance.Call(GetLocationSceneId(key),
                        new ObjectGetRequest() { Type = type, Key = key });
            return response.InstanceId;
        }

        public static async UniTask AddLocation(this Entity self, int type)
        {
            await LocationProxyComponent.Instance.Add(type, self.Id, self.InstanceId);
        }

        public static async UniTask RemoveLocation(this Entity self, int type)
        {
            await LocationProxyComponent.Instance.Remove(type, self.Id);
        }
    }
}