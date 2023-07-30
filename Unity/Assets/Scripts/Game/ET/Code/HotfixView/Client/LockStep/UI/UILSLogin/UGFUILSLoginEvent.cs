using Game;
using UnityEngine;

namespace ET.Client
{
    [FriendOf(typeof(UGFUILSLogin))]
    public static partial class UGFUILSLoginSystem
    {
        [UGFUIFormEvent(UGFUIFormId.UILSLogin)]
        private class UGFUILSLoginEvent : AUGFUIFormEvent
        {
            public override void OnInit(UGFUIForm uiForm, object userData)
            {
                base.OnInit(uiForm, userData);
                UGFUILSLogin uiLSLogin = uiForm.AddComponent<UGFUILSLogin, Transform>(uiForm.transform);
                uiLSLogin.loginButton.Set(uiLSLogin.OnLogin);
            }
            
            public override void OnOpen(UGFUIForm uiForm, object userData)
            {
                base.OnOpen(uiForm, userData);
                
            }
        }
    }
}
