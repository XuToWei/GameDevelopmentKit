namespace ET
{
    public interface IUGFUIWidgetEvent
    {
        void OnReload(UGFUIWidget uiWidget);

        void OnInit(UGFUIWidget uiWidget, object userData);

        void OnOpen(UGFUIWidget uiWidget, object userData);

        void OnClose(UGFUIWidget uiWidget, bool isShutdown, object userData);

        void OnPause(UGFUIWidget uiWidget);

        void OnResume(UGFUIWidget uiWidget);

        void OnCover(UGFUIWidget uiWidget);

        void OnReveal(UGFUIWidget uiWidget);

        void OnRefocus(UGFUIWidget uiWidget, object userData);

        void OnUpdate(UGFUIWidget uiWidget, float elapseSeconds, float realElapseSeconds);

        void OnDepthChanged(UGFUIWidget uiWidget, int uiGroupDepth, int depthInUIGroup);

        void OnRecycle(UGFUIWidget uiWidget);
    }
}
