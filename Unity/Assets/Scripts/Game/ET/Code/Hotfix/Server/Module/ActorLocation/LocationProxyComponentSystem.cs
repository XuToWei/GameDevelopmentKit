using System;
using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [ObjectSystem]
    public class LocationProxyComponentAwakeSystem: AwakeSystem<LocationProxyComponent>
    {
        protected override void Awake(LocationProxyComponent self)
        {
            LocationProxyComponent.Instance = self;
        }
    }
    
    [ObjectSystem]
    public class LocationProxyComponentDestroySystem: DestroySystem<LocationProxyComponent>
    {
        protected override void Destroy(LocationProxyComponent self)
        {
            LocationProxyComponent.Instance = null;
        }
    }

    public static class LocationProxyComponentSystem
    {
        private static long GetLocationSceneId(long key)
        {
            return Tables.Instance.DTStartSceneConfig.LocationConfig.InstanceId;
        }

        public static async UniTask Add(this LocationProxyComponent self, long key, long instanceId)
        {
            Log.Info($"location proxy add {key}, {instanceId} {TimeHelper.ServerNow()}");
            await ActorMessageSenderComponent.Instance.Call(GetLocationSceneId(key),
                new ObjectAddRequest() { Key = key, InstanceId = instanceId });
        }

        public static async UniTask Lock(this LocationProxyComponent self, long key, long instanceId, int time = 60000)
        {
            Log.Info($"location proxy lock {key}, {instanceId} {TimeHelper.ServerNow()}");
            await ActorMessageSenderComponent.Instance.Call(GetLocationSceneId(key),
                new ObjectLockRequest() { Key = key, InstanceId = instanceId, Time = time });
        }

        public static async UniTask UnLock(this LocationProxyComponent self, long key, long oldInstanceId, long instanceId)
        {
            Log.Info($"location proxy unlock {key}, {instanceId} {TimeHelper.ServerNow()}");
            await ActorMessageSenderComponent.Instance.Call(GetLocationSceneId(key),
                new ObjectUnLockRequest() { Key = key, OldInstanceId = oldInstanceId, InstanceId = instanceId });
        }

        public static async UniTask Remove(this LocationProxyComponent self, long key)
        {
            Log.Info($"location proxy add {key}, {TimeHelper.ServerNow()}");
            await ActorMessageSenderComponent.Instance.Call(GetLocationSceneId(key),
                new ObjectRemoveRequest() { Key = key });
        }

        public static async UniTask<long> Get(this LocationProxyComponent self, long key)
        {
            if (key == 0)
            {
                throw new Exception($"get location key 0");
            }

            // location server配置到共享区，一个大战区可以配置N多个location server,这里暂时为1
            ObjectGetResponse response =
                    (ObjectGetResponse) await ActorMessageSenderComponent.Instance.Call(GetLocationSceneId(key),
                        new ObjectGetRequest() { Key = key });
            return response.InstanceId;
        }

        public static async UniTask AddLocation(this Entity self)
        {
            await LocationProxyComponent.Instance.Add(self.Id, self.InstanceId);
        }

        public static async UniTask RemoveLocation(this Entity self)
        {
            await LocationProxyComponent.Instance.Remove(self.Id);
        }
    }
}