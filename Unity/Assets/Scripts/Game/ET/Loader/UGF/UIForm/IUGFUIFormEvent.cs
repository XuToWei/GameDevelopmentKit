namespace ET
{
    public interface IUGFUIFormEvent
    {
        void OnReload(UGFUIForm uiForm);

        void OnInit(UGFUIForm uiForm, object userData);

        void OnOpen(UGFUIForm uiForm, object userData);

        void OnClose(UGFUIForm uiForm, bool isShutdown, object userData);

        void OnPause(UGFUIForm uiForm);

        void OnResume(UGFUIForm uiForm);

        void OnCover(UGFUIForm uiForm);

        void OnReveal(UGFUIForm uiForm);

        void OnRefocus(UGFUIForm uiForm, object userData);

        void OnUpdate(UGFUIForm uiForm, float elapseSeconds, float realElapseSeconds);

        void OnDepthChanged(UGFUIForm uiForm, int uiGroupDepth, int depthInUIGroup);

        void OnRecycle(UGFUIForm uiForm);
    }
}