using CodeBind;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ET.Client
{
    [FriendOf(typeof (UGFUILSLogin))]
    public static partial class UGFUILSLoginSystem
    {
        [EntitySystem]
        private class UGFUILSLoginAwakeSystem : AwakeSystem<UGFUILSLogin, Transform>
        {
             protected override void Awake(UGFUILSLogin self, Transform uiTransform)
             {
                self.InitBind(uiTransform);
             }
        }
        
        [EntitySystem]
        private class UGFUILSLoginDestroySystem : DestroySystem<UGFUILSLogin>
        {
            protected override void Destroy(UGFUILSLogin self)
            {
                self.ClearBind();
            }
        }
        
        public static void OnLogin(this UGFUILSLogin self)
        {
            LoginHelper.Login(
                self.DomainScene(), 
                self.accountInputField.text, 
                self.passwordInputField.text).Forget();
        }
    }
}
