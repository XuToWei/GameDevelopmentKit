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
            self.View.LoginButton.SetAsync(self.OnLogin);
            self.View.LoginButton.SetAsync(self.OnLogin);
            self.AddComponentUIWidget<UIWidgetTest>(self.View.TestWidgetTest);
            self.GetComponent<UIWidgetTest>().Open();
        }

        public static UniTask OnLogin(this UIFormLoginComponent self)
        {
            return LoginHelper.Login(
                self.Root(),
                self.View.AccountInputField.text,
                self.View.PasswordInputField.text);
        }
    }
}
