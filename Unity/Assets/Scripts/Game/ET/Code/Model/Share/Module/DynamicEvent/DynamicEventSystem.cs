using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace ET
{
    public sealed class DynamicEventSystem : Singleton<DynamicEventSystem>, ISingletonAwake
    {
        private readonly Queue<EntityRef<Entity>> queue = new ();

        public void Awake()
        {
            
        }

        public void RegisterEntity(Entity entity)
        {
            if (entity == null)
            {
                return;
            }
            if (entity.IsDisposed)
            {
                return;
            }
            this.queue.Enqueue(entity);
        }

        public void UnRegisterEntity(Entity entity)
        {
            Queue<EntityRef<Entity>> tempQueue = this.queue;
            int count = tempQueue.Count;
            while (count-- > 0)
            {
                Entity tempEntity = tempQueue.Dequeue();
                if (tempEntity == null)
                {
                    continue;
                }
                if (tempEntity.IsDisposed)
                {
                    continue;
                }
                if (tempEntity == entity)
                {
                    continue;
                }
                tempQueue.Enqueue(entity);
            }
        }

        public void Publish<A>(Scene scene, A arg) where A : struct
        {
            SceneType domainSceneType = scene.SceneType;
            Type argType = typeof(A);
            if (DynamicEventTypeSystem.Instance.AllEventInfos.TryGetValue(argType, out List<DynamicEventInfo> dynamicEventInfos))
            {
                foreach (DynamicEventInfo dynamicEventInfo in dynamicEventInfos)
                {
                    if (!domainSceneType.HasSameFlag(dynamicEventInfo.SceneType))
                    {
                        continue;
                    }
                    IDynamicEvent<A> dynamicEvent = (IDynamicEvent<A>)dynamicEventInfo.DynamicEvent;
                    Queue<EntityRef<Entity>> tempQueue = this.queue;
                    int count = tempQueue.Count;
                    while (count-- > 0)
                    {
                        Entity entity = tempQueue.Dequeue();
                        if (entity == null)
                        {
                            continue;
                        }
                        if (entity.IsDisposed)
                        {
                            continue;
                        }
                        tempQueue.Enqueue(entity);
                        if (dynamicEventInfo.DynamicEvent.EntityType == entity.GetType())
                        {
                            dynamicEvent.Handle(scene, entity, arg).Forget();
                        }
                    }
                }
            }
        }

        public async UniTask PublishAsync<A>(Scene scene, A arg) where A : struct
        {
            using ListComponent<UniTask> taskList = ListComponent<UniTask>.Create();
            SceneType domainSceneType = scene.SceneType;
            Type argType = typeof(A);
            if (DynamicEventTypeSystem.Instance.AllEventInfos.TryGetValue(argType, out List<DynamicEventInfo> dynamicEventInfos))
            {
                foreach (DynamicEventInfo dynamicEventInfo in dynamicEventInfos)
                {
                    if (!domainSceneType.HasSameFlag(dynamicEventInfo.SceneType))
                    {
                        continue;
                    }
                    IDynamicEvent<A> dynamicEvent = (IDynamicEvent<A>)dynamicEventInfo.DynamicEvent;
                    Queue<EntityRef<Entity>> tempQueue = this.queue;
                    int count = tempQueue.Count;
                    while (count-- > 0)
                    {
                        Entity entity = tempQueue.Dequeue();
                        if (entity == null)
                        {
                            continue;
                        }
                        if (entity.IsDisposed)
                        {
                            continue;
                        }
                        tempQueue.Enqueue(entity);
                        if (dynamicEventInfo.DynamicEvent.EntityType == entity.GetType())
                        {
                            taskList.Add(dynamicEvent.Handle(scene, entity, arg));
                        }
                    }
                }
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
