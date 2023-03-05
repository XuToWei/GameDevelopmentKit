using System;
using System.Collections.Generic;

namespace ET
{
    [EnableMethod]
    [ComponentOf(typeof(Scene))]
    public sealed class UGFEventComponent : Entity, IAwake, ILoad
    {
        [StaticField] public static UGFEventComponent Instance;

        private readonly Dictionary<int, IUGFUIFormEvent> UIFormEvents = new Dictionary<int, IUGFUIFormEvent>();

        private readonly Dictionary<Type, IUGFEntityEvent> EntityEvents = new Dictionary<Type, IUGFEntityEvent>();
        
        internal void Init()
        {
            this.UIFormEvents.Clear();
            HashSet<Type> uiEventAttributes = EventSystem.Instance.GetTypes(typeof (UGFUIFormEventAttribute));
            foreach (Type type in uiEventAttributes)
            {
                object[] attrs = type.GetCustomAttributes(typeof(UGFUIFormEventAttribute), false);
                UGFUIFormEventAttribute ugfUIFormEventAttribute = (UGFUIFormEventAttribute)attrs[0];
                IUGFUIFormEvent ugfUIFormEvent = Activator.CreateInstance(type) as IUGFUIFormEvent;
                this.UIFormEvents.Add(ugfUIFormEventAttribute.uiFormId, ugfUIFormEvent);
            }
            
            this.EntityEvents.Clear();
            
            Dictionary<string, Type> types = EventSystem.Instance.GetTypes();
            foreach (Type type in types.Values)
            {
                if (!type.IsSubclassOf(typeof (IUGFEntityEvent)) || type.IsGenericType || type.IsAbstract)
                {
                    continue;
                }
                IUGFEntityEvent ugfEntityEvent = Activator.CreateInstance(type) as IUGFEntityEvent;
                this.EntityEvents.Add(type, ugfEntityEvent);
            }
        }
        
        public bool TryGetUIFormEvent(int uiFormId, out IUGFUIFormEvent uiFormEvent)
        {
            return this.UIFormEvents.TryGetValue(uiFormId, out uiFormEvent);
        }
        
        public bool TryGetEntityEvent(Type entityType, out IUGFEntityEvent entityEvent)
        {
            return this.EntityEvents.TryGetValue(entityType, out entityEvent);
        }
    }
    
    [FriendOf(typeof(UGFEventComponent))]
    public static class UGFEventComponentSystem
    {
        [ObjectSystem]
        public class UGFEventComponentAwakeSystem : AwakeSystem<UGFEventComponent>
        {
            protected override void Awake(UGFEventComponent self)
            {
                UGFEventComponent.Instance = self;
                self.Init();
            }
        }

        [ObjectSystem]
        public class UGFEventComponentLoadSystem : LoadSystem<UGFEventComponent>
        {
            protected override void Load(UGFEventComponent self)
            {
                self.Init();
            }
        }
    }
}
