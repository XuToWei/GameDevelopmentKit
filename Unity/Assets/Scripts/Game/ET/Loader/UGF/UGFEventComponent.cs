using System;
using System.Collections.Generic;

namespace ET
{
    [EnableMethod]
    [ComponentOf]
    public sealed class UGFEventComponent : Entity, IAwake, IDestroy, ILoad
    {
        [StaticField] public static UGFEventComponent Instance;

        private readonly Dictionary<int, IUGFUIFormEvent> m_UIFormEvents = new Dictionary<int, IUGFUIFormEvent>();

        private readonly Dictionary<Type, IUGFEntityEvent> m_EntityEvents = new Dictionary<Type, IUGFEntityEvent>();

        internal void Clear()
        {
            this.m_UIFormEvents.Clear();
            this.m_EntityEvents.Clear();
        }
        
        internal void Init()
        {
            this.m_UIFormEvents.Clear();
            HashSet<Type> uiEventAttributes = EventSystem.Instance.GetTypes(typeof (UGFUIFormEventAttribute));
            foreach (Type type in uiEventAttributes)
            {
                object[] attrs = type.GetCustomAttributes(typeof(UGFUIFormEventAttribute), false);
                UGFUIFormEventAttribute ugfUIFormEventAttribute = (UGFUIFormEventAttribute)attrs[0];
                IUGFUIFormEvent ugfUIFormEvent = Activator.CreateInstance(type) as IUGFUIFormEvent;
                foreach (int uiFormId in ugfUIFormEventAttribute.uiFormIds)
                {
                    this.m_UIFormEvents.Add(uiFormId, ugfUIFormEvent);
                }
            }
            this.m_EntityEvents.Clear();
            
            Dictionary<string, Type> types = EventSystem.Instance.GetTypes();
            Type entityEventType = typeof(IUGFEntityEvent);
            foreach (Type type in types.Values)
            {
                if (type.IsGenericType || type.IsAbstract || !entityEventType.IsAssignableFrom(type))
                {
                    continue;
                }
                IUGFEntityEvent ugfEntityEvent = Activator.CreateInstance(type) as IUGFEntityEvent;
                this.m_EntityEvents.Add(type, ugfEntityEvent);
            }
        }
        
        public bool TryGetUIFormEvent(int uiFormId, out IUGFUIFormEvent uiFormEvent)
        {
            return this.m_UIFormEvents.TryGetValue(uiFormId, out uiFormEvent);
        }
        
        public bool TryGetEntityEvent(Type entityType, out IUGFEntityEvent entityEvent)
        {
            return this.m_EntityEvents.TryGetValue(entityType, out entityEvent);
        }
    }
    
    [FriendOf(typeof(UGFEventComponent))]
    public static class UGFEventComponentSystem
    {
        [EntitySystem]
        private class UGFEventComponentAwakeSystem : AwakeSystem<UGFEventComponent>
        {
            protected override void Awake(UGFEventComponent self)
            {
                if (UGFEventComponent.Instance != null)
                {
                    throw new Exception("UGFEventComponent already exist!");
                }
                UGFEventComponent.Instance = self;
                self.Init();
            }
        }

        [EntitySystem]
        private class UGFEventComponentDestroySystem : DestroySystem<UGFEventComponent>
        {
            protected override void Destroy(UGFEventComponent self)
            {
                self.Clear();
                UGFEventComponent.Instance = null;
            }
        }

        [EntitySystem]
        private class UGFEventComponentLoadSystem : LoadSystem<UGFEventComponent>
        {
            protected override void Load(UGFEventComponent self)
            {
                self.Init();
            }
        }
    }
}
