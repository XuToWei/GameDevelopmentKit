using Cysharp.Threading.Tasks;
using Game;

namespace ET.Client
{
    [EntitySystemOf(typeof(UIFormLoginComponent))]
    [FriendOf(typeof(UIFormLoginComponent))]
    public static partial class UIFormLoginComponentSystem
    {
        [UGFUIFormSystem]
        private static void UGFUIFormOnOpen(this UIFormLoginComponent self)
        {
            self.Mono.LoginButton.SetAsync(self.OnLogin);
            self.Mono.LoginButton.SetAsync(self.OnLogin);
            self.AddComponentUIWidget<UIWidgetTest>(self.Mono.TestWidgetTest);
            self.GetComponent<UIWidgetTest>().Open();
        }

        public static UniTask OnLogin(this UIFormLoginComponent self)
        {
            return LoginHelper.Login(
                self.Root(),
                self.Mono.AccountInputField.text,
                self.Mono.PasswordInputField.text);
        }
    }
}
