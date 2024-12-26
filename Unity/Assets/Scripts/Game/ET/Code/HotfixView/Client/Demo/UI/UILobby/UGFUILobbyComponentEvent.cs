using Game;
using UnityEngine;

namespace ET.Client
{
    [FriendOf(typeof(UGFUILobbyComponent))]
    public static partial class UGFUILobbyComponentSystem
    {
        [UGFUIFormEvent(UGFUIFormId.UILobby)]
        private class UGFUILobbyComponentEvent : AUGFUIFormEvent
        {
            public override void OnInit(UGFUIForm uiForm, object userData)
            {
                base.OnInit(uiForm, userData);
                var uiLobby = uiForm.AddComponent<UGFUILobbyComponent, Transform>(uiForm.Transform);
                uiLobby.EnterMapButton.SetAsync(uiLobby.EnterMap);
            }

            public override void OnOpen(UGFUIForm uiForm, object userData)
            {
                base.OnOpen(uiForm, userData);
                
            }
        }
    }
}
