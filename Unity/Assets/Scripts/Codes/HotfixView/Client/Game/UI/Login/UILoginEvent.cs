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
            UILoginView uiView = ui.UIForm.GetComponent<UILoginView>();
            UILoginComponent uiLoginComponent = ui.AddComponent<UILoginComponent, UILoginView>(uiView);
            uiView.LoginButton.Set(uiLoginComponent.OnLogin);
        }
        
        public override void OnOpen(UI ui, object userData)
        {
            base.OnOpen(ui, userData);
            
        }
    }
}
