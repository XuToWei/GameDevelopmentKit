namespace ET
{
    public class DynamicEventInfo
    {
        public SceneType SceneType { get; }
        public IDynamicEvent DynamicEvent { get; }

        public DynamicEventInfo(SceneType sceneType, IDynamicEvent iDynamicEvent)
        {
            this.SceneType = sceneType;
            this.DynamicEvent = iDynamicEvent;
        }
    }
}
