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
            var uiWidget = self.AddComponentUIWidget<UIWidgetTest>(self.View.TestWidgetTest);
            uiWidget.Open();
        }

        [UGFUIFormSystem]
        private static void UGFUIFormOnClose(this UIFormLoginComponent self)
        {
            
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
