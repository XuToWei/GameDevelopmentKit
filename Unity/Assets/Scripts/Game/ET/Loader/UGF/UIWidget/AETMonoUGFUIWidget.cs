using System;
using Game;
using GameFramework;
using UnityGameFramework.Extension;

namespace ET
{
    /// <summary>
    /// ET使用GF的UIWidget基类
    /// </summary>
    /// 界面Widget的预制体绑定代码可以直接使用此类的子类
    [EnableClass]
    public abstract class AETMonoUGFUIWidget : AUIWidget
    {
        private AETMonoUGFUIWidget m_ParentUIWidget;
        private AETMonoUGFUIForm m_UIForm;
        private UGFUIWidget m_UGFUIWidget;
        private UIWidgetContainer m_UIWidgetContainer;

        public UGFUIWidget UGFUIWidget => m_UGFUIWidget;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            ETMonoUGFUIWidgetData widgetData = (ETMonoUGFUIWidgetData)userData;
            m_UGFUIWidget = widgetData.UGFUIWidget;
            ReferencePool.Release(widgetData);
            m_UGFUIWidget.UGFMono = this;
            m_UGFUIWidget.CachedRectTransform = CachedRectTransform;
        }

        private void ClearContainer()
        {
            if (m_UIWidgetContainer != null)
            {
                ReferencePool.Release(m_UIWidgetContainer);
                m_UIWidgetContainer = null;
            }
        }

        protected virtual void OnDestroy()
        {
            RemoveAllUIWidgets();
            ClearContainer();
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            UGFList<AETMonoUGFUIWidget> monoUIWidgets = UGFList<AETMonoUGFUIWidget>.Create();
            GetComponentsInChildren(true, monoUIWidgets);
            foreach (AETMonoUGFUIWidget monoUIWidget in monoUIWidgets)
            {
                if (monoUIWidget == this || monoUIWidget.UIFormOwner != null || monoUIWidget.GetComponentInParent<AETMonoUGFUIWidget>(true) != this)
                    continue;
                m_UGFUIWidget.AddChildUIWidget(monoUIWidget, true);
            }
            monoUIWidgets.Dispose();

            UGFSystemSingleton.Instance.UGFUIWidgetOnOpen(m_UGFUIWidget);
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            m_UIWidgetContainer?.OnClose(isShutdown, userData);
            UGFSystemSingleton.Instance.UGFUIWidgetOnClose(m_UGFUIWidget, isShutdown);
            RemoveAllUIWidgets();
            ClearContainer();
            base.OnClose(isShutdown, userData);
        }

        protected override void OnPause()
        {
            base.OnPause();
            UGFSystemSingleton.Instance.UGFUIWidgetOnPause(m_UGFUIWidget);
            m_UIWidgetContainer?.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();
            UGFSystemSingleton.Instance.UGFUIWidgetOnResume(m_UGFUIWidget);
            m_UIWidgetContainer?.OnResume();
        }

        protected override void OnCover()
        {
            base.OnCover();
            UGFSystemSingleton.Instance.UGFUIWidgetOnCover(m_UGFUIWidget);
            m_UIWidgetContainer?.OnCover();
        }

        protected override void OnReveal()
        {
            base.OnReveal();
            UGFSystemSingleton.Instance.UGFUIWidgetOnReveal(m_UGFUIWidget);
            m_UIWidgetContainer?.OnReveal();
        }

        protected override void OnRefocus(object userData)
        {
            base.OnRefocus(userData);
            UGFSystemSingleton.Instance.UGFUIWidgetOnRefocus(m_UGFUIWidget);
            m_UIWidgetContainer?.OnRefocus(userData);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            UGFSystemSingleton.Instance.UGFUIWidgetOnUpdate(m_UGFUIWidget, elapseSeconds, realElapseSeconds);
            m_UIWidgetContainer?.OnUpdate(elapseSeconds, realElapseSeconds);
        }

