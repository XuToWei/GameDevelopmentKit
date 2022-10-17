using UGF;
using UnityEngine.UI;

namespace ET.Client
{
    [UIEvent(UIFormId.Login)]
    [FriendOf(typeof(UILoginComponent))]
    public class UILoginEvent: AUIEvent
    {
        public override void OnInit(UI ui, object userData)
        {
            base.OnInit(ui, userData);
            LoginForm uiForm = ui.UIForm.Logic as LoginForm;
            UILoginComponent uiLoginComponent = ui.AddComponent<UILoginComponent, LoginForm>(uiForm);
            uiForm.LoginButton.Set(uiLoginComponent.OnLogin);
        }
        
        public override void OnOpen(UI ui, object userData)
        {
            base.OnOpen(ui, userData);
            
        }
    }
}
