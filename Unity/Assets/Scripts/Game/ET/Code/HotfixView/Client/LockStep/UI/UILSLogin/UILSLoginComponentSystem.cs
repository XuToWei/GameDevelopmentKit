using Cysharp.Threading.Tasks;
using Game;

namespace ET.Client
{
    [EntitySystemOf(typeof(UILSLoginComponent))]
    [FriendOf(typeof(UILSLoginComponent))]
    public static partial class UILSLoginComponentSystem
    {
        [UGFUIFormSystem]
        private static void UGFUIFormOnOpen(this UILSLoginComponent self)
        {
            self.Mono.LoginButton.Set(self.OnLogin);
        }

        public static void OnLogin(this UILSLoginComponent self)
        {
            LoginHelper.Login(
                self.Root(), 
                self.Mono.AccountInputField.text, 
                self.Mono.PasswordInputField.text).Forget();
        }
    }
}
