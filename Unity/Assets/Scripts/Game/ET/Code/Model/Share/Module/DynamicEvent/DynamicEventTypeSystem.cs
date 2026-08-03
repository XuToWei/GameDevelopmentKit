using System;
using System.Collections.Generic;

namespace ET
{
    [EnableClass]
    internal sealed class DynamicEventInfo
    {
        public SceneType SceneType { get; }

        public IDynamicEvent DynamicEvent { get; }

        public DynamicEventInfo(SceneType sceneType, IDynamicEvent dynamicEvent)
        {
            this.SceneType = sceneType;
            this.DynamicEvent = dynamicEvent;
        }
    }

    [Code]
    internal sealed class DynamicEventTypeSystem : Singleton<DynamicEventTypeSystem>, ISingletonAwake
    {
        internal readonly Dictionary<Type, List<DynamicEventInfo>> AllEventInfos = new();

        public void Awake()
        {
            HashSet<Type> types = CodeTypes.Instance.GetTypes(typeof(DynamicEventAttribute));
            foreach (Type type in types)
            {
                object[] attrs = type.GetCustomAttributes(typeof(DynamicEventAttribute), false);
                foreach (object attr in attrs)
                {
                    DynamicEventAttribute dynamicEventAttribute = (DynamicEventAttribute)attr;
                    IDynamicEvent dynamicEvent = (IDynamicEvent)Activator.CreateInstance(type);
                    DynamicEventInfo dynamicEventInfo = new(dynamicEventAttribute.SceneType, dynamicEvent);
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
