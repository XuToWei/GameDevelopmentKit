using Game;
using UnityEngine;

namespace ET.Client
{
    [FriendOf(typeof(UGFUILSLobby))]
    public static partial class UGFUILSLobbySystem
    {
        [UGFUIFormEvent(UGFUIFormId.UILSLobby)]
        public class UGFUILSLobbyEvent : AUGFUIFormEvent
        {
            public override void OnInit(UGFUIForm uiForm, object userData)
            {
                base.OnInit(uiForm, userData);
                UGFUILSLobby uiLSLobby = uiForm.AddComponent<UGFUILSLobby, Transform>(uiForm.transform);
                uiLSLobby.enterMapButton.SetAsync(uiLSLobby.EnterMap);
                uiLSLobby.replayButton.Set(uiLSLobby.Replay);
            }

            public override void OnOpen(UGFUIForm uiForm, object userData)
            {
                base.OnOpen(uiForm, userData);
            }
        }
    }
}