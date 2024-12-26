using CodeBind;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ET.Client
{
    [EntitySystemOf(typeof(UGFUILSLoginComponent))]
    [FriendOf(typeof(UGFUILSLoginComponent))]
    public static partial class UGFUILSLoginComponentSystem
    {
        [EntitySystem]
        private static void Awake(this UGFUILSLoginComponent self, Transform uiTransform)
        {
            self.InitBind(uiTransform);
        }

        [EntitySystem]
        private static void Destroy(this UGFUILSLoginComponent self)
        {
            self.ClearBind();
        }

        public static void OnLogin(this UGFUILSLoginComponent self)
        {
            LoginHelper.Login(
                self.Root(), 
                self.AccountInputField.text, 
                self.PasswordInputField.text).Forget();
        }
    }
}
