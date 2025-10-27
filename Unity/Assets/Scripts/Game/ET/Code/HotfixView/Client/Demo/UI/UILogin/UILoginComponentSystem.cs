using Cysharp.Threading.Tasks;
using Game;

namespace ET.Client
{
    [EntitySystemOf(typeof(UILoginComponent))]
    [FriendOf(typeof(UILoginComponent))]
    public static partial class UILoginComponentSystem
    {
        [UGFUIFormSystem]
        private static void UGFUIFormOnInit(this UILoginComponent self)
        {
            self.Mono.LoginButton.SetAsync(self.OnLogin);
            self.AddComponentUIWidget<UIWidgetTest>(self.Mono.TestWidgetTest);
        }
        
        [UGFUIFormSystem]
        private static void UGFUIFormOnOpen(this UILoginComponent self)
        {
            self.Mono.LoginButton.SetAsync(self.OnLogin);
            self.GetComponent<UIWidgetTest>().Open();
        }

        public static UniTask OnLogin(this UILoginComponent self)
        {
            return LoginHelper.Login(
                self.Root(),
                self.Mono.AccountInputField.text,
                self.Mono.PasswordInputField.text);
        }
    }
}
