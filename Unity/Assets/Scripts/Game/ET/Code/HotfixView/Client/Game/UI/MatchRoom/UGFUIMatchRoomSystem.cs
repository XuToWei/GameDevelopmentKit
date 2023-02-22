using UnityEngine;

namespace ET.Client
{
    [FriendOf(typeof (UGFUIMatchRoom))]
    public static class UGFUIMatchRoomSystem
    {
        public class UGFUIMatchRoomAwakeSystem : AwakeSystem<UGFUIMatchRoom, Transform>
        {
             protected override void Awake(UGFUIMatchRoom self, Transform uiTransform)
             {
                self.transform = uiTransform;
             }
        }
        
        public class UGFUIMatchRoomDestroySystem : DestroySystem<UGFUIMatchRoom>
        {
            protected override void Destroy(UGFUIMatchRoom self)
            {
                self.transform = null;
            }
        }
    }
}
