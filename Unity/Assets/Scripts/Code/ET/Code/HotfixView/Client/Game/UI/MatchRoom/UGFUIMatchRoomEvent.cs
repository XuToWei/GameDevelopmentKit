using UnityEngine;

namespace ET.Client
{
    [UGFUIFormEvent(UGFUIFormId.MatchRoom)]
    [FriendOf(typeof(UGFUIMatchRoom))]
    public class UGFUIMatchRoomEvent: AUGFUIFormEvent
    {
        public override void OnInit(UGFUIForm uiForm, object userData)
        {
            base.OnInit(uiForm, userData);
            UGFUIMatchRoom uiMatchRoom = uiForm.AddComponent<UGFUIMatchRoom, Transform>(uiForm.transform);
        }
        
        public override void OnOpen(UGFUIForm uiForm, object userData)
        {
            base.OnOpen(uiForm, userData);
            
        }
    }
}
