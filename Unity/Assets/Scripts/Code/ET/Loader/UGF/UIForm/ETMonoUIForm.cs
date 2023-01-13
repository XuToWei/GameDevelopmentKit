using System;
using Game;
using GameFramework;

namespace ET
{
    public sealed class ETMonoUIForm : UGuiForm
    {
        public UGFUIForm ugfUIForm { get; private set; }
        public int uiFormId { get; private set; }
        
        private bool m_IsInitOpen = true;
        private IUGFUIFormEvent m_UIFormEvent;

        public void Load()
        {
            if(UGFEventComponent.Instance.TryGetUIFormEvent(this.uiFormId, out IUGFUIFormEvent uiFormEvent))
            {
                this.m_UIFormEvent = uiFormEvent;
            }
            else
            {
                this.m_UIFormEvent = default;
                Log.Warning($"UIFormId {this.uiFormId} doesn't exist UIFormEvent!");
            }
        }
        
        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            this.m_IsInitOpen = true;
            ETMonoUIFormData formData = userData as ETMonoUIFormData;
            if (formData == null)
            {
                throw new Exception("UGFETUIFormData is null!");
            }

            if (formData.ParentEntity == null)
            {
                throw new Exception("UGFETUIFormData ParentEntity is null!");
            }

            this.uiFormId = formData.UIFormId;
            this.ugfUIForm = formData.ParentEntity.AddChild<UGFUIForm, int, ETMonoUIForm>(this.uiFormId, this);

            Load();
            
            this.m_UIFormEvent?.OnInit(this.ugfUIForm, formData.UserData);
        }
        
        private void OnDestroy()
        {
            this.ugfUIForm.Dispose();
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            this.ugfUIForm.isOpen = true;
            if (this.m_IsInitOpen)
            {
                this.m_IsInitOpen = false;
                ETMonoUIFormData formData = userData as ETMonoUIFormData;
                this.m_UIFormEvent?.OnOpen(this.ugfUIForm, formData.UserData);
            }
            else
            {
                this.m_UIFormEvent?.OnOpen(this.ugfUIForm, userData);
            }
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            this.m_UIFormEvent?.OnClose(this.ugfUIForm, isShutdown, userData);
            this.ugfUIForm.isOpen = false;
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
