namespace ET.Client
{
    public class UnityEventHandler
    {
        [Invoke]
        public class OnApplicationPauseHandler: AInvokeHandler<OnApplicationPause>
        {
            public override void Handle(OnApplicationPause arg)
            {
                
            }
        }
        
        [Invoke]
        public class OnApplicationFocusHandler: AInvokeHandler<OnApplicationFocus>
        {
            public override void Handle(OnApplicationFocus arg)
            {
                
            }
        }
        
        [Invoke]
        public class OnShutdownHandler: AInvokeHandler<OnShutdown>
        {
            public override void Handle(OnShutdown arg)
            {
                
            }
        }
    }
}