using System;
using System.Collections.Generic;
using ET;
using ET.Client;
using GameFramework;
using UnityGameFramework.Runtime;
using UIComponent = ET.Client.UIComponent;

namespace UGF
{
    public abstract class ETUIForm: UGuiForm
    {
        public UIFormId UIFormId { get; private set; }

        public UI UI { get; private set; }

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            ETUIFormData formData = userData as ETUIFormData;
            this.UIFormId = formData.UIFormId;
            this.UI = formData.UIComponent.AddChild<UI, UIFormId, UIForm>(this.UIFormId, this.UIForm);
            UIEventComponent.Instance.UIEvents[this.UIFormId].OnInit(this.UI, formData.UserData);
        }

        protected override void OnRecycle()
        {
            base.OnRecycle();
            UIEventComponent.Instance.UIEvents[this.UIFormId].OnRecycle(this.UI);
        }

        private void OnDestroy()
        {
            this.UI.Dispose();
            this.UI = null;
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            ETUIFormData formData = userData as ETUIFormData;
            UIEventComponent.Instance.UIEvents[this.UIFormId].OnOpen(this.UI, formData.UserData);
            ReferencePool.Release(formData);
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            UIEventComponent.Instance.UIEvents[this.UIFormId].OnClose(this.UI, isShutdown, userData);
            base.OnClose(isShutdown, userData);
        }

        protected override void OnPause()
        {
            base.OnPause();
            UIEventComponent.Instance.UIEvents[this.UIFormId].OnPause(this.UI);
        }
        
        protected override void OnResume()
        {
            base.OnResume();
            UIEventComponent.Instance.UIEvents[this.UIFormId].OnResume(this.UI);
        }
        
        protected override void OnCover()
        {
            base.OnCover();
            UIEventComponent.Instance.UIEvents[this.UIFormId].OnCover(this.UI);
        }
        
        protected override void OnReveal()
        {
            base.OnReveal();
            UIEventComponent.Instance.UIEvents[this.UIFormId].OnReveal(this.UI);
        }
        
        protected override void OnRefocus(object userData)
        {
            base.OnRefocus(userData);
            UIEventComponent.Instance.UIEvents[this.UIFormId].OnRefocus(this.UI, userData);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            UIEventComponent.Instance.UIEvents[this.UIFormId].OnUpdate(this.UI, elapseSeconds, realElapseSeconds);
        }

        protected override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            base.OnDepthChanged(uiGroupDepth, depthInUIGroup);
            UIEventComponent.Instance.UIEvents[this.UIFormId].OnDepthChanged(this.UI, uiGroupDepth, depthInUIGroup);
        }
    }
}