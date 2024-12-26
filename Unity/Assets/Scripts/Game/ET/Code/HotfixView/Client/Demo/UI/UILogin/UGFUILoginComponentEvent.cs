using Game;
using UnityEngine;

namespace ET.Client
{
    [FriendOf(typeof(UGFUILoginComponent))]
    public static partial class UGFUILoginComponentSystem
    {
        [UGFUIFormEvent(UGFUIFormId.UILogin)]
        public class UGFUILoginComponentEvent : AUGFUIFormEvent
        {
            public override void OnInit(UGFUIForm uiForm, object userData)
            {
                base.OnInit(uiForm, userData);
                var uiLogin = uiForm.AddComponent<UGFUILoginComponent, Transform>(uiForm.Transform);
                uiLogin.LoginButton.SetAsync(uiLogin.OnLogin);
                uiLogin.uiWidgetTest = uiForm.AddUIWidget<UGFUIWidgetTestEvent>(uiLogin.TestWidgetTransform);
                
            }

            public override void OnOpen(UGFUIForm uiForm, object userData)
            {
                base.OnOpen(uiForm, userData);
                var uiLogin = uiForm.GetComponent<UGFUILoginComponent>();
                uiForm.OpenUIWidget(uiLogin.uiWidgetTest);
            }
        }
    }
}
