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
            MainForm MainForm = ui.UIForm.Logic as MainForm;
            UIMainComponent uiMainComponent = ui.AddComponent<UIMainComponent, MainForm>(MainForm);
        }
        
        public override void OnOpen(UI ui, object userData)
        {
            base.OnOpen(ui, userData);
            
        }
    }
}
