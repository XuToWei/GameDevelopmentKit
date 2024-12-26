using CodeBind;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ET.Client
{
    [EntitySystemOf(typeof(UGFUILoginComponent))]
    [FriendOf(typeof(UGFUILoginComponent))]
    public static partial class UGFUILoginComponentSystem
    {
        [EntitySystem]
        private static void Awake(this UGFUILoginComponent self, Transform uiTransform)
        {
            self.InitBind(uiTransform);
        }

        [EntitySystem]
        private static void Destroy(this UGFUILoginComponent self)
        {
            self.ClearBind();
        }

        public static UniTask OnLogin(this UGFUILoginComponent self)
        {
            return LoginHelper.Login(
                self.Root(),
                self.AccountInputField.text,
                self.PasswordInputField.text);
        }
    }
}
