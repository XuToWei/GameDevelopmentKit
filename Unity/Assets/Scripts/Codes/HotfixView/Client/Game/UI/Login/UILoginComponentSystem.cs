using UnityEngine.UI;

namespace ET.Client
{
    [FriendOf(typeof (UILoginComponent))]
    public static class UILoginComponentSystem
    {
        public class UILoginComponentAwakeSystem : AwakeSystem<UILoginComponent, LoginForm>
        {
             protected override void Awake(UILoginComponent self, LoginForm uiForm)
             {
                self.Form = uiForm;
             }
        }
        
        public class UILoginComponentDestroySystem : DestroySystem<UILoginComponent>
        {
            protected override void Destroy(UILoginComponent self)
            {
                self.Form = null;
            }
        }

        public static async ETTask OnLogin(this UILoginComponent self)
        {
            string account = self.Form.AccountTMPInputField.text;
            string password = self.Form.PasswordTMPInputField.text;
            int? r = AccountHelper.CheckAccountPassword(account, password);
            if (r.HasValue)
            {
                return;
            }
            await LoginHelper.Login(self.ClientScene(), account, password);
        }
    }
}
