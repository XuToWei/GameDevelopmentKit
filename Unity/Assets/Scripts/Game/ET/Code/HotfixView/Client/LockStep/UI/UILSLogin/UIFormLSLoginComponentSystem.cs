using Cysharp.Threading.Tasks;
using Game;

namespace ET.Client
{
    [EntitySystemOf(typeof(UIFormLSLoginComponent))]
    [FriendOf(typeof(UIFormLSLoginComponent))]
    public static partial class UIFormLSLoginComponentSystem
    {
        [UGFUIFormSystem]
        private static void UGFUIFormOnOpen(this UIFormLSLoginComponent self)
        {
            self.View.loginButton.Set(self.OnLogin);
        }

        public static void OnLogin(this UIFormLSLoginComponent self)
        {
            LoginHelper.Login(
                self.Root(), 
                self.View.accountInputField.text, 
                self.View.passwordInputField.text).Forget();
        }
    }
}
