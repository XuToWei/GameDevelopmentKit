namespace ET
{
    [EnableClass]
    public abstract class AUGFUIWidgetEvent : IUGFUIWidgetEvent
    {
        public virtual void OnReload(UGFUIWidget uiWidget)
        {
        }

        public virtual void OnInit(UGFUIWidget uiWidget, object userData)
        {
        }

        public virtual void OnOpen(UGFUIWidget uiWidget, object userData)
        {
        }

        public virtual void OnClose(UGFUIWidget uiWidget, bool isShutdown, object userData)
        {
        }

        public virtual void OnPause(UGFUIWidget uiWidget)
        {
        }

        public virtual void OnResume(UGFUIWidget uiWidget)
        {
        }

        public virtual void OnCover(UGFUIWidget uiWidget)
        {
        }

        public virtual void OnReveal(UGFUIWidget uiWidget)
        {
        }

        public virtual void OnRefocus(UGFUIWidget uiWidget, object userData)
        {
        }

        public virtual void OnUpdate(UGFUIWidget uiWidget, float elapseSeconds, float realElapseSeconds)
        {
        }

        public virtual void OnDepthChanged(UGFUIWidget uiWidget, int uiGroupDepth, int depthInUIGroup)
        {
        }

        public virtual void OnRecycle(UGFUIWidget uiWidget)
        {
        }
    }
}
