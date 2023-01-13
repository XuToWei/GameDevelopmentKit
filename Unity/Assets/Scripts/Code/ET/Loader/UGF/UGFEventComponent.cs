using System;
using System.Collections.Generic;

namespace ET
{
    public class UGFEventComponent : Singleton<UGFEventComponent>
    {
        private readonly Dictionary<int, IUGFUIFormEvent> UIFormEvents = new Dictionary<int, IUGFUIFormEvent>();
        
        private readonly Dictionary<Type, IUGFEntityEvent> EntityEvents = new Dictionary<Type, IUGFEntityEvent>();

        public void Load()
        {
            this.UIFormEvents.Clear();
            HashSet<Type> uiEventAttributes = EventSystem.Instance.GetTypes(typeof (UGFUIFormEventAttribute));
            foreach (Type type in uiEventAttributes)
            {
                object[] attrs = type.GetCustomAttributes(typeof(UGFUIFormEventAttribute), false);
                UGFUIFormEventAttribute ugfUIFormEventAttribute = attrs[0] as UGFUIFormEventAttribute;
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
}
