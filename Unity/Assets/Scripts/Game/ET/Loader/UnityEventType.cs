namespace ET
{
    public struct OnApplicationPause
    {
        public bool pauseStatus
        {
            get;
        }

        public OnApplicationPause(bool pauseStatus)
        {
            this.pauseStatus = pauseStatus;
        }
    }
        
    public struct OnApplicationFocus
    {
        public bool hasFocus
        {
            get;
        }

        public OnApplicationFocus(bool hasFocus)
        {
            this.hasFocus = hasFocus;
        }
    }

    public struct OnShutdown
    {
    }

    public struct OnCodeReload
    {
    }
}
