namespace ET.Client
{
    [EnableClass]
    [FriendOf(typeof(UGFUIComponent))]
    public abstract class AUGFUIFormEvent : IUGFUIFormEvent
    {
        public virtual void OnReload(UGFUIForm uiForm)
        {
        }

        public virtual void OnInit(UGFUIForm uiForm, object userData)
        {
        }

        public virtual void OnOpen(UGFUIForm uiForm, object userData)
        {
            uiForm.GetParent<UGFUIComponent>().AllOpenUIForms.Add(uiForm);
        }

        public virtual void OnClose(UGFUIForm uiForm, bool isShutdown, object userData)
        {
            uiForm.GetParent<UGFUIComponent>().AllOpenUIForms.Remove(uiForm);
        }

        public virtual void OnPause(UGFUIForm uiForm)
        {
        }

        public virtual void OnResume(UGFUIForm uiForm)
        {
        }

        public virtual void OnCover(UGFUIForm uiForm)
        {
        }

        public virtual void OnReveal(UGFUIForm uiForm)
        {
        }

        public virtual void OnRefocus(UGFUIForm uiForm, object userData)
        {
        }

        public virtual void OnUpdate(UGFUIForm uiForm, float elapseSeconds, float realElapseSeconds)
        {
        }

        public virtual void OnDepthChanged(UGFUIForm uiForm, int uiGroupDepth, int depthInUIGroup)
        {
        }

        public virtual void OnRecycle(UGFUIForm uiForm)
        {
        }
    }
}