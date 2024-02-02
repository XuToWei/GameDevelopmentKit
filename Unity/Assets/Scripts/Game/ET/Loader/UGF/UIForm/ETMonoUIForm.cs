using Game;
using GameFramework;
using UnityEngine;

namespace ET
{
    [DisallowMultipleComponent]
    public sealed class ETMonoUIForm : AUGuiForm
    {
        private UGFUIForm m_UGFUIForm;
        private int m_UIFormId;
        public bool IsOpen { get; private set; }
        public UGFUIForm UGFUIForm => m_UGFUIForm;

        public void OnReload()
        {
            UGFEventComponent.Instance.GetUIFormEvent(m_UIFormId).OnReload(m_UGFUIForm);
            if (m_UGFUIForm.UIWidgets != null)
            {
                foreach (UGFUIWidget uiWidget in m_UGFUIForm.UIWidgets)
                {
                    UGFEventComponent.Instance.GetUIWidgetEvent(uiWidget.WidgetEventType).OnReload(uiWidget);
                }
            }
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            ETMonoUIFormData formData = (ETMonoUIFormData)userData;
            if (formData.ParentEntity == null)
            {
                throw new GameFrameworkException("ETMonoUIFormData ParentEntity is null!");
            }
            if (m_UGFUIForm == default || m_UIFormId != formData.UIFormId || formData.ParentEntity != m_UGFUIForm.Parent)
            {
                UGFUIFormDispose();
                m_UIFormId = formData.UIFormId;
                m_UGFUIForm = formData.ParentEntity.AddChild<UGFUIForm, int, ETMonoUIForm>(m_UIFormId, this, true);
                UGFEventComponent.Instance.GetUIFormEvent(m_UIFormId).OnInit(m_UGFUIForm, formData.UserData);
                if (m_UGFUIForm.UIWidgets != null)
                {
                    foreach (UGFUIWidget uiWidget in m_UGFUIForm.UIWidgets)
                    {
                        UGFEventComponent.Instance.GetUIWidgetEvent(uiWidget.WidgetEventType).OnInit(uiWidget, formData.UserData);
                    }
                }
            }
            IsOpen = true;
            UGFEventComponent.Instance.GetUIFormEvent(m_UIFormId).OnOpen(m_UGFUIForm, formData.UserData);
            if (m_UGFUIForm.UIWidgets != null)
            {
                foreach (UGFUIWidget uiWidget in m_UGFUIForm.UIWidgets)
                {
                    UGFEventComponent.Instance.GetUIWidgetEvent(uiWidget.WidgetEventType).OnOpen(uiWidget, formData.UserData);
                }
            }
            formData.Release();
        }

        private void UGFUIFormDispose()
        {
            if (m_UGFUIForm != default && !m_UGFUIForm.IsDisposed)
            {
                UGFUIForm ugfUIForm = m_UGFUIForm;
                m_UGFUIForm = default;
                ugfUIForm.Dispose();
            }
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            if (m_UGFUIForm.UIWidgets != null)
            {
                foreach (UGFUIWidget uiWidget in m_UGFUIForm.UIWidgets)
                {
                    UGFEventComponent.Instance.GetUIWidgetEvent(uiWidget.WidgetEventType).OnClose(uiWidget, isShutdown, userData);
                }
            }
            UGFEventComponent.Instance.GetUIFormEvent(m_UIFormId).OnClose(m_UGFUIForm, isShutdown, userData);
            IsOpen = false;
            if (isShutdown)
            {
                UGFUIFormDispose();
            }
            base.OnClose(isShutdown, userData);
        }

