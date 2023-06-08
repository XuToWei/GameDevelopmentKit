using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace ET
{
    [EnableMethod]
    [ComponentOf(typeof(Scene))]
    public class DynamicEventWatcherComponent : Entity, IAwake, IDestroy, ILoad, IUpdate
    {
        [StaticField] public static DynamicEventWatcherComponent Instance;

        private class DynamicEventInfo
        {
            public SceneType SceneType { get; }
            public IDynamicEvent DynamicEvent { get; }

            public DynamicEventInfo(SceneType sceneType, IDynamicEvent iDynamicEvent)
            {
                this.SceneType = sceneType;
                this.DynamicEvent = iDynamicEvent;
            }
        }

        /// <summary>
        /// 参数Type：{Entity的Type：DynamicEventInfo}
        /// </summary>
        private readonly Dictionary<Type, ListComponent<DynamicEventInfo>> allDynamicEventInfos = new Dictionary<Type, ListComponent<DynamicEventInfo>>();

        private readonly HashSet<Entity> registeredEntities = new HashSet<Entity>();
        private readonly HashSet<Entity> needRemoveEntities = new HashSet<Entity>();

        public void Register(Entity entity)
        {
            this.registeredEntities.Add(entity);
        }

        public void UnRegister(Entity entity)
        {
            this.needRemoveEntities.Add(entity);
        }

        internal void Clear()
        {
            foreach (var list in this.allDynamicEventInfos.Values)
            {
                list.Dispose();
            }
            this.allDynamicEventInfos.Clear();
            this.registeredEntities.Clear();
            this.needRemoveEntities.Clear();
        }

        internal void RemoveUnRegisteredEntityIds()
        {
            if (this.needRemoveEntities.Count < 1)
            {
                return;
            }
            foreach (var id in this.needRemoveEntities)
            {
                this.registeredEntities.Remove(id);
            }
            this.needRemoveEntities.Clear();
        }

        internal void Init()
        {
            foreach (var list in this.allDynamicEventInfos.Values)
            {
                list.Dispose();
            }
            this.allDynamicEventInfos.Clear();
            HashSet<Type> types = EventSystem.Instance.GetTypes(typeof(DynamicEventAttribute));
            foreach (Type type in types)
            {
                object[] attrs = type.GetCustomAttributes(typeof(DynamicEventAttribute), false);

                foreach (object attr in attrs)
                {
                    DynamicEventAttribute dynamicEventAttribute = (DynamicEventAttribute)attr;
                    IDynamicEvent obj = (IDynamicEvent)Activator.CreateInstance(type);
                    DynamicEventInfo dynamicEventInfo = new DynamicEventInfo(dynamicEventAttribute.SceneType, obj);
                    if (!this.allDynamicEventInfos.TryGetValue(dynamicEventInfo.DynamicEvent.ArgType, out ListComponent<DynamicEventInfo> dynamicEventInfos))
                    {
                        dynamicEventInfos = ListComponent<DynamicEventInfo>.Create();
                        this.allDynamicEventInfos.Add(dynamicEventInfo.DynamicEvent.ArgType, dynamicEventInfos);
                    }
                    dynamicEventInfos.Add(dynamicEventInfo);
                }
            }
        }

        public void Publish<A>(Scene scene, A arg) where A : struct
        {
            SceneType domainSceneType = scene.SceneType;
            Type argType = typeof(A);
            if (this.allDynamicEventInfos.TryGetValue(argType, out ListComponent<DynamicEventInfo> dynamicEventInfos))
            {
                foreach (DynamicEventInfo dynamicEventInfo in dynamicEventInfos)
                {
                    if (dynamicEventInfo.SceneType != domainSceneType && dynamicEventInfo.SceneType != SceneType.None)
                    {
                        continue;
                    }
                    IDynamicEvent<A> dynamicEvent = (IDynamicEvent<A>)dynamicEventInfo.DynamicEvent;
                    foreach (Entity entity in this.registeredEntities)
                    {
                        if (entity is { IsDisposed: false } && dynamicEventInfo.DynamicEvent.EntityType == entity.GetType())
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
            if (this.allDynamicEventInfos.TryGetValue(argType, out ListComponent<DynamicEventInfo> dynamicEventInfos))
            {
                foreach (DynamicEventInfo dynamicEventInfo in dynamicEventInfos)
                {
                    if (dynamicEventInfo.SceneType != domainSceneType && dynamicEventInfo.SceneType != SceneType.None)
                    {
                        continue;
                    }
                    IDynamicEvent<A> dynamicEvent = (IDynamicEvent<A>)dynamicEventInfo.DynamicEvent;
                    foreach (Entity entity in this.registeredEntities)
                    {
                        if (entity is { IsDisposed: false } && dynamicEventInfo.DynamicEvent.EntityType == entity.GetType())
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
    
    [FriendOf(typeof(DynamicEventWatcherComponent))]
    public static class DynamicEventWatcherSystem
    {
        [EntitySystem]
        private class DynamicEventWatcherAwakeSystem : AwakeSystem<DynamicEventWatcherComponent>
        {
            protected override void Awake(DynamicEventWatcherComponent self)
            {
                DynamicEventWatcherComponent.Instance = self;
                self.Init();
            }
        }
        
        [EntitySystem]
        private class DynamicEventWatcherDestroySystem : DestroySystem<DynamicEventWatcherComponent>
        {
            protected override void Destroy(DynamicEventWatcherComponent self)
            {
                self.Clear();
                DynamicEventWatcherComponent.Instance = null;
            }
        }

        [EntitySystem]
        private class DynamicEventWatcherLoadSystem : LoadSystem<DynamicEventWatcherComponent>
        {
            protected override void Load(DynamicEventWatcherComponent self)
            {
                self.Init();
            }
        }
        
        [EntitySystem]
        private class DynamicEventWatcherUpdateSystem : UpdateSystem<DynamicEventWatcherComponent>
        {
            protected override void Update(DynamicEventWatcherComponent self)
            {
                self.RemoveUnRegisteredEntityIds();
            }
        }
    }
}