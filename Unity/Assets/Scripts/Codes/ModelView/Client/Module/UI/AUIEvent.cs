namespace ET.Client
{
    public abstract class AUIEvent
    {
        public virtual void OnInit(UI ui, object userData)
        {
            
        }

        public virtual void OnRecycle(UI ui)
        {
            
        }

        public virtual void OnDestroy(UI ui)
        {
            
        }

        public virtual void OnOpen(UI ui, object userData)
        {
            
        }

        public virtual void OnClose(UI ui, bool isShutdown, object userData)
        {
            
        }

        public virtual void OnPause(UI ui)
        {
            
        }

        public virtual void OnResume(UI ui)
        {
            
        }

        public virtual void OnCover(UI ui)
        {
            
        }

        public virtual void OnReveal(UI ui)
        {
            
        }

        public virtual void OnRefocus(UI ui, object userData)
        {
            
        }

        public virtual void OnUpdate(UI ui, float elapseSeconds, float realElapseSeconds)
        {
            
        }

        public virtual void OnDepthChanged(UI ui, int uiGroupDepth, int depthInUIGroup)
        {
            
        }
    }
}