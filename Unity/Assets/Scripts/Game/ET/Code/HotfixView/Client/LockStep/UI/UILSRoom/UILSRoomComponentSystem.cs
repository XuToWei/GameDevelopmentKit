using Game;

namespace ET.Client
{
    [EntitySystemOf(typeof(UILSRoomComponent))]
    [FriendOf(typeof(UILSRoomComponent))]
    public static partial class UILSRoomComponentSystem
    {
        [UGFUIFormSystem]
        private static void UGFUIFormOnOpen(this UILSRoomComponent self)
        {
            Room room = self.Room();
            if (room.IsReplay)
            {
                self.Mono.ReplayTransform.gameObject.SetActive(true);
                self.Mono.PlayTransform.gameObject.SetActive(false);
                self.Mono.JumpButton.Set(self.JumpReplay);
                self.Mono.SpeedButton.Set(self.OnReplaySpeedClicked);
                self.Mono.FrameCountText.text = self.Room().Replay.FrameInputs.Count.ToString();
            }
            else
            {
                self.Mono.ReplayTransform.gameObject.SetActive(false);
                self.Mono.PlayTransform.gameObject.SetActive(true);
                self.Mono.SaveReplayButton.Set(self.OnSaveReplay);
            }
        }

        [UGFUIFormSystem]
        private static void UGFUIFormOnUpdate(this UILSRoomComponent self, float elapseSeconds, float realElapseSeconds)
        {
            Room room = self.Room();
            if (self.frame != room.AuthorityFrame)
            {
                self.frame = room.AuthorityFrame;
                self.Mono.FrameCountText.text = room.AuthorityFrame.ToString();
            }

            if (!room.IsReplay)
            {
                if (self.predictFrame != room.PredictionFrame)
                {
                    self.predictFrame = room.PredictionFrame;
                    self.Mono.PredictText.text = room.PredictionFrame.ToString();
                }
            }
        }

        private static void JumpReplay(this UILSRoomComponent self)
        {
            int toFrame = int.Parse(self.Mono.JumpToCountInputField.text);
            LSClientHelper.JumpReplay(self.Room(), toFrame);
        }

        private static void OnReplaySpeedClicked(this UILSRoomComponent self)
        {
            LSReplayUpdater lsReplayUpdater = self.Room().GetComponent<LSReplayUpdater>();
            lsReplayUpdater.ChangeReplaySpeed();
            self.Mono.SpeedText.text = $"X{lsReplayUpdater.ReplaySpeed}";
        }

        private static void OnSaveReplay(this UILSRoomComponent self)
        {
            string name = self.Mono.SaveNameInputField.text;

            LSClientHelper.SaveReplay(self.Room(), name);
        }
    }
}
