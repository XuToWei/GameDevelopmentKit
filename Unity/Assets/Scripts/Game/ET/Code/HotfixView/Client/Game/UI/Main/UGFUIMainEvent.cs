using UnityEngine;

namespace ET.Client
{
    [UGFUIFormEvent(UGFUIFormId.Main)]
    [FriendOf(typeof(UGFUIMain))]
    public class UGFUIMainEvent: AUGFUIFormEvent
    {
        public override void OnInit(UGFUIForm uiForm, object userData)
        {
            base.OnInit(uiForm, userData);
            UGFUIMain uiMain = uiForm.AddComponent<UGFUIMain, Transform>(uiForm.transform);
        }
        
        public override void OnOpen(UGFUIForm uiForm, object userData)
        {
            base.OnOpen(uiForm, userData);
            
        }
    }
}
