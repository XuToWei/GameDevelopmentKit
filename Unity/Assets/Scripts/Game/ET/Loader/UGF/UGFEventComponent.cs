using System;
using System.Collections.Generic;

namespace ET
{
    [Code]
    [EnableMethod]
    [ComponentOf]
    public sealed class UGFEventComponent : Singleton<UGFEventComponent>, ISingletonAwake
    {
        private readonly Dictionary<int, IUGFUIFormEvent> m_UIFormEvents = new();
        private readonly Dictionary<long, IUGFUIWidgetEvent> m_UIWidgetEvents = new();
        private readonly Dictionary<long, IUGFEntityEvent> m_EntityEvents = new();

        public bool TryGetUIFormEvent(int uiFormId, out IUGFUIFormEvent uiFormEvent)
        {
            return m_UIFormEvents.TryGetValue(uiFormId, out uiFormEvent);
        }

        public bool TryGetUIWidgetEvent(long uiWidgetEventTypeLongHashCode, out IUGFUIWidgetEvent uiWidgetEvent)
        {
            return m_UIWidgetEvents.TryGetValue(uiWidgetEventTypeLongHashCode, out uiWidgetEvent);
        }

        public bool TryGetEntityEvent(long entityEventTypeLongHashCode, out IUGFEntityEvent entityEvent)
        {
            return m_EntityEvents.TryGetValue(entityEventTypeLongHashCode, out entityEvent);
        }

        public IUGFUIFormEvent GetUIFormEvent(int uiFormId)
        {
            return m_UIFormEvents[uiFormId];
        }

        public IUGFUIWidgetEvent GetUIWidgetEvent(long uiWidgetEventTypeLongHashCode)
        {
            return m_UIWidgetEvents[uiWidgetEventTypeLongHashCode];
        }

        public IUGFEntityEvent GetEntityEvent(long entityEventTypeLongHashCode)
        {
            return m_EntityEvents[entityEventTypeLongHashCode];
        }

        public void Awake()
        {
            m_UIFormEvents.Clear();
            HashSet<Type> uiFormEventAttributes = CodeTypes.Instance.GetTypes(typeof(UGFUIFormEventAttribute));
            foreach (Type type in uiFormEventAttributes)
            {
                object[] attrs = type.GetCustomAttributes(typeof(UGFUIFormEventAttribute), false);
                UGFUIFormEventAttribute ugfUIFormEventAttribute = (UGFUIFormEventAttribute)attrs[0];
                IUGFUIFormEvent ugfUIFormEvent = Activator.CreateInstance(type) as IUGFUIFormEvent;
                foreach (int uiFormId in ugfUIFormEventAttribute.uiFormIds)
                {
                    m_UIFormEvents.Add(uiFormId, ugfUIFormEvent);
                }
            }

            HashSet<Type> uiWidgetEventAttributes = CodeTypes.Instance.GetTypes(typeof(UGFUIWidgetEventAttribute));
            foreach (Type type in uiWidgetEventAttributes)
            {
                IUGFUIWidgetEvent ugfUIWidgetEvent = Activator.CreateInstance(type) as IUGFUIWidgetEvent;
                m_UIWidgetEvents.Add(type.FullName.GetLongHashCode(), ugfUIWidgetEvent);
            }

            m_EntityEvents.Clear();
            var types = CodeTypes.Instance.GetTypes();
            Type entityEventType = typeof(IUGFEntityEvent);
            foreach (Type type in types.Values)
            {
                if (type.IsGenericType || type.IsAbstract || !entityEventType.IsAssignableFrom(type))
                {
                    continue;
                }
                IUGFEntityEvent ugfEntityEvent = Activator.CreateInstance(type) as IUGFEntityEvent;
                m_EntityEvents.Add(type.FullName.GetLongHashCode(), ugfEntityEvent);
            }
        }
    }
}
