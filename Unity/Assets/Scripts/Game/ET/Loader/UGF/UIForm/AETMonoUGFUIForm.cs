using Game;
using GameFramework;
using UnityGameFramework.Extension;

namespace ET
{
    /// <summary>
    /// ET使用GF的UIForm基类
    /// </summary>
    /// 界面的预制体绑定代码可以直接使用此类的子类
    [EnableClass]
    public abstract class AETMonoUGFUIForm : AUIForm
    {
        private UGFUIForm m_UGFUIForm;
        private UIWidgetContainer m_UIWidgetContainer;

        public UGFUIForm UGFUIForm => m_UGFUIForm;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            ETMonoUGFUIFormData formData = (ETMonoUGFUIFormData)userData;
            m_UGFUIForm = formData.UGFUIForm;
            m_UGFUIForm.UGFMono = this;
            m_UGFUIForm.CachedTransform = CachedTransform;
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            ETMonoUGFUIFormData formData = (ETMonoUGFUIFormData)userData;
            m_UGFUIForm = formData.UGFUIForm;
            ReferencePool.Release(formData);
            m_UGFUIForm.CachedTransform = CachedTransform;
            m_UGFUIForm.UGFMono = this;

            UGFList<AETMonoUGFUIWidget> monoUIWidgets = UGFList<AETMonoUGFUIWidget>.Create();
            GetComponentsInChildren(true, monoUIWidgets);
            foreach (AETMonoUGFUIWidget monoUIWidget in monoUIWidgets)
            {
                if(monoUIWidget.UIFormOwner != null || monoUIWidget.GetComponentInParent<AETMonoUGFUIForm>() != this)
                    continue;
                m_UGFUIForm.AddChildUIWidget(monoUIWidget, true);
            }
            monoUIWidgets.Dispose();

            UGFSystemSingleton.Instance.UGFUIFormOnOpen(m_UGFUIForm);
        }

        private void ClearContainer()
        {
            if (m_UIWidgetContainer != null)
            {
                ReferencePool.Release(m_UIWidgetContainer);
                m_UIWidgetContainer = null;
            }
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            m_UIWidgetContainer?.OnClose(isShutdown, userData);
            UGFSystemSingleton.Instance.UGFUIFormOnClose(m_UGFUIForm, isShutdown);
            RemoveAllUIWidgets();
            ClearContainer();
            base.OnClose(isShutdown, userData);
        }

        protected override void OnPause()
        {
            base.OnPause();
            UGFSystemSingleton.Instance.UGFUIFormOnPause(m_UGFUIForm);
            m_UIWidgetContainer?.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();
            UGFSystemSingleton.Instance.UGFUIFormOnResume(m_UGFUIForm);
            m_UIWidgetContainer?.OnResume();
        }

        protected override void OnCover()
        {
            base.OnCover();
            UGFSystemSingleton.Instance.UGFUIFormOnCover(m_UGFUIForm);
            m_UIWidgetContainer?.OnCover();
        }

        protected override void OnReveal()
        {
            base.OnReveal();
            UGFSystemSingleton.Instance.UGFUIFormOnReveal(m_UGFUIForm);
            m_UIWidgetContainer?.OnReveal();
        }

        protected override void OnRefocus(object userData)
        {
            base.OnRefocus(userData);
            UGFSystemSingleton.Instance.UGFUIFormOnRefocus(m_UGFUIForm);
            m_UIWidgetContainer?.OnRefocus(userData);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            UGFSystemSingleton.Instance.UGFUIFormOnUpdate(m_UGFUIForm, elapseSeconds, realElapseSeconds);
            m_UIWidgetContainer?.OnUpdate(elapseSeconds, realElapseSeconds);
        }

        protected override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            base.OnDepthChanged(uiGroupDepth, depthInUIGroup);
            UGFSystemSingleton.Instance.UGFUIFormOnDepthChanged(m_UGFUIForm, uiGroupDepth, depthInUIGroup);
            m_UIWidgetContainer?.OnDepthChanged(uiGroupDepth, depthInUIGroup);
        }

        protected override void OnRecycle()
        {
            base.OnRecycle();
            UGFSystemSingleton.Instance.UGFUIFormOnRecycle(m_UGFUIForm);
            m_UIWidgetContainer?.OnRecycle();
        }

        internal void AddUIWidget(AUIWidget auiWidget, ETMonoUGFUIWidgetData widgetData)
        {
            if (m_UIWidgetContainer == null)
            {
                m_UIWidgetContainer = UIWidgetContainer.Create(this);
            }
            m_UIWidgetContainer.AddUIWidget(auiWidget, widgetData);
        }

        internal bool HasUIWidget(AUIWidget auiWidget)
        {
            if (m_UIWidgetContainer == null)
            {
                return false;
            }
            return m_UIWidgetContainer.HasUIWidget(auiWidget);
        }

        internal void RemoveUIWidget(AUIWidget auiWidget)
        {
            if (m_UIWidgetContainer == null)
            {
                throw new GameFrameworkException("Container is empty!");
            }
            m_UIWidgetContainer.RemoveUIWidget(auiWidget);
        }

        internal void RemoveAllUIWidgets()
        {
            if (m_UIWidgetContainer == null)
                return;
            m_UIWidgetContainer.RemoveAllUIWidgets();
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

        /// <summary>
        /// 打开UIWidget，不刷新Depth，一般在UIForm的OnOpen中调用
        /// </summary>
        /// <param name="auiWidget"></param>
        /// <param name="userData"></param>
        /// <exception cref="GameFrameworkException"></exception>
        internal void OpenUIWidget(AUIWidget auiWidget, object userData = null)
        {
            if (m_UIWidgetContainer == null)
            {
                throw new GameFrameworkException("Container is empty!");
            }
            m_UIWidgetContainer.OpenUIWidget(auiWidget, userData);
        }

        /// <summary>
        /// 动态打开UIWidget，刷新Depth
        /// </summary>
        /// <param name="auiWidget"></param>
        /// <param name="userData"></param>
        /// <exception cref="GameFrameworkException"></exception>
        internal void DynamicOpenUIWidget(AUIWidget auiWidget, object userData = null)
        {
            if (m_UIWidgetContainer == null)
            {
                throw new GameFrameworkException("Container is empty!");
            }
            m_UIWidgetContainer.DynamicOpenUIWidget(auiWidget, userData);
        }

        internal void CloseUIWidget(AUIWidget uiWidget, bool isShutdown = false, object userData = null)
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
