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
        private UGFUIForm m_UGFUIForm;
        private UGFUIWidget m_UGFUIWidget;
        private UGFUIWidget m_ParentUGFUIWidget;
        private UIWidgetContainer m_UIWidgetContainer;

        public UGFUIWidget UGFUIWidget => m_UGFUIWidget;
        public UGFUIForm UGFUIForm => m_UGFUIForm;
        public UGFUIWidget ParentUGFUIWidget => m_ParentUGFUIWidget;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            ETMonoUGFUIWidgetData widgetData = (ETMonoUGFUIWidgetData)userData;
            m_UGFUIForm = widgetData.UGFUIForm;
            m_ParentUGFUIWidget = widgetData.ParentUGFUIWidget;
            m_UGFUIWidget = widgetData.UGFUIWidget;
            ReferencePool.Release(widgetData);
            m_UGFUIWidget.UGFMono = this;
            m_UGFUIWidget.CachedTransform = CachedTransform;
            UGFSystemSingleton.Instance.UGFUIWidgetOnInit(m_UGFUIWidget);

            UGFList<AETMonoUGFUIWidget> monoUIWidgets = new UGFList<AETMonoUGFUIWidget>();
            GetComponentsInChildren(true, monoUIWidgets);
            foreach (AETMonoUGFUIWidget monoUIWidget in monoUIWidgets)
            {
                if (monoUIWidget == this || monoUIWidget.UIFormOwner != null || monoUIWidget.GetComponentInParent<AETMonoUGFUIWidget>(true) != this)
                    continue;
                m_UGFUIWidget.AddChildUIWidget(monoUIWidget, true);
            }
            monoUIWidgets.Dispose();
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
            RemoveAllUIWidget();
            ClearContainer();
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            UGFSystemSingleton.Instance.UGFUIWidgetOnOpen(m_UGFUIWidget);

            if (m_UIWidgetContainer != null)
            {
                foreach (AUIWidget uiWidget in m_UIWidgetContainer.UIWidgets)
                {
                    if (uiWidget.Visible && !uiWidget.Available)
                    {
                        m_UIWidgetContainer.OpenUIWidget(uiWidget);
                    }
                }
            }
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            m_UIWidgetContainer?.OnClose(isShutdown, userData);
            UGFSystemSingleton.Instance.UGFUIWidgetOnClose(m_UGFUIWidget, isShutdown);
            if (isShutdown)
            {
                RemoveAllUIWidget();
                ClearContainer();
            }
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

        internal void Open()
        {
            m_UGFUIForm.UGFMono.OpenUIWidget(this);
        }

        internal void DynamicOpen()
        {
            m_UGFUIForm.UGFMono.DynamicOpenUIWidget(this);
        }

        internal void Close()
        {
            m_UGFUIForm.UGFMono.CloseUIWidget(this);
        }

        internal bool Has()
        {
            return m_UGFUIForm.UGFMono.HasUIWidget(this);
        }

        internal void Remove()
        {
            m_UGFUIForm.UGFMono.RemoveUIWidget(this);
        }
        
        public void AddUIWidget(AUIWidget auiWidget, ETMonoUGFUIWidgetData widgetData)
        {
            if (m_UIWidgetContainer == null)
            {
                m_UIWidgetContainer = UIWidgetContainer.Create(UIFormOwner);
            }
            m_UIWidgetContainer.AddUIWidget(auiWidget, widgetData);
        }

        public bool HasUIWidget(AUIWidget auiWidget)
        {
            if (m_UIWidgetContainer == null)
            {
                return false;
            }
            return m_UIWidgetContainer.HasUIWidget(auiWidget);
        }

        public void RemoveUIWidget(AUIWidget auiWidget)
        {
            if (m_UIWidgetContainer == null)
            {
                throw new GameFrameworkException("Container is empty!");
            }
            m_UIWidgetContainer.RemoveUIWidget(auiWidget);
        }

        public void RemoveAllUIWidget()
        {
            if (m_UIWidgetContainer == null)
                return;
            m_UIWidgetContainer.RemoveAllUIWidget();
        }

        /// <summary>
        /// 打开UIWidget，不刷新Depth，一般在UIForm的OnOpen中调用
        /// </summary>
        /// <param name="auiWidget"></param>
        /// <param name="userData"></param>
        /// <exception cref="GameFrameworkException"></exception>
        public void OpenUIWidget(AUIWidget auiWidget)
        {
            if (m_UIWidgetContainer == null)
            {
                throw new GameFrameworkException("Container is empty!");
            }
            m_UIWidgetContainer.OpenUIWidget(auiWidget);
        }

        /// <summary>
        /// 动态打开UIWidget，刷新Depth
        /// </summary>
        /// <param name="auiWidget"></param>
        /// <param name="userData"></param>
        /// <exception cref="GameFrameworkException"></exception>
        public void DynamicOpenUIWidget(AUIWidget auiWidget, object userData = null)
        {
            if (m_UIWidgetContainer == null)
            {
                throw new GameFrameworkException("Container is empty!");
            }
            m_UIWidgetContainer.DynamicOpenUIWidget(auiWidget, userData);
        }

        public void CloseUIWidget(AUIWidget uiWidget, bool isShutdown = false, object userData = null)
        {
            if (m_UIWidgetContainer == null)
            {
                throw new GameFrameworkException("Container is empty!");
            }
            m_UIWidgetContainer.CloseUIWidget(uiWidget, isShutdown, userData);
        }

        public void CloseAllUIWidgets(bool isShutdown = false, object userData = null)
        {
            if (m_UIWidgetContainer == null)
                return;
            m_UIWidgetContainer.CloseAllUIWidgets(isShutdown, userData);
        }
    }
}
