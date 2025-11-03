using Game;
using GameFramework;

namespace ET
{
    /// <summary>
    /// ET使用GF的UIForm基类
    /// </summary>
    /// 界面的预制体绑定代码可以直接使用此类的子类
    [EnableClass]
    public abstract class AETMonoUGFUIForm : AUGuiForm
    {
        private UGFUIForm ugfUIForm;
        private UIWidgetContainer uiWidgetContainer;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            ETMonoUGFUIFormData formData = (ETMonoUGFUIFormData)userData;
            ugfUIForm = formData.UGFUIForm;
            ugfUIForm.CachedTransform = CachedTransform;
            ugfUIForm.UGFMono = this;
            UGFEntitySystemSingleton.Instance.UGFUIFormOnInit(ugfUIForm);
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            ETMonoUGFUIFormData formData = (ETMonoUGFUIFormData)userData;
            ugfUIForm = formData.UGFUIForm;
            ReferencePool.Release(formData);
            ugfUIForm.CachedTransform = CachedTransform;
            ugfUIForm.UGFMono = this;
            UGFEntitySystemSingleton.Instance.UGFUIFormOnOpen(ugfUIForm);
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            uiWidgetContainer?.OnClose(isShutdown, userData);
            UGFEntitySystemSingleton.Instance.UGFUIFormOnClose(ugfUIForm, isShutdown);
            base.OnClose(isShutdown, userData);
        }

        protected override void OnPause()
        {
            base.OnPause();
            UGFEntitySystemSingleton.Instance.UGFUIFormOnPause(ugfUIForm);
            uiWidgetContainer?.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();
            UGFEntitySystemSingleton.Instance.UGFUIFormOnResume(ugfUIForm);
            uiWidgetContainer?.OnResume();
        }

        protected override void OnCover()
        {
            base.OnCover();
            UGFEntitySystemSingleton.Instance.UGFUIFormOnCover(ugfUIForm);
            uiWidgetContainer?.OnCover();
        }

        protected override void OnReveal()
        {
            base.OnReveal();
            UGFEntitySystemSingleton.Instance.UGFUIFormOnReveal(ugfUIForm);
            uiWidgetContainer?.OnReveal();
        }

        protected override void OnRefocus(object userData)
        {
            base.OnRefocus(userData);
            UGFEntitySystemSingleton.Instance.UGFUIFormOnRefocus(ugfUIForm);
            uiWidgetContainer?.OnRefocus(userData);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            UGFEntitySystemSingleton.Instance.UGFUIFormOnUpdate(ugfUIForm, elapseSeconds, realElapseSeconds);
            uiWidgetContainer?.OnUpdate(elapseSeconds, realElapseSeconds);
        }

        protected override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            base.OnDepthChanged(uiGroupDepth, depthInUIGroup);
            UGFEntitySystemSingleton.Instance.UGFUIFormOnDepthChanged(ugfUIForm, uiGroupDepth, depthInUIGroup);
            uiWidgetContainer?.OnDepthChanged(uiGroupDepth, depthInUIGroup);
        }

        protected override void OnRecycle()
        {
            base.OnRecycle();
            UGFEntitySystemSingleton.Instance.UGFUIFormOnRecycle(ugfUIForm);
            uiWidgetContainer?.OnRecycle();
        }

        public void AddUIWidget(AUIWidget auiWidget, object userData = null)
        {
            if (uiWidgetContainer == null)
            {
                uiWidgetContainer = UIWidgetContainer.Create(this);
            }
            uiWidgetContainer.AddUIWidget(auiWidget, userData);
        }

        public void RemoveUIWidget(AUIWidget auiWidget)
        {
            if (uiWidgetContainer == null)
            {
                throw new GameFrameworkException("Container is empty!");
            }
            uiWidgetContainer.RemoveUIWidget(auiWidget);
        }

        public void RemoveAllUIWidget()
        {
            if (uiWidgetContainer == null)
                return;
            uiWidgetContainer.RemoveAllUIWidget();
        }

        /// <summary>
        /// 打开UIWidget，不刷新Depth，一般在UIForm的OnOpen中调用
        /// </summary>
        /// <param name="auiWidget"></param>
        /// <param name="userData"></param>
        /// <exception cref="GameFrameworkException"></exception>
        public void OpenUIWidget(AUIWidget auiWidget, object userData = null)
        {
            if (uiWidgetContainer == null)
            {
                throw new GameFrameworkException("Container is empty!");
            }
            uiWidgetContainer.OpenUIWidget(auiWidget, userData);
        }

        /// <summary>
        /// 动态打开UIWidget，刷新Depth
        /// </summary>
        /// <param name="auiWidget"></param>
        /// <param name="userData"></param>
        /// <exception cref="GameFrameworkException"></exception>
        public void DynamicOpenUIWidget(AUIWidget auiWidget, object userData = null)
        {
            if (uiWidgetContainer == null)
            {
                throw new GameFrameworkException("Container is empty!");
            }
            uiWidgetContainer.DynamicOpenUIWidget(auiWidget, userData);
        }

        public void CloseUIWidget(AUIWidget uiWidget, object userData = null, bool isShutdown = false)
        {
            if (uiWidgetContainer == null)
            {
                throw new GameFrameworkException("Container is empty!");
            }
            uiWidgetContainer.CloseUIWidget(uiWidget, userData, isShutdown);
        }

        public void CloseAllUIWidgets(object userData = null, bool isShutdown = false)
        {
            if (uiWidgetContainer == null)
                return;
            uiWidgetContainer.CloseAllUIWidgets(userData, isShutdown);
        }
    }
}
