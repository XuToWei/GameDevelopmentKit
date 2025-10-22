using Game;
using GameFramework;
using UnityEngine;

namespace ET
{
    /// <summary>
    /// ET使用GF的UIWidget基类
    /// </summary>
    /// 界面Widget的预制体绑定代码可以直接使用此类的子类
    [DisallowMultipleComponent]
    public abstract class ETMonoUGFUIWidget : AUIWidget
    {
        private UGFUIWidget m_UGFUIWidget;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            ETMonoUGFUIWidgetData widgetData = (ETMonoUGFUIWidgetData)userData;
            m_UGFUIWidget = widgetData.UGFUIWidget;
            UGFEntitySystemSingleton.Instance.UGFUIWidgetOnInit(m_UGFUIWidget);
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            ETMonoUGFUIWidgetData widgetData = (ETMonoUGFUIWidgetData)userData;
            m_UGFUIWidget = widgetData.UGFUIWidget;
            UGFEntitySystemSingleton.Instance.UGFUIWidgetOnOpen(m_UGFUIWidget);
            ReferencePool.Release(widgetData);
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            UGFEntitySystemSingleton.Instance.UGFUIWidgetOnClose(m_UGFUIWidget, isShutdown);
            base.OnClose(isShutdown, userData);
        }

        protected override void OnPause()
        {
            base.OnPause();
            UGFEntitySystemSingleton.Instance.UGFUIWidgetOnPause(m_UGFUIWidget);
        }

        protected override void OnResume()
        {
            base.OnResume();
            UGFEntitySystemSingleton.Instance.UGFUIWidgetOnResume(m_UGFUIWidget);
        }

        protected override void OnCover()
        {
            base.OnCover();
            UGFEntitySystemSingleton.Instance.UGFUIWidgetOnCover(m_UGFUIWidget);
        }

        protected override void OnReveal()
        {
            base.OnReveal();
            UGFEntitySystemSingleton.Instance.UGFUIWidgetOnReveal(m_UGFUIWidget);
        }

        protected override void OnRefocus(object userData)
        {
            base.OnRefocus(userData);
            UGFEntitySystemSingleton.Instance.UGFUIWidgetOnRefocus(m_UGFUIWidget);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            UGFEntitySystemSingleton.Instance.UGFUIWidgetOnUpdate(m_UGFUIWidget, elapseSeconds, realElapseSeconds);
        }

        protected override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            base.OnDepthChanged(uiGroupDepth, depthInUIGroup);
            UGFEntitySystemSingleton.Instance.UGFUIWidgetOnDepthChanged(m_UGFUIWidget, uiGroupDepth, depthInUIGroup);
        }

        protected override void OnRecycle()
        {
            base.OnRecycle();
            UGFEntitySystemSingleton.Instance.UGFUIWidgetOnRecycle(m_UGFUIWidget);
        }
    }
}