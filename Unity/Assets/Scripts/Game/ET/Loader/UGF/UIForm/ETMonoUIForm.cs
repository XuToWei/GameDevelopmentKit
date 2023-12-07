using Game;
using GameFramework;
using UnityEngine;

namespace ET
{
    [DisallowMultipleComponent]
    public sealed class ETMonoUIForm : UGuiForm
    {
        private UGFUIForm m_UGFUIForm;
        private int m_UIFormId;
        private IUGFUIFormEvent ugfUIFormEvent => UGFEventComponent.Instance.GetUIFormEvent(this.m_UIFormId);
        public bool isOpen { get; private set; }
        public UGFUIForm ugfUIForm => this.m_UGFUIForm;

        public void OnReload()
        {
            ugfUIFormEvent.OnReload(this.m_UGFUIForm);
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            ETMonoUIFormData formData = (ETMonoUIFormData)userData;
            if (formData.ParentEntity == null)
            {
                throw new GameFrameworkException("ETMonoUIFormData ParentEntity is null!");
            }
            if (this.m_UGFUIForm == default || this.m_UIFormId != formData.UIFormId || formData.ParentEntity != this.m_UGFUIForm.Parent)
            {
                UGFUIFormDispose();
                this.m_UIFormId = formData.UIFormId;
                this.m_UGFUIForm = formData.ParentEntity.AddChild<UGFUIForm, int, ETMonoUIForm>(this.m_UIFormId, this);
                this.ugfUIFormEvent.OnInit(this.m_UGFUIForm, formData.UserData);
            }
            this.isOpen = true;
            this.ugfUIFormEvent.OnOpen(this.m_UGFUIForm, formData.UserData);
            formData.Release();
        }

        private void UGFUIFormDispose()
        {
            if (this.m_UGFUIForm != default && !this.m_UGFUIForm.IsDisposed)
            {
                UGFUIForm ugfUIForm = this.m_UGFUIForm;
                this.m_UGFUIForm = default;
                ugfUIForm.Dispose();
            }
        }
        
        private void OnDestroy()
        {
            UGFUIFormDispose();
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            this.isOpen = false;
            this.ugfUIFormEvent.OnClose(this.m_UGFUIForm, isShutdown, userData);
            if (isShutdown)
            {
                UGFUIFormDispose();
            }
            base.OnClose(isShutdown, userData);
        }

        protected override void OnPause()
        {
            base.OnPause();
            this.ugfUIFormEvent.OnPause(this.m_UGFUIForm);
        }

        protected override void OnResume()
        {
            base.OnResume();
            this.ugfUIFormEvent.OnResume(this.m_UGFUIForm);
        }

        protected override void OnCover()
        {
            base.OnCover();
            this.ugfUIFormEvent.OnCover(this.m_UGFUIForm);
        }

        protected override void OnReveal()
        {
            base.OnReveal();
            this.ugfUIFormEvent.OnReveal(this.m_UGFUIForm);
        }

        protected override void OnRefocus(object userData)
        {
            base.OnRefocus(userData);
            this.ugfUIFormEvent.OnRefocus(this.m_UGFUIForm, userData);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            this.ugfUIFormEvent.OnUpdate(this.m_UGFUIForm, elapseSeconds, realElapseSeconds);
        }

        protected override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            base.OnDepthChanged(uiGroupDepth, depthInUIGroup);
            this.ugfUIFormEvent.OnDepthChanged(this.m_UGFUIForm, uiGroupDepth, depthInUIGroup);
        }

        protected override void OnRecycle()
        {
            base.OnRecycle();
            this.ugfUIFormEvent.OnRecycle(this.m_UGFUIForm);
        }
    }
}
