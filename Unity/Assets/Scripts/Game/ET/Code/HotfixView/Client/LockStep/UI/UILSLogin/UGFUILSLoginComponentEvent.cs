using Game;
using UnityEngine;

namespace ET.Client
{
    [FriendOf(typeof(UGFUILSLoginComponent))]
    public static partial class UGFUILSLoginComponentSystem
    {
        [UGFUIFormEvent(UGFUIFormId.UILSLogin)]
        private class UGFUILSLoginComponentEvent : AUGFUIFormEvent
        {
            public override void OnInit(UGFUIForm uiForm, object userData)
            {
                base.OnInit(uiForm, userData);
                UGFUILSLoginComponent uiLSLogin = uiForm.AddComponent<UGFUILSLoginComponent, Transform>(uiForm.Transform);
                uiLSLogin.LoginButton.Set(uiLSLogin.OnLogin);
            }

            public override void OnOpen(UGFUIForm uiForm, object userData)
            {
                base.OnOpen(uiForm, userData);
                
            }
        }
    }
}
