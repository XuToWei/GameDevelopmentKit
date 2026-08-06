using Cysharp.Threading.Tasks;
using Game;

namespace ET.Client
{
    [EntitySystemOf(typeof(UIFormSurvivorLoginComponent))]
    public static partial class UIFormSurvivorLoginComponentSystem
    {
        [UGFUIFormSystem]
        private static void UGFUIFormOnOpen(this UIFormSurvivorLoginComponent self)
        {
            self.View.LoginButton.SetAsync(self.Login);
        }

        [UGFUIFormSystem]
        private static void UGFUIFormOnClose(this UIFormSurvivorLoginComponent self, bool isShutdown)
        {
        }

        private static UniTask Login(this UIFormSurvivorLoginComponent self)
        {
            return LoginHelper.Login(
                self.Root(),
                self.View.AccountInputField.text.Trim(),
                self.View.PasswordInputField.text);
        }
    }
}
