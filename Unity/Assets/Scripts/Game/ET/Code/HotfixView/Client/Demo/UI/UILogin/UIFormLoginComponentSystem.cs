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
        private static void UGFUIFormOnOpen(this UIFormLoginComponent self)
        {
            self.OpenAllUIWidgets();
            self.View.loginButton.SetAsync(self.OnLogin);
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
                self.View.accountInputField.text,
                self.View.passwordInputField.text);
        }

        private static async UniTaskVoid LoadTest1(this UIFormLoginComponent self)
        {
            var uiWidget = await self.LoadChildUIWidgetAsync<UIWidgetTest>(UGFUIEntityId.WidgetTest);
            uiWidget.CachedRectTransform.SetParent(self.View.test1RectTransform);
            uiWidget.CachedRectTransform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            uiWidget.CachedRectTransform.localScale = Vector3.one;
            uiWidget.Open();
        }

        private static async UniTaskVoid LoadTest2(this UIFormLoginComponent self)
        {
            var uiWidget = await self.LoadChildUIWidgetAsync<UIWidgetTest>(UGFUIEntityId.WidgetTest);
            uiWidget.CachedRectTransform.SetParent(self.View.test2RectTransform);
            uiWidget.CachedRectTransform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            uiWidget.CachedRectTransform.localScale = Vector3.one;
            uiWidget.Open();
        }
    }
}
