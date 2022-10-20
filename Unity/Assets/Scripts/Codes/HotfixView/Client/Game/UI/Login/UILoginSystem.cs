namespace ET.Client
{
    [FriendOf(typeof (UILoginComponent))]
    public static class UILoginSystem
    {
        public class UILoginAwakeSystem : AwakeSystem<UILoginComponent, UILoginView>
        {
             protected override void Awake(UILoginComponent self, UILoginView uiView)
             {
                self.View = uiView;
             }
        }
        
        public class UILoginDestroySystem : DestroySystem<UILoginComponent>
        {
            protected override void Destroy(UILoginComponent self)
            {
                self.View = null;
            }
        }
        
        public static async ETTask OnLogin(this UILoginComponent self)
        {
            string account = self.View.AccountTMPInputField.text;
            string password = self.View.PasswordTMPInputField.text;
            int? r = AccountHelper.CheckAccountPassword(account, password);
            if (r.HasValue)
            {
                return;
            }
            await LoginHelper.Login(self.ClientScene(), account, password);
        }
    }
}