        protected override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            base.OnDepthChanged(uiGroupDepth, depthInUIGroup);
            UGFSystemSingleton.Instance.UGFUIWidgetOnDepthChanged(m_UGFUIWidget, uiGroupDepth, depthInUIGroup);
            m_UIWidgetContainer?.OnDepthChanged(uiGroupDepth, depthInUIGroup);
        }

        protected override void OnRecycle()
        {
            base.OnRecycle();
            UGFSystemSingleton.Instance.UGFUIWidgetOnRecycle(m_UGFUIWidget);
            m_UIWidgetContainer?.OnRecycle();
        }

        /// <summary>
        /// 打开所有UIWidget
        /// </summary>
        internal void OpenAllUIWidgets()
        {
            if (m_UIWidgetContainer == null)
                return;
            UGFList<AUIWidget> uiWidgets = UGFList<AUIWidget>.Create();
            m_UIWidgetContainer.GetAllUIWidgets(uiWidgets);
            foreach (AUIWidget uiWidget in uiWidgets)
            {
                if (!uiWidget.Available)
                {
                    m_UIWidgetContainer.OpenUIWidget(uiWidget);
                }
            }
            uiWidgets.Dispose();
        }

        internal void Open()
        {
            if(m_ParentUIWidget != null)
            {
                m_ParentUIWidget.OpenUIWidget(this);
                return;
            }

            if (m_UIForm != null)
            {
                m_UIForm.OpenUIWidget(this);
                return;
            }

            throw new GameFrameworkException("UI widget is invalid.");
        }

        internal void TryOpen()
        {
            if (Available)
                return;

            if(m_ParentUIWidget != null)
            {
                m_ParentUIWidget.OpenUIWidget(this);
                return;
            }

            if (m_UIForm != null)
            {
                m_UIForm.OpenUIWidget(this);
                return;
            }
        }

        internal void DynamicOpen()
        {
            if(m_ParentUIWidget != null)
            {
                m_ParentUIWidget.DynamicOpenUIWidget(this);
                return;
            }

            if (m_UIForm != null)
            {
                m_UIForm.DynamicOpenUIWidget(this);
                return;
            }

            throw new GameFrameworkException("UI widget is invalid.");
        }

        internal void TryDynamicOpen()
        {
            if (Available)
                return;

            if(m_ParentUIWidget != null)
            {
                m_ParentUIWidget.DynamicOpenUIWidget(this);
                return;
            }

            if (m_UIForm != null)
            {
                m_UIForm.DynamicOpenUIWidget(this);
                return;
            }
        }

        public override void SetUIFormOwner(AUIForm uiForm)
        {
            base.SetUIFormOwner(uiForm);
            m_UIForm = uiForm as AETMonoUGFUIForm;
        }

        internal void Close()
        {
            if(m_ParentUIWidget != null)
            {
                m_ParentUIWidget.CloseUIWidget(this);
                return;
            }

            if (m_UIForm != null)
            {
                m_UIForm.CloseUIWidget(this);
                return;
            }

            throw new GameFrameworkException("UI widget is invalid.");
        }

        internal void TryClose()
        {
            if (!Available)
                return;

            if(m_ParentUIWidget != null)
            {
                m_ParentUIWidget.CloseUIWidget(this);
                return;
            }

            if (m_UIForm != null)
            {
                m_UIForm.CloseUIWidget(this);
                return;
            }
        }

        internal bool Has()
        {
            if(m_ParentUIWidget != null)
            {
                return m_ParentUIWidget.HasUIWidget(this);
            }

            if (m_UIForm != null)
            {
                return m_UIForm.HasUIWidget(this);
            }

            return false;
        }

        internal void Remove()
        {
            if(m_ParentUIWidget != null)
            {
                m_ParentUIWidget.RemoveUIWidget(this);
                return;
            }

            if (m_UIForm != null)
            {
                 m_UIForm.RemoveUIWidget(this);
                 return;
            }
        }

        internal void AddUIWidget(AETMonoUGFUIWidget aUIWidget, ETMonoUGFUIWidgetData widgetData)
        {
            if (m_UIWidgetContainer == null)
            {
                m_UIWidgetContainer = UIWidgetContainer.Create(UIFormOwner);
            }
            aUIWidget.m_ParentUIWidget = this;
            m_UIWidgetContainer.AddUIWidget(aUIWidget, widgetData);
        }

        internal bool HasUIWidget(AETMonoUGFUIWidget aUIWidget)
        {
            if (m_UIWidgetContainer == null)
            {
                return false;
            }
            return m_UIWidgetContainer.HasUIWidget(aUIWidget);
        }

        internal void RemoveUIWidget(AETMonoUGFUIWidget aUIWidget)
        {
            if (m_UIWidgetContainer == null)
            {
                throw new GameFrameworkException("Container is empty!");
            }
            m_UIWidgetContainer.RemoveUIWidget(aUIWidget);
        }

        internal void RemoveAllUIWidgets()
        {
            if (m_UIWidgetContainer == null)
                return;
            m_UIWidgetContainer.RemoveAllUIWidgets();
        }

        /// <summary>
        /// 打开UIWidget，不刷新Depth，一般在UIForm的OnOpen中调用
        /// </summary>
        /// <param name="aUIWidget"></param>
        /// <param name="userData"></param>
        /// <exception cref="GameFrameworkException"></exception>
        internal void OpenUIWidget(AETMonoUGFUIWidget aUIWidget, object userData = null)
        {
            if (m_UIWidgetContainer == null)
            {
                throw new GameFrameworkException("Container is empty!");
            }
            m_UIWidgetContainer.OpenUIWidget(aUIWidget, userData);
        }

        /// <summary>
        /// 动态打开UIWidget，刷新Depth
        /// </summary>
        /// <param name="aUIWidget"></param>
        /// <param name="userData"></param>
        /// <exception cref="GameFrameworkException"></exception>
        internal void DynamicOpenUIWidget(AETMonoUGFUIWidget aUIWidget, object userData = null)
        {
            if (m_UIWidgetContainer == null)
            {
                throw new GameFrameworkException("Container is empty!");
            }
            m_UIWidgetContainer.DynamicOpenUIWidget(aUIWidget, userData);
        }

        internal void CloseUIWidget(AETMonoUGFUIWidget uiWidget, bool isShutdown = false, object userData = null)
        {
            if (m_UIWidgetContainer == null)
            {
                throw new GameFrameworkException("Container is empty!");
            }
            m_UIWidgetContainer.CloseUIWidget(uiWidget, isShutdown, userData);
        }

        internal void CloseAllUIWidgets(bool isShutdown = false, object userData = null)
        {
            if (m_UIWidgetContainer == null)
                return;
            m_UIWidgetContainer.CloseAllUIWidgets(isShutdown, userData);
        }
    }
}
