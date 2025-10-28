using Game;
using GameFramework;

namespace ET
{
    /// <summary>
    /// ET使用GF的UIWidget基类
    /// </summary>
    /// 界面Widget的预制体绑定代码可以直接使用此类的子类
    [EnableClass]
    public abstract class ETMonoUGFUIWidget : AUIWidget
    {
        private UGFUIForm ugfUIForm;
        private UGFUIWidget ugfUIWidget;
        internal AETMonoUGFUIForm etMonoUGFUIForm { private get; set; }

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            ETMonoUGFUIWidgetData widgetData = (ETMonoUGFUIWidgetData)userData;
            ugfUIForm = widgetData.UGFUIForm;
            ugfUIWidget = widgetData.UGFUIWidget;
            ReferencePool.Release(widgetData);
            ugfUIWidget.ETMono = this;
            ugfUIWidget.CachedTransform = CachedTransform;
            UGFEntitySystemSingleton.Instance.UGFUIWidgetOnInit(ugfUIWidget);
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            UGFEntitySystemSingleton.Instance.UGFUIWidgetOnOpen(ugfUIWidget);
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            UGFEntitySystemSingleton.Instance.UGFUIWidgetOnClose(ugfUIWidget, isShutdown);
            base.OnClose(isShutdown, userData);
        }

        protected override void OnPause()
        {
            base.OnPause();
            UGFEntitySystemSingleton.Instance.UGFUIWidgetOnPause(ugfUIWidget);
        }

        protected override void OnResume()
        {
            base.OnResume();
            UGFEntitySystemSingleton.Instance.UGFUIWidgetOnResume(ugfUIWidget);
        }

        protected override void OnCover()
        {
            base.OnCover();
            UGFEntitySystemSingleton.Instance.UGFUIWidgetOnCover(ugfUIWidget);
        }

        protected override void OnReveal()
        {
            base.OnReveal();
            UGFEntitySystemSingleton.Instance.UGFUIWidgetOnReveal(ugfUIWidget);
        }

        protected override void OnRefocus(object userData)
        {
            base.OnRefocus(userData);
            UGFEntitySystemSingleton.Instance.UGFUIWidgetOnRefocus(ugfUIWidget);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            UGFEntitySystemSingleton.Instance.UGFUIWidgetOnUpdate(ugfUIWidget, elapseSeconds, realElapseSeconds);
        }

        protected override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            base.OnDepthChanged(uiGroupDepth, depthInUIGroup);
            UGFEntitySystemSingleton.Instance.UGFUIWidgetOnDepthChanged(ugfUIWidget, uiGroupDepth, depthInUIGroup);
        }

        protected override void OnRecycle()
        {
            base.OnRecycle();
            UGFEntitySystemSingleton.Instance.UGFUIWidgetOnRecycle(ugfUIWidget);
        }

        internal void Open()
        {
            ugfUIForm.ETMono.OpenUIWidget(this);
        }

        internal void DynamicOpen()
        {
            ugfUIForm.ETMono.DynamicOpenUIWidget(this);
        }

        internal void Close()
        {
            ugfUIForm.ETMono.CloseUIWidget(this);
        }
    }
}
