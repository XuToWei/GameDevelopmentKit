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
        private IUGFUIFormEvent m_UIFormEvent;
        
        public bool isOpen { get; private set; }
        public UGFUIForm ugfUIForm => this.m_UGFUIForm;

        public void OnLoad()
        {
            if(UGFEventComponent.Instance.TryGetUIFormEvent(this.m_UIFormId, out IUGFUIFormEvent uiFormEvent))
            {
                this.m_UIFormEvent = uiFormEvent;
            }
            else
            {
                this.m_UIFormEvent = default;
                throw new GameFrameworkException($"UIFormId {this.m_UIFormId} doesn't exist UIFormEvent!");
            }
            this.m_UIFormEvent.OnLoad(this.m_UGFUIForm);
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
                if(UGFEventComponent.Instance.TryGetUIFormEvent(formData.UIFormId, out IUGFUIFormEvent uiFormEvent))
                {
                    this.m_UIFormEvent = uiFormEvent;
                }
                else
                {
                    this.m_UIFormEvent = default;
                    throw new GameFrameworkException($"UIFormId {this.m_UIFormId} doesn't exist UIFormEvent!");
                }
                this.m_UIFormId = formData.UIFormId;
                this.m_UGFUIForm = formData.ParentEntity.AddChild<UGFUIForm, int, ETMonoUIForm>(this.m_UIFormId, this);
                this.m_UIFormEvent.OnInit(this.m_UGFUIForm, formData.UserData);
            }
            this.isOpen = true;
            this.m_UIFormEvent.OnOpen(this.m_UGFUIForm, formData.UserData);
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
            this.m_UIFormEvent.OnClose(this.m_UGFUIForm, isShutdown, userData);
            if (isShutdown)
            {
                UGFUIFormDispose();
            }
            base.OnClose(isShutdown, userData);
        }

        protected override void OnPause()
        {
            base.OnPause();
            this.m_UIFormEvent.OnPause(this.m_UGFUIForm);
        }

        protected override void OnResume()
        {
            base.OnResume();
            this.m_UIFormEvent.OnResume(this.m_UGFUIForm);
        }

        protected override void OnCover()
        {
            base.OnCover();
            this.m_UIFormEvent.OnCover(this.m_UGFUIForm);
        }

        protected override void OnReveal()
        {
            base.OnReveal();
            this.m_UIFormEvent.OnReveal(this.m_UGFUIForm);
        }

        protected override void OnRefocus(object userData)
        {
            base.OnRefocus(userData);
            this.m_UIFormEvent.OnRefocus(this.m_UGFUIForm, userData);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            this.m_UIFormEvent.OnUpdate(this.m_UGFUIForm, elapseSeconds, realElapseSeconds);
        }

        protected override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            base.OnDepthChanged(uiGroupDepth, depthInUIGroup);
            this.m_UIFormEvent.OnDepthChanged(this.m_UGFUIForm, uiGroupDepth, depthInUIGroup);
        }

        protected override void OnRecycle()
        {
            base.OnRecycle();
            this.m_UIFormEvent.OnRecycle(this.m_UGFUIForm);
        }
    }
}
