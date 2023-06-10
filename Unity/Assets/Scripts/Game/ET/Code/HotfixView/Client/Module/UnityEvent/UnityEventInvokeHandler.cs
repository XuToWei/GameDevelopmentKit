namespace ET.Client
{
    [Invoke]
    public class UnityOnApplicationPauseInvokeHandler : AInvokeHandler<EventType.OnApplicationPause>
    {
        public override void Handle(EventType.OnApplicationPause arg)
        {
            if (Root.Instance != null)
            {
                EventSystem.Instance.Publish(Root.Instance.Scene, arg);
            }
        }
    }
    
    [Invoke]
    public class UnityOnApplicationFocusInvokeHandler : AInvokeHandler<EventType.OnApplicationFocus>
    {
        public override void Handle(EventType.OnApplicationFocus arg)
        {
            if (Root.Instance != null)
            {
                EventSystem.Instance.Publish(Root.Instance.Scene, arg);
            }
        }
    }
    
    [Invoke]
    public class UnityOnShutDownInvokeHandler : AInvokeHandler<EventType.OnShutdown>
    {
        public override void Handle(EventType.OnShutdown arg)
        {
            if (Root.Instance != null)
            {
                EventSystem.Instance.Publish(Root.Instance.Scene, arg);
            }
        }
    }
}
