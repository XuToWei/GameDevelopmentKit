using CodeBind;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ET.Client
{
    [FriendOf(typeof (UGFUILogin))]
    public static partial class UGFUILoginSystem
    {
        public class UGFUILoginAwakeSystem : AwakeSystem<UGFUILogin, Transform>
        {
             protected override void Awake(UGFUILogin self, Transform uiTransform)
             {
                self.InitBind(uiTransform);
             }
        }
        
        public class UGFUILoginDestroySystem : DestroySystem<UGFUILogin>
        {
            protected override void Destroy(UGFUILogin self)
            {
                self.ClearBind();
            }
        }
        
        public static void OnLogin(this UGFUILogin self)
        {
            LoginHelper.Login(
                self.DomainScene(), 
                self.AccountInputField.text, 
                self.PasswordInputField.text).Forget();
        }
    }
}
