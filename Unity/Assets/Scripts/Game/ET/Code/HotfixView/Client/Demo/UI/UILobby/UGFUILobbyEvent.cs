using Cysharp.Threading.Tasks;
using Game;
using UnityEngine;

namespace ET.Client
{
    [FriendOf(typeof (UGFUILobby))]
    public static partial class UGFUILobbySystem
    {
        [UGFUIFormEvent(UGFUIFormId.UILobby)]
        public class UGFUILobbyEvent: AUGFUIFormEvent
        {
            public override void OnInit(UGFUIForm uiForm, object userData)
            {
                base.OnInit(uiForm, userData);
                UGFUILobby uiLobby = uiForm.AddComponent<UGFUILobby, Transform>(uiForm.transform);
                uiLobby.EnterMapButton.onClick.Set(() => { uiLobby.EnterMap().Forget(); });
            }
            
            public override void OnOpen(UGFUIForm uiForm, object userData)
            {
                base.OnOpen(uiForm, userData);
                
            }
        }
    }
}