        protected override void OnPause()
        {
            base.OnPause();
            UGFEventComponent.Instance.GetUIFormEvent(m_UIFormId).OnPause(m_UGFUIForm);
            if (m_UGFUIForm.UIWidgets != null)
            {
                foreach (UGFUIWidget uiWidget in m_UGFUIForm.UIWidgets)
                {
                    UGFEventComponent.Instance.GetUIWidgetEvent(uiWidget.WidgetEventType).OnPause(uiWidget);
                }
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            UGFEventComponent.Instance.GetUIFormEvent(m_UIFormId).OnResume(m_UGFUIForm);
            if (m_UGFUIForm.UIWidgets != null)
            {
                foreach (UGFUIWidget uiWidget in m_UGFUIForm.UIWidgets)
                {
                    UGFEventComponent.Instance.GetUIWidgetEvent(uiWidget.WidgetEventType).OnResume(uiWidget);
                }
            }
        }

        protected override void OnCover()
        {
            base.OnCover();
            UGFEventComponent.Instance.GetUIFormEvent(m_UIFormId).OnCover(m_UGFUIForm);
            if (m_UGFUIForm.UIWidgets != null)
            {
                foreach (UGFUIWidget uiWidget in m_UGFUIForm.UIWidgets)
                {
                    UGFEventComponent.Instance.GetUIWidgetEvent(uiWidget.WidgetEventType).OnCover(uiWidget);
                }
            }
        }

        protected override void OnReveal()
        {
            base.OnReveal();
            UGFEventComponent.Instance.GetUIFormEvent(m_UIFormId).OnReveal(m_UGFUIForm);
            if (m_UGFUIForm.UIWidgets != null)
            {
                foreach (UGFUIWidget uiWidget in m_UGFUIForm.UIWidgets)
                {
                    UGFEventComponent.Instance.GetUIWidgetEvent(uiWidget.WidgetEventType).OnReveal(uiWidget);
                }
            }
        }

        protected override void OnRefocus(object userData)
        {
            base.OnRefocus(userData);
            UGFEventComponent.Instance.GetUIFormEvent(m_UIFormId).OnRefocus(m_UGFUIForm, userData);
            if (m_UGFUIForm.UIWidgets != null)
            {
                foreach (UGFUIWidget uiWidget in m_UGFUIForm.UIWidgets)
                {
                    UGFEventComponent.Instance.GetUIWidgetEvent(uiWidget.WidgetEventType).OnRefocus(uiWidget, userData);
                }
            }
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            UGFEventComponent.Instance.GetUIFormEvent(m_UIFormId).OnUpdate(m_UGFUIForm, elapseSeconds, realElapseSeconds);
            if (m_UGFUIForm.UIWidgets != null)
            {
                foreach (UGFUIWidget uiWidget in m_UGFUIForm.UIWidgets)
                {
                    UGFEventComponent.Instance.GetUIWidgetEvent(uiWidget.WidgetEventType).OnUpdate(uiWidget, elapseSeconds, realElapseSeconds);
                }
            }
        }

        protected override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            base.OnDepthChanged(uiGroupDepth, depthInUIGroup);
            UGFEventComponent.Instance.GetUIFormEvent(m_UIFormId).OnDepthChanged(m_UGFUIForm, uiGroupDepth, depthInUIGroup);
            if (m_UGFUIForm.UIWidgets != null)
            {
                foreach (UGFUIWidget uiWidget in m_UGFUIForm.UIWidgets)
                {
                    UGFEventComponent.Instance.GetUIWidgetEvent(uiWidget.WidgetEventType).OnDepthChanged(uiWidget, uiGroupDepth, depthInUIGroup);
                }
            }
        }

        protected override void OnRecycle()
        {
            base.OnRecycle();
            UGFEventComponent.Instance.GetUIFormEvent(m_UIFormId).OnRecycle(m_UGFUIForm);
            if (m_UGFUIForm.UIWidgets != null)
            {
                foreach (UGFUIWidget uiWidget in m_UGFUIForm.UIWidgets)
                {
                    UGFEventComponent.Instance.GetUIWidgetEvent(uiWidget.WidgetEventType).OnRecycle(uiWidget);
                }
            }
            UGFUIFormDispose();
        }
    }
}
