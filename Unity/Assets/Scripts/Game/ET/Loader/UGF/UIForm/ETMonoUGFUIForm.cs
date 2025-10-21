using Game;
using GameFramework;
using UnityEngine;

namespace ET
{
    /// <summary>
    /// ET使用GF的UIForm基类
    /// </summary>
    /// 界面的预制体绑定代码可以直接使用此类的子类
    [DisallowMultipleComponent]
    public abstract class ETMonoUGFUIForm : AUGuiForm
    {
        private UGFUIForm m_UGFUIForm;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            ETMonoUGFUIFormData formData = (ETMonoUGFUIFormData)userData;
            m_UGFUIForm = formData.UGFUIForm;
            UGFEntitySystemSingleton.Instance.UGFUIFormOnOpen(m_UGFUIForm);
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            ETMonoUGFUIFormData formData = (ETMonoUGFUIFormData)userData;
            m_UGFUIForm = formData.UGFUIForm;
            UGFEntitySystemSingleton.Instance.UGFUIFormOnOpen(m_UGFUIForm);
            ReferencePool.Release(formData);
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            UGFEntitySystemSingleton.Instance.UGFUIFormOnClose(m_UGFUIForm, isShutdown);
            base.OnClose(isShutdown, userData);
        }

        protected override void OnPause()
        {
            base.OnPause();
            UGFEntitySystemSingleton.Instance.UGFUIFormOnPause(m_UGFUIForm);
        }

        protected override void OnResume()
        {
            base.OnResume();
            UGFEntitySystemSingleton.Instance.UGFUIFormOnResume(m_UGFUIForm);
        }

        protected override void OnCover()
        {
            base.OnCover();
            UGFEntitySystemSingleton.Instance.UGFUIFormOnCover(m_UGFUIForm);
        }

        protected override void OnReveal()
        {
            base.OnReveal();
            UGFEntitySystemSingleton.Instance.UGFUIFormOnReveal(m_UGFUIForm);
        }

        protected override void OnRefocus(object userData)
        {
            base.OnRefocus(userData);
            UGFEntitySystemSingleton.Instance.UGFUIFormOnRefocus(m_UGFUIForm);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            UGFEntitySystemSingleton.Instance.UGFUIFormOnUpdate(m_UGFUIForm);
        }

        protected override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            base.OnDepthChanged(uiGroupDepth, depthInUIGroup);
            UGFEntitySystemSingleton.Instance.UGFUIFormOnDepthChanged(m_UGFUIForm, uiGroupDepth, depthInUIGroup);
        }

        protected override void OnRecycle()
        {
            base.OnRecycle();
            UGFEntitySystemSingleton.Instance.UGFUIFormOnRecycle(m_UGFUIForm);
        }
    }
}
