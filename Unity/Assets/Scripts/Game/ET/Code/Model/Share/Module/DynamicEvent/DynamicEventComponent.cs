using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace ET
{
    [EntitySystemOf(typeof(DynamicEventComponent))]
    [FriendOf(typeof(DynamicEventComponent))]
    public static partial class DynamicEventComponentSystem
    {
        private const int FullRemoveIntervalTime = 60 * 1000;

        [EntitySystem]
        private static void Awake(this DynamicEventComponent self)
        {
        }

        [EntitySystem]
        private static void Update(this DynamicEventComponent self)
        {
            foreach (EntityRef<Entity> entityRef in self.NeedRemoveEntities)
            {
                Entity entity = entityRef;
                if (entity != null && self.RegisteredEntityDict.TryGetValue(entity.GetType(), out List<EntityRef<Entity>> entityRefs))
                {
                    entityRefs.Remove(entityRef);
                }
            }

            self.NeedRemoveEntities.Clear();
            if (TimeInfo.Instance.FrameTime - self.RemoveTime < FullRemoveIntervalTime)
            {
                return;
            }

            self.RemoveTime = TimeInfo.Instance.FrameTime;
            foreach (List<EntityRef<Entity>> entityRefs in self.RegisteredEntityDict.Values)
            {
                for (int i = entityRefs.Count - 1; i >= 0; --i)
                {
                    Entity entity = entityRefs[i];
                    if (entity == null)
                    {
                        entityRefs.RemoveAt(i);
                    }
                }
            }
        }

        internal static void RegisterEntity(this DynamicEventComponent self, Entity entity)
        {
            Type entityType = entity.GetType();
            if (!self.RegisteredEntityDict.TryGetValue(entityType, out List<EntityRef<Entity>> entityRefs))
            {
                entityRefs = new List<EntityRef<Entity>>();
                self.RegisteredEntityDict.Add(entityType, entityRefs);
            }

            EntityRef<Entity> entityRef = entity;
            if (!entityRefs.Contains(entityRef))
            {
                entityRefs.Add(entityRef);
            }

            self.NeedRemoveEntities.Remove(entityRef);
        }

        internal static void UnRegisterEntity(this DynamicEventComponent self, Entity entity)
        {
            EntityRef<Entity> entityRef = entity;
            if (!self.NeedRemoveEntities.Contains(entityRef))
            {
                self.NeedRemoveEntities.Add(entityRef);
            }
        }

        public static void Publish<A>(this DynamicEventComponent self, A arg) where A : struct
        {
            self.Publish(self.Scene().SceneType, arg);
        }

        public static UniTask PublishAsync<A>(this DynamicEventComponent self, A arg) where A : struct
        {
            return self.PublishAsync(self.Scene().SceneType, arg);
        }

        public static void Publish<A>(this DynamicEventComponent self, SceneType sceneType, A arg) where A : struct
        {
            Type argType = typeof(A);
            if (!DynamicEventTypeSystem.Instance.AllEventInfos.TryGetValue(argType, out List<DynamicEventInfo> dynamicEventInfos))
            {
                return;
            }

            using ListComponent<int> removeIndexList = ListComponent<int>.Create();
            foreach (DynamicEventInfo dynamicEventInfo in dynamicEventInfos)
            {
                if (!sceneType.HasSameFlag(dynamicEventInfo.SceneType))
                {
                    continue;
                }

                IDynamicEvent<A> dynamicEvent = (IDynamicEvent<A>)dynamicEventInfo.DynamicEvent;
                if (!self.RegisteredEntityDict.TryGetValue(dynamicEvent.EntityType, out List<EntityRef<Entity>> entityRefs))
                {
                    continue;
                }

                removeIndexList.Clear();
                for (int i = 0; i < entityRefs.Count; ++i)
                {
                    Entity entity = entityRefs[i];
                    if (entity != null)
                    {
                        dynamicEvent.Handle(entity, arg).Forget();
                    }
                    else
                    {
                        removeIndexList.Add(i);
                    }
                }

                for (int i = removeIndexList.Count - 1; i >= 0; --i)
                {
                    entityRefs.RemoveAt(removeIndexList[i]);
                }
            }
        }

        public static async UniTask PublishAsync<A>(this DynamicEventComponent self, SceneType sceneType, A arg) where A : struct
        {
            Type argType = typeof(A);
            if (!DynamicEventTypeSystem.Instance.AllEventInfos.TryGetValue(argType, out List<DynamicEventInfo> dynamicEventInfos))
            {
                return;
            }

            using ListComponent<UniTask> taskList = ListComponent<UniTask>.Create();
            using ListComponent<int> removeIndexList = ListComponent<int>.Create();
            foreach (DynamicEventInfo dynamicEventInfo in dynamicEventInfos)
            {
                if (!sceneType.HasSameFlag(dynamicEventInfo.SceneType))
                {
                    continue;
                }

                IDynamicEvent<A> dynamicEvent = (IDynamicEvent<A>)dynamicEventInfo.DynamicEvent;
                if (!self.RegisteredEntityDict.TryGetValue(dynamicEvent.EntityType, out List<EntityRef<Entity>> entityRefs))
                {
                    continue;
                }

                removeIndexList.Clear();
                for (int i = 0; i < entityRefs.Count; ++i)
                {
                    Entity entity = entityRefs[i];
                    if (entity != null)
                    {
                        taskList.Add(dynamicEvent.Handle(entity, arg));
                    }
                    else
                    {
                        removeIndexList.Add(i);
                    }
                }

                for (int i = removeIndexList.Count - 1; i >= 0; --i)
                {
                    entityRefs.RemoveAt(removeIndexList[i]);
                }
            }

            if (taskList.Count == 0)
            {
                return;
            }

            try
            {
                await UniTask.WhenAll(taskList);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

    }

    [ComponentOf(typeof(Scene))]
    public sealed class DynamicEventComponent : Entity, IAwake, IUpdate
    {
        internal readonly Dictionary<Type, List<EntityRef<Entity>>> RegisteredEntityDict = new();

        internal readonly List<EntityRef<Entity>> NeedRemoveEntities = new();

        internal long RemoveTime;
    }
}
