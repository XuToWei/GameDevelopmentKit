using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace ET
{
    public sealed partial class DynamicEventSystem : Singleton<DynamicEventSystem>, ISingletonAwake
    {
        private readonly Dictionary<Type, List<EntityRef<Entity>>> registeredEntityDict = new Dictionary<Type, List<EntityRef<Entity>>>();
        private readonly List<EntityRef<Entity>> needRemoveEntities = new List<EntityRef<Entity>>();
        //60s删除一次
        private const int FULL_REMOVE_INTERVAL_TIME = 60 * 1000;
        private long removeTime = 0;

        public void Awake()
        {
            
        }

        public void Update()
        {
            foreach (EntityRef<Entity> entityRef in this.needRemoveEntities)
            {
                Entity entity = entityRef;
                if (entity != null && this.registeredEntityDict.TryGetValue(entity.GetType(), out List<EntityRef<Entity>> entityRefs))
                {
                    entityRefs.Remove(entityRef);
                }
            }
            this.needRemoveEntities.Clear();
            if (TimeInfo.Instance.FrameTime - removeTime >= FULL_REMOVE_INTERVAL_TIME)
            {
                removeTime = TimeInfo.Instance.FrameTime;
                foreach (List<EntityRef<Entity>> entityRefs in this.registeredEntityDict.Values)
                {
                    for (int i = entityRefs.Count - 1; i >= 0; i--)
                    {
                        Entity entity = entityRefs[i];
                        if (entity == null)
                        {
                            entityRefs.RemoveAt(i);
                        }
                    }
                }
            }
        }

        public void RegisterEntity(Entity entity)
        {
            Type entityType = entity.GetType();
            if (!this.registeredEntityDict.TryGetValue(entityType, out List<EntityRef<Entity>> entityRefs))
            {
                entityRefs = new List<EntityRef<Entity>>();
                this.registeredEntityDict.Add(entityType, entityRefs);
            }
            EntityRef<Entity> entityRef = entity;
            if (!entityRefs.Contains(entityRef))
            {
                entityRefs.Add(entityRef);
            }
            this.needRemoveEntities.Remove(entityRef);
        }

        public void UnRegisterEntity(Entity entity)
        {
            EntityRef<Entity> entityRef = entity;
            if (!this.needRemoveEntities.Contains(entityRef))
            {
                this.needRemoveEntities.Add(entityRef);
            }
        }

        public void Publish<A>(A arg) where A : struct
        {
            Publish(SceneType.All, arg);
        }

        public UniTask PublishAsync<A>(A arg) where A : struct
        {
            return PublishAsync(SceneType.All, arg);
        }

        public void Publish<A>(Scene scene, A arg) where A : struct
        {
            Publish(scene.SceneType, arg);
        }

        public UniTask PublishAsync<A>(Scene scene, A arg) where A : struct
        {
            return PublishAsync(scene.SceneType, arg);
        }

        public void Publish<A>(SceneType sceneType, A arg) where A : struct
        {
            Type argType = typeof(A);
            if (DynamicEventTypeSystem.Instance.AllEventInfos.TryGetValue(argType, out List<DynamicEventInfo> dynamicEventInfos))
            {
                using ListComponent<int> removeIndexList = ListComponent<int>.Create();
                foreach (DynamicEventInfo dynamicEventInfo in dynamicEventInfos)
                {
                    if (!sceneType.HasSameFlag(dynamicEventInfo.SceneType))
                    {
                        continue;
                    }
                    IDynamicEvent<A> dynamicEvent = (IDynamicEvent<A>)dynamicEventInfo.DynamicEvent;
                    if (this.registeredEntityDict.TryGetValue(dynamicEvent.EntityType, out List<EntityRef<Entity>> entityRefs))
                    {
                        removeIndexList.Clear();
                        for (int i = 0; i < entityRefs.Count; i++)
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
                        for (int i = removeIndexList.Count - 1; i >= 0; i--)
                        {
                            entityRefs.RemoveAt(removeIndexList[i]);
                        }
                    }
                }
            }
        }

        public async UniTask PublishAsync<A>(SceneType sceneType, A arg) where A : struct
        {
            Type argType = typeof(A);
            if (DynamicEventTypeSystem.Instance.AllEventInfos.TryGetValue(argType, out List<DynamicEventInfo> dynamicEventInfos))
            {
                using ListComponent<UniTask> taskList = ListComponent<UniTask>.Create();
                using ListComponent<int> removeIndexList = ListComponent<int>.Create();
                foreach (DynamicEventInfo dynamicEventInfo in dynamicEventInfos)
                {
                    if (!sceneType.HasSameFlag(dynamicEventInfo.SceneType))
                    {
                        continue;
                    }
                    IDynamicEvent<A> dynamicEvent = (IDynamicEvent<A>)dynamicEventInfo.DynamicEvent;
                    if (this.registeredEntityDict.TryGetValue(dynamicEvent.EntityType, out List<EntityRef<Entity>> entityRefs))
                    {
                        removeIndexList.Clear();
                        for (int i = 0; i < entityRefs.Count; i++)
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
                        for (int i = removeIndexList.Count - 1; i >= 0; i--)
                        {
                            entityRefs.RemoveAt(removeIndexList[i]);
                        }
                    }
                }
                if (taskList.Count < 1)
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
    }
}
