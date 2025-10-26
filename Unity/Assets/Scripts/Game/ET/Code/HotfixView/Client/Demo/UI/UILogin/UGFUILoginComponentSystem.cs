using CodeBind;
using Cysharp.Threading.Tasks;
using Game;
using UnityEngine;

namespace ET.Client
{
    [EntitySystemOf(typeof(UILoginComponent))]
    [FriendOf(typeof(UILoginComponent))]
    public static partial class UILoginComponentSystem
    {
        [UGFUIFormSystem]
        private static void UGFUIFormOnOpen(this UILoginComponent self)
        {
            self.Mono = (MonoUILogin)self.ETMono;
            self.Mono.LoginButton.SetAsync(self.OnLogin);
            self.Add
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
