using UGF;

namespace ET.Client
{
    [UIEvent(UIFormId.Main)]
    [FriendOf(typeof(UIMainComponent))]
    public class UIMainEvent: AUIEvent
    {
        public override void OnInit(UI ui, object userData)
        {
            base.OnInit(ui, userData);
            UIMainView uiView = ui.UIForm.GetComponent<UIMainView>();
            UIMainComponent uiMainComponent = ui.AddComponent<UIMainComponent, UIMainView>(uiView);
        }
        
        public override void OnOpen(UI ui, object userData)
        {
            base.OnOpen(ui, userData);
            
        }
    }
}
