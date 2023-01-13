using UnityEngine;

namespace ET.Client
{
    [UGFUIFormEvent(UGFUIFormId.Login)]
    [FriendOf(typeof(UGFUILogin))]
    public class UGFUILoginEvent: AUGFUIFormEvent
    {
        public override void OnInit(UGFUIForm uiForm, object userData)
        {
            base.OnInit(uiForm, userData);
            UGFUILogin uiLogin = uiForm.AddComponent<UGFUILogin, Transform>(uiForm.transform);
        }
        
        public override void OnOpen(UGFUIForm uiForm, object userData)
        {
            base.OnOpen(uiForm, userData);
            
        }
    }
}
