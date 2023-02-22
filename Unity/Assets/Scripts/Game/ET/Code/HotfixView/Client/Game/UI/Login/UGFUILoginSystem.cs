using UnityEngine;

namespace ET.Client
{
    [FriendOf(typeof (UGFUILogin))]
    public static class UGFUILoginSystem
    {
        public class UGFUILoginAwakeSystem : AwakeSystem<UGFUILogin, Transform>
        {
             protected override void Awake(UGFUILogin self, Transform uiTransform)
             {
                self.transform = uiTransform;
             }
        }
        
        public class UGFUILoginDestroySystem : DestroySystem<UGFUILogin>
        {
            protected override void Destroy(UGFUILogin self)
            {
                self.transform = null;
            }
        }
    }
}
