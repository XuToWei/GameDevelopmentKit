using UnityEngine;

namespace ET.Client
{
    [FriendOf(typeof (UGFUIMain))]
    public static class UGFUIMainSystem
    {
        public class UGFUIMainAwakeSystem : AwakeSystem<UGFUIMain, Transform>
        {
             protected override void Awake(UGFUIMain self, Transform uiTransform)
             {
                self.transform = uiTransform;
             }
        }
        
        public class UGFUIMainDestroySystem : DestroySystem<UGFUIMain>
        {
            protected override void Destroy(UGFUIMain self)
            {
                self.transform = null;
            }
        }
    }
}
