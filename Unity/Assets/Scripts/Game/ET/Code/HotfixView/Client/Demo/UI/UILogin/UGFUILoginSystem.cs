using CodeBind;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ET.Client
{
    [FriendOf(typeof (UGFUILogin))]
    public static partial class UGFUILoginSystem
    {
        [EntitySystem]
        private class UGFUILoginAwakeSystem : AwakeSystem<UGFUILogin, Transform>
        {
             protected override void Awake(UGFUILogin self, Transform uiTransform)
             {
                self.InitBind(uiTransform);
             }
        }
        
        [EntitySystem]
        private class UGFUILoginDestroySystem : DestroySystem<UGFUILogin>
        {
            protected override void Destroy(UGFUILogin self)
            {
                self.ClearBind();
            }
        }
        
        public static UniTask OnLogin(this UGFUILogin self)
        {
            return LoginHelper.Login(
                self.DomainScene(),
                self.accountInputField.text,
                self.passwordInputField.text);
        }
    }
}
