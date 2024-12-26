using CodeBind;
using UnityEngine;

namespace ET.Client
{
    [EntitySystemOf(typeof(UGFUILSRoomComponent))]
    [FriendOf(typeof(UGFUILSRoomComponent))]
    public static partial class UGFUILSRoomComponentSystem
    {
        [EntitySystem]
        private static void Awake(this UGFUILSRoomComponent self, Transform uiTransform)
        {
            self.InitBind(uiTransform);
        }

        [EntitySystem]
        private static void Destroy(this UGFUILSRoomComponent self)
        {
            self.ClearBind();
        }

        private static void JumpReplay(this UGFUILSRoomComponent self)
        {
            int toFrame = int.Parse(self.JumpToCountInputField.text);
            LSClientHelper.JumpReplay(self.Room(), toFrame);
        }

        private static void OnReplaySpeedClicked(this UGFUILSRoomComponent self)
        {
            LSReplayUpdater lsReplayUpdater = self.Room().GetComponent<LSReplayUpdater>();
            lsReplayUpdater.ChangeReplaySpeed();
            self.SpeedText.text = $"X{lsReplayUpdater.ReplaySpeed}";
        }

        private static void OnSaveReplay(this UGFUILSRoomComponent self)
        {
            string name = self.SaveNameInputField.text;

            LSClientHelper.SaveReplay(self.Room(), name);
        }
    }
}
