using Cysharp.Threading.Tasks;
using Game;

namespace ET.Client
{
    [EntitySystemOf(typeof(UIFormLoginComponent))]
    [FriendOf(typeof(UIFormLoginComponent))]
    public static partial class UIFormLoginComponentSystem
    {
        [UGFUIFormSystem]
        private static void UGFUIFormOnInit(this UIFormLoginComponent self)
        {
            Log.Debug("Login界面OnInit");
        }

        [UGFUIFormSystem]
        private static void UGFUIFormOnOpen(this UIFormLoginComponent self)
        {
            self.View.LoginButton.SetAsync(self.OnLogin);
            var uiWidget = self.AddComponentUIWidget<UIWidgetTest>(self.View.TestWidgetTest);
            uiWidget.Open();
            Log.Debug("Login界面OnOpen");
        }

        [UGFUIFormSystem]
        private static void UGFUIFormOnClose(this UIFormLoginComponent self, bool isShutdown)
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
