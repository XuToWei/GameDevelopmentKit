using System;
using System.Collections.Generic;

namespace ET
{
    [Code]
    public class DynamicEventTypeSystem : Singleton<DynamicEventTypeSystem>, ISingletonAwake
    {
        public readonly Dictionary<Type, List<DynamicEventInfo>> AllEventInfos = new();

        public void Awake()
        {
            HashSet<Type> types = CodeTypes.Instance.GetTypes(typeof(DynamicEventAttribute));
            foreach (Type type in types)
            {
                object[] attrs = type.GetCustomAttributes(typeof(DynamicEventAttribute), false);

                foreach (object attr in attrs)
                {
                    DynamicEventAttribute dynamicEventAttribute = (DynamicEventAttribute)attr;
                    IDynamicEvent obj = (IDynamicEvent)Activator.CreateInstance(type);
                    DynamicEventInfo dynamicEventInfo = new DynamicEventInfo(dynamicEventAttribute.SceneType, obj);
                    if (!this.AllEventInfos.TryGetValue(dynamicEventInfo.DynamicEvent.ArgType, out List<DynamicEventInfo> dynamicEventInfos))
                    {
                        dynamicEventInfos = new List<DynamicEventInfo>();
                        this.AllEventInfos.Add(dynamicEventInfo.DynamicEvent.ArgType, dynamicEventInfos);
                    }
                    dynamicEventInfos.Add(dynamicEventInfo);
                }
            }
        }
    }
}
