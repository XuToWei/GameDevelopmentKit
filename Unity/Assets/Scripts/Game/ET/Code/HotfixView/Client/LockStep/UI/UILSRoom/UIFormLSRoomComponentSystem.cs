using Game;

namespace ET.Client
{
    [EntitySystemOf(typeof(UIFormLSRoomComponent))]
    [FriendOf(typeof(UIFormLSRoomComponent))]
    public static partial class UIFormLSRoomComponentSystem
    {
        [UGFUIFormSystem]
        private static void UGFUIFormOnOpen(this UIFormLSRoomComponent self)
        {
            Room room = self.Room();
            if (room.IsReplay)
            {
                self.View.ReplayTransform.gameObject.SetActive(true);
                self.View.PlayTransform.gameObject.SetActive(false);
                self.View.JumpButton.Set(self.JumpReplay);
                self.View.SpeedButton.Set(self.OnReplaySpeedClicked);
                self.View.FrameCountText.text = self.Room().Replay.FrameInputs.Count.ToString();
            }
            else
            {
                self.View.ReplayTransform.gameObject.SetActive(false);
                self.View.PlayTransform.gameObject.SetActive(true);
                self.View.SaveReplayButton.Set(self.OnSaveReplay);
            }
        }

        [UGFUIFormSystem]
        private static void UGFUIFormOnUpdate(this UIFormLSRoomComponent self, float elapseSeconds, float realElapseSeconds)
        {
            Room room = self.Room();
            if (self.frame != room.AuthorityFrame)
            {
                self.frame = room.AuthorityFrame;
                self.View.FrameCountText.text = room.AuthorityFrame.ToString();
            }

            if (!room.IsReplay)
            {
                if (self.predictFrame != room.PredictionFrame)
                {
                    self.predictFrame = room.PredictionFrame;
                    self.View.PredictText.text = room.PredictionFrame.ToString();
                }
            }
        }

        private static void JumpReplay(this UIFormLSRoomComponent self)
        {
            int toFrame = int.Parse(self.View.JumpToCountInputField.text);
            LSClientHelper.JumpReplay(self.Room(), toFrame);
        }

        private static void OnReplaySpeedClicked(this UIFormLSRoomComponent self)
        {
            LSReplayUpdater lsReplayUpdater = self.Room().GetComponent<LSReplayUpdater>();
            lsReplayUpdater.ChangeReplaySpeed();
            self.View.SpeedText.text = $"X{lsReplayUpdater.ReplaySpeed}";
        }

        private static void OnSaveReplay(this UIFormLSRoomComponent self)
        {
            string name = self.View.SaveNameInputField.text;

            LSClientHelper.SaveReplay(self.Room(), name);
        }
    }
}
