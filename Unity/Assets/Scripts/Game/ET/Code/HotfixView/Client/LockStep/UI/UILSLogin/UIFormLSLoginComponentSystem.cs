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
            self.Mono.LoginButton.Set(self.OnLogin);
        }

        public static void OnLogin(this UIFormLSLoginComponent self)
        {
            LoginHelper.Login(
                self.Root(), 
                self.Mono.AccountInputField.text, 
                self.Mono.PasswordInputField.text).Forget();
        }
    }
}
