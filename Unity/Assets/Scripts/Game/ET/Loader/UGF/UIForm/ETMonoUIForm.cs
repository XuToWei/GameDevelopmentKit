using System;
using Game;
using GameFramework;
using UnityEngine;

namespace ET
{
    [DisallowMultipleComponent]
    public sealed class ETMonoUIForm : UGuiForm
    {
        public UGFUIForm ugfUIForm { get; private set; }
        public int uiFormId { get; private set; }
        
        private bool m_IsInit = false;
        private IUGFUIFormEvent m_UIFormEvent;

        public void OnLoad()
        {
            if(UGFEventComponent.Instance.TryGetUIFormEvent(this.uiFormId, out IUGFUIFormEvent uiFormEvent))
            {
                this.m_UIFormEvent = uiFormEvent;
            }
            else
            {
                this.m_UIFormEvent = default;
                throw new GameFrameworkException($"UIFormId {this.uiFormId} doesn't exist UIFormEvent!");
            }
            this.m_UIFormEvent.OnLoad(this.ugfUIForm);
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            ETMonoUIFormData formData = (ETMonoUIFormData)userData;
            this.uiFormId = formData.UIFormId;
            if(UGFEventComponent.Instance.TryGetUIFormEvent(this.uiFormId, out IUGFUIFormEvent uiFormEvent))
            {
                this.m_UIFormEvent = uiFormEvent;
            }
            else
            {
                this.m_UIFormEvent = default;
                throw new GameFrameworkException($"UIFormId {this.uiFormId} doesn't exist UIFormEvent!");
            }
            if (!this.m_IsInit)
            {
                this.m_IsInit = true;
                this.ugfUIForm = formData.ParentEntity.AddChild<UGFUIForm, int, ETMonoUIForm>(this.uiFormId, this);
                this.m_UIFormEvent?.OnInit(this.ugfUIForm, formData.UserData);
            }
            else
            {
                this.ugfUIForm.SetUIFormId(this.uiFormId);
            }
            this.ugfUIForm.isOpen = true;
            this.m_UIFormEvent?.OnOpen(this.ugfUIForm, formData.UserData);
            formData.Release();
        }
        
        private void OnDestroy()
        {
            if (this.ugfUIForm != default)
            {
                this.ugfUIForm.Dispose();
                this.ugfUIForm = default;
            }
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            this.m_UIFormEvent?.OnClose(this.ugfUIForm, isShutdown, userData);
            this.ugfUIForm.isOpen = false;
            if (isShutdown)
            {
                this.ugfUIForm.Dispose();
                this.ugfUIForm = default;
            }
            base.OnClose(isShutdown, userData);
        }

        protected override void OnPause()
        {
            base.OnPause();
            this.m_UIFormEvent?.OnPause(this.ugfUIForm);
        }

        protected override void OnResume()
        {
            base.OnResume();
            this.m_UIFormEvent?.OnResume(this.ugfUIForm);
        }

        protected override void OnCover()
        {
            base.OnCover();
            this.m_UIFormEvent?.OnCover(this.ugfUIForm);
        }

        protected override void OnReveal()
        {
            base.OnReveal();
            this.m_UIFormEvent?.OnReveal(this.ugfUIForm);
        }

        protected override void OnRefocus(object userData)
        {
            base.OnRefocus(userData);
            this.m_UIFormEvent?.OnRefocus(this.ugfUIForm, userData);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            this.m_UIFormEvent?.OnUpdate(this.ugfUIForm, elapseSeconds, realElapseSeconds);
        }

        protected override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            base.OnDepthChanged(uiGroupDepth, depthInUIGroup);
            this.m_UIFormEvent?.OnDepthChanged(this.ugfUIForm, uiGroupDepth, depthInUIGroup);
        }

        protected override void OnRecycle()
        {
            base.OnRecycle();
            this.m_UIFormEvent?.OnRecycle(this.ugfUIForm);
        }
    }
}
