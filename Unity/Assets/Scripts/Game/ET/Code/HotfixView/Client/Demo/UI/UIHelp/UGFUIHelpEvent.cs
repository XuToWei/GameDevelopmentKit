using UnityEngine;

namespace ET.Client
{
    [FriendOf(typeof(UGFUIHelp))]
    public static partial class UGFUIHelpSystem
    {
        [UGFUIFormEvent(UGFUIFormId.UIHelp)]
        public class UGFUIHelpEvent : AUGFUIFormEvent
        {
            public override void OnInit(UGFUIForm uiForm, object userData)
            {
                base.OnInit(uiForm, userData);
                UGFUIHelp uiHelp = uiForm.AddComponent<UGFUIHelp, Transform>(uiForm.transform);
            }
            
            public override void OnOpen(UGFUIForm uiForm, object userData)
            {
                base.OnOpen(uiForm, userData);
                
            }
        }
    }
}
