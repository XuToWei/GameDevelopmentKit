using System;
using Game;
using GameFramework;

namespace ET
{
    public sealed class ETMonoUIForm : UGuiForm
    {
        public UGFUIForm UGFUIForm { get; private set; }
        public int UIFormId { get; private set; }
        
        private bool m_IsInitOpen = true;
        private IUGFUIFormEvent m_UIFormEvent;

        public void Load()
        {
            if(UGFEventComponent.Instance.TryGetUIFormEvent(this.UIFormId, out IUGFUIFormEvent uiFormEvent))
            {
                this.m_UIFormEvent = uiFormEvent;
            }
            else
            {
                this.m_UIFormEvent = default;
                Log.Warning($"UIFormId {this.UIFormId} doesn't exist UIFormEvent!");
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

            this.UIFormId = formData.UIFormId;
            this.UGFUIForm = formData.ParentEntity.AddChild<UGFUIForm, int, ETMonoUIForm>(this.UIFormId, this);

            Load();
            
            this.m_UIFormEvent?.OnInit(this.UGFUIForm, formData.UserData);
        }
        
        private void OnDestroy()
        {
            this.UGFUIForm.Dispose();
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            this.UGFUIForm.IsOpen = true;
            if (this.m_IsInitOpen)
            {
                this.m_IsInitOpen = false;
                ETMonoUIFormData formData = userData as ETMonoUIFormData;
                this.m_UIFormEvent?.OnOpen(this.UGFUIForm, formData.UserData);
            }
            else
            {
                this.m_UIFormEvent?.OnOpen(this.UGFUIForm, userData);
            }
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            this.m_UIFormEvent?.OnClose(this.UGFUIForm, isShutdown, userData);
            this.UGFUIForm.IsOpen = false;
            base.OnClose(isShutdown, userData);
        }

        protected override void OnPause()
        {
            base.OnPause();
            this.m_UIFormEvent?.OnPause(this.UGFUIForm);
        }

        protected override void OnResume()
        {
            base.OnResume();
            this.m_UIFormEvent?.OnResume(this.UGFUIForm);
        }

        protected override void OnCover()
        {
            base.OnCover();
            this.m_UIFormEvent?.OnCover(this.UGFUIForm);
        }

        protected override void OnReveal()
        {
            base.OnReveal();
            this.m_UIFormEvent?.OnReveal(this.UGFUIForm);
        }

        protected override void OnRefocus(object userData)
        {
            base.OnRefocus(userData);
            this.m_UIFormEvent?.OnRefocus(this.UGFUIForm, userData);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            this.m_UIFormEvent?.OnUpdate(this.UGFUIForm, elapseSeconds, realElapseSeconds);
        }

        protected override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            base.OnDepthChanged(uiGroupDepth, depthInUIGroup);
            this.m_UIFormEvent?.OnDepthChanged(this.UGFUIForm, uiGroupDepth, depthInUIGroup);
        }

        protected override void OnRecycle()
        {
            base.OnRecycle();
            this.m_UIFormEvent?.OnRecycle(this.UGFUIForm);
        }
    }
}
