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
                self.View.replayTransform.gameObject.SetActive(true);
                self.View.playTransform.gameObject.SetActive(false);
                self.View.jumpButton.Set(self.JumpReplay);
                self.View.speedButton.Set(self.OnReplaySpeedClicked);
                self.View.frameCountText.text = self.Room().Replay.FrameInputs.Count.ToString();
            }
            else
            {
                self.View.replayTransform.gameObject.SetActive(false);
                self.View.playTransform.gameObject.SetActive(true);
                self.View.saveReplayButton.Set(self.OnSaveReplay);
            }
        }

        [UGFUIFormSystem]
        private static void UGFUIFormOnUpdate(this UIFormLSRoomComponent self, float elapseSeconds, float realElapseSeconds)
        {
            Room room = self.Room();
            if (self.frame != room.AuthorityFrame)
            {
                self.frame = room.AuthorityFrame;
                self.View.frameCountText.text = room.AuthorityFrame.ToString();
            }

            if (!room.IsReplay)
            {
                if (self.predictFrame != room.PredictionFrame)
                {
                    self.predictFrame = room.PredictionFrame;
                    self.View.predictText.text = room.PredictionFrame.ToString();
                }
            }
        }

        private static void JumpReplay(this UIFormLSRoomComponent self)
        {
            int toFrame = int.Parse(self.View.jumpToCountInputField.text);
            LSClientHelper.JumpReplay(self.Room(), toFrame);
        }

        private static void OnReplaySpeedClicked(this UIFormLSRoomComponent self)
        {
            LSReplayUpdater lsReplayUpdater = self.Room().GetComponent<LSReplayUpdater>();
            lsReplayUpdater.ChangeReplaySpeed();
            self.View.speedText.text = $"X{lsReplayUpdater.ReplaySpeed}";
        }

        private static void OnSaveReplay(this UIFormLSRoomComponent self)
        {
            string name = self.View.saveNameInputField.text;

            LSClientHelper.SaveReplay(self.Room(), name);
        }
    }
}
