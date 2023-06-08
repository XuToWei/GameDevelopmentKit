using CodeBind;
using UnityEngine;

namespace ET.Client
{
    [FriendOf(typeof (UGFUILSRoom))]
    public static partial class UGFUILSRoomSystem
    {
        [EntitySystem]
        private class UGFUILSRoomAwakeSystem : AwakeSystem<UGFUILSRoom, Transform>
        {
             protected override void Awake(UGFUILSRoom self, Transform uiTransform)
             {
                self.InitBind(uiTransform);
             }
        }
        
        [EntitySystem]
        private class UGFUILSRoomDestroySystem : DestroySystem<UGFUILSRoom>
        {
            protected override void Destroy(UGFUILSRoom self)
            {
                self.ClearBind();
            }
        }
        
        private static void JumpReplay(this UGFUILSRoom self)
        {
            int toFrame = int.Parse(self.jumpToCountInputField.text);
            LSClientHelper.JumpReplay(self.Room(), toFrame);
        }
        
        private static void OnReplaySpeedClicked(this UGFUILSRoom self)
        {
            LSReplayUpdater lsReplayUpdater = self.Room().GetComponent<LSReplayUpdater>();
            lsReplayUpdater.ChangeReplaySpeed();
            self.speedText.text = $"X{lsReplayUpdater.ReplaySpeed}";
        }
        
        private static void OnSaveReplay(this UGFUILSRoom self)
        {
            string name = self.saveNameInputField.text;

            LSClientHelper.SaveReplay(self.Room(), name);
        }
    }
}
