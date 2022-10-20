using UGF;

namespace ET.Client
{
    [UIEvent(UIFormId.MatchRoom)]
    [FriendOf(typeof(UIMatchRoomComponent))]
    public class UIMatchRoomEvent: AUIEvent
    {
        public override void OnInit(UI ui, object userData)
        {
            base.OnInit(ui, userData);
            UIMatchRoomView uiView = ui.UIForm.GetComponent<UIMatchRoomView>();
            UIMatchRoomComponent uiMatchRoomComponent = ui.AddComponent<UIMatchRoomComponent, UIMatchRoomView>(uiView);
        }
        
        public override void OnOpen(UI ui, object userData)
        {
            base.OnOpen(ui, userData);
            
        }
    }
}
