using Cysharp.Threading.Tasks;
using Game;
using UnityEngine;

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
            self.LoadTest1().Forget();
            self.LoadTest2().Forget();
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

        private static async UniTaskVoid LoadTest1(this UIFormLoginComponent self)
        {
            var uiWidget = await self.LoadChildUIWidgetAsync<UIWidgetTest>(UGFUIEntityId.WidgetTest);
            uiWidget.CachedTransform.SetParent(self.View.Test1RectTransform);
            uiWidget.CachedTransform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            uiWidget.CachedTransform.localScale = Vector3.one;
            uiWidget.Open();
        }

        private static async UniTaskVoid LoadTest2(this UIFormLoginComponent self)
        {
            var uiWidget = await self.LoadChildUIWidgetAsync<UIWidgetTest>(UGFUIEntityId.WidgetTest);
            uiWidget.CachedTransform.SetParent(self.View.Test2RectTransform);
            uiWidget.CachedTransform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            uiWidget.CachedTransform.localScale = Vector3.one;
        }
    }
}
