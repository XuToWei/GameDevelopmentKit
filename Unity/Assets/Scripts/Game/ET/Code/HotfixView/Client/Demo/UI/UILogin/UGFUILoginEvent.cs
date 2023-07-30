using Game;
using UnityEngine;

namespace ET.Client
{
    [FriendOf(typeof(UGFUILogin))]
    public static partial class UGFUILoginSystem
    {
        [UGFUIFormEvent(UGFUIFormId.UILogin)]
        public class UGFUILoginEvent : AUGFUIFormEvent
        {
            public override void OnInit(UGFUIForm uiForm, object userData)
            {
                base.OnInit(uiForm, userData);
                UGFUILogin uiLogin = uiForm.AddComponent<UGFUILogin, Transform>(uiForm.transform);
                uiLogin.loginButton.SetAsync(uiLogin.OnLogin);
            }
            
            public override void OnOpen(UGFUIForm uiForm, object userData)
            {
                base.OnOpen(uiForm, userData);
                
            }
        }
    }
}
