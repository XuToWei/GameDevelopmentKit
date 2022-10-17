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
            MatchRoomForm MatchRoomForm = ui.UIForm.Logic as MatchRoomForm;
            UIMatchRoomComponent uiMatchRoomComponent = ui.AddComponent<UIMatchRoomComponent, MatchRoomForm>(MatchRoomForm);
        }
        
        public override void OnOpen(UI ui, object userData)
        {
            base.OnOpen(ui, userData);
            
        }
    }
}
