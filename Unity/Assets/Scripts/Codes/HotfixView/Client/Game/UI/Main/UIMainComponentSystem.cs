namespace ET.Client
{
    [FriendOf(typeof (UIMainComponent))]
    public static class UIMainComponentSystem
    {
        public class UIMainComponentAwakeSystem : AwakeSystem<UIMainComponent, MainForm>
        {
             protected override void Awake(UIMainComponent self, MainForm uiForm)
             {
                self.Form = uiForm as MainForm;
             }
        }
        
        public class UIMainComponentDestroySystem : DestroySystem<UIMainComponent>
        {
            protected override void Destroy(UIMainComponent self)
            {
                self.Form = null;
            }
        }
    }
}
