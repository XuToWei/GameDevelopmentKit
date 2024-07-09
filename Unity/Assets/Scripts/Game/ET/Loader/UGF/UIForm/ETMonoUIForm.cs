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
            if (m_UGFUIForm.UIWidgets != null)
            {
                foreach (UGFUIWidget uiWidget in m_UGFUIForm.UIWidgets)
                {
                    UGFEventComponent.Instance.GetUIWidgetEvent(uiWidget.WidgetEventTypeLongHashCode).OnReload(uiWidget);
                }
            }
            UGFEventComponent.Instance.GetUIFormEvent(m_UIFormId).OnReload(m_UGFUIForm);
        }

        protected override void OnOpen(object userData)
        {
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
            }
            base.OnOpen(userData);
            IsOpen = true;
            UGFEventComponent.Instance.GetUIFormEvent(m_UIFormId).OnOpen(m_UGFUIForm, formData.UserData);
            formData.Release();
        }

        private void OnDestroy()
        {
            UGFUIFormDispose();
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
            IsOpen = false;
            if (m_UGFUIForm.UIWidgets != null)
            {
                foreach (UGFUIWidget uiWidget in m_UGFUIForm.UIWidgets)
                {
                    UGFEventComponent.Instance.GetUIWidgetEvent(uiWidget.WidgetEventTypeLongHashCode).OnClose(uiWidget, isShutdown, userData);
                }
            }
            UGFEventComponent.Instance.GetUIFormEvent(m_UIFormId).OnClose(m_UGFUIForm, isShutdown, userData);
            if (isShutdown)
            {
                UGFUIFormDispose();
            }
            base.OnClose(isShutdown, userData);
        }

        protected override void OnPause()
        {
            base.OnPause();
            if (m_UGFUIForm.UIWidgets != null)
            {
                foreach (UGFUIWidget uiWidget in m_UGFUIForm.UIWidgets)
                {
                    UGFEventComponent.Instance.GetUIWidgetEvent(uiWidget.WidgetEventTypeLongHashCode).OnPause(uiWidget);
                }
            }
            UGFEventComponent.Instance.GetUIFormEvent(m_UIFormId).OnPause(m_UGFUIForm);
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (m_UGFUIForm.UIWidgets != null)
            {
                foreach (UGFUIWidget uiWidget in m_UGFUIForm.UIWidgets)
                {
                    UGFEventComponent.Instance.GetUIWidgetEvent(uiWidget.WidgetEventTypeLongHashCode).OnResume(uiWidget);
                }
            }
            UGFEventComponent.Instance.GetUIFormEvent(m_UIFormId).OnResume(m_UGFUIForm);
        }

        protected override void OnCover()
        {
            base.OnCover();
            if (m_UGFUIForm.UIWidgets != null)
            {
                foreach (UGFUIWidget uiWidget in m_UGFUIForm.UIWidgets)
                {
                    UGFEventComponent.Instance.GetUIWidgetEvent(uiWidget.WidgetEventTypeLongHashCode).OnCover(uiWidget);
                }
            }
            UGFEventComponent.Instance.GetUIFormEvent(m_UIFormId).OnCover(m_UGFUIForm);
        }

        protected override void OnReveal()
        {
            base.OnReveal();
            if (m_UGFUIForm.UIWidgets != null)
            {
                foreach (UGFUIWidget uiWidget in m_UGFUIForm.UIWidgets)
                {
                    UGFEventComponent.Instance.GetUIWidgetEvent(uiWidget.WidgetEventTypeLongHashCode).OnReveal(uiWidget);
                }
            }
            UGFEventComponent.Instance.GetUIFormEvent(m_UIFormId).OnReveal(m_UGFUIForm);
        }

        protected override void OnRefocus(object userData)
        {
            base.OnRefocus(userData);
            if (m_UGFUIForm.UIWidgets != null)
            {
                foreach (UGFUIWidget uiWidget in m_UGFUIForm.UIWidgets)
                {
                    UGFEventComponent.Instance.GetUIWidgetEvent(uiWidget.WidgetEventTypeLongHashCode).OnRefocus(uiWidget, userData);
                }
            }
            UGFEventComponent.Instance.GetUIFormEvent(m_UIFormId).OnRefocus(m_UGFUIForm, userData);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            if (m_UGFUIForm.UIWidgets != null)
            {
                foreach (UGFUIWidget uiWidget in m_UGFUIForm.UIWidgets)
                {
                    UGFEventComponent.Instance.GetUIWidgetEvent(uiWidget.WidgetEventTypeLongHashCode).OnUpdate(uiWidget, elapseSeconds, realElapseSeconds);
                }
            }
            UGFEventComponent.Instance.GetUIFormEvent(m_UIFormId).OnUpdate(m_UGFUIForm, elapseSeconds, realElapseSeconds);
        }

        protected override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            base.OnDepthChanged(uiGroupDepth, depthInUIGroup);
            if (m_UGFUIForm.UIWidgets != null)
            {
                foreach (UGFUIWidget uiWidget in m_UGFUIForm.UIWidgets)
                {
                    UGFEventComponent.Instance.GetUIWidgetEvent(uiWidget.WidgetEventTypeLongHashCode).OnDepthChanged(uiWidget, uiGroupDepth, depthInUIGroup);
                }
            }
            UGFEventComponent.Instance.GetUIFormEvent(m_UIFormId).OnDepthChanged(m_UGFUIForm, uiGroupDepth, depthInUIGroup);
        }

        protected override void OnRecycle()
        {
            base.OnRecycle();
            if (m_UGFUIForm.UIWidgets != null)
            {
                foreach (UGFUIWidget uiWidget in m_UGFUIForm.UIWidgets)
                {
                    UGFEventComponent.Instance.GetUIWidgetEvent(uiWidget.WidgetEventTypeLongHashCode).OnRecycle(uiWidget);
                }
            }
            UGFEventComponent.Instance.GetUIFormEvent(m_UIFormId).OnRecycle(m_UGFUIForm);
        }
    }
}
