using Game;
using UnityEngine;

namespace ET.Client
{
    [FriendOf(typeof(UGFUILSRoomComponent))]
    public static partial class UGFUILSRoomComponentSystem
    {
        [UGFUIFormEvent(UGFUIFormId.UILSRoom)]
        private class UGFUILSRoomComponentEvent : AUGFUIFormEvent
        {
            public override void OnInit(UGFUIForm uiForm, object userData)
            {
                base.OnInit(uiForm, userData);
                UGFUILSRoomComponent uiLSRoom = uiForm.AddComponent<UGFUILSRoomComponent, Transform>(uiForm.Transform);
                Room room = uiLSRoom.Room();
                if (room.IsReplay)
                {
                    uiLSRoom.ReplayTransform.gameObject.SetActive(true);
                    uiLSRoom.PlayTransform.gameObject.SetActive(false);
                    uiLSRoom.JumpButton.Set(uiLSRoom.JumpReplay);
                    uiLSRoom.SpeedButton.Set(uiLSRoom.OnReplaySpeedClicked);
                    uiLSRoom.FrameCountText.text = uiLSRoom.Room().Replay.FrameInputs.Count.ToString();
                }
                else
                {
                    uiLSRoom.ReplayTransform.gameObject.SetActive(false);
                    uiLSRoom.PlayTransform.gameObject.SetActive(true);
                    uiLSRoom.SaveReplayButton.Set(uiLSRoom.OnSaveReplay);
                }
            }

            public override void OnOpen(UGFUIForm uiForm, object userData)
            {
                base.OnOpen(uiForm, userData);
                
            }

            public override void OnUpdate(UGFUIForm uiForm, float elapseSeconds, float realElapseSeconds)
            {
                base.OnUpdate(uiForm, elapseSeconds, realElapseSeconds);
                UGFUILSRoomComponent uiLSRoom = uiForm.GetComponent<UGFUILSRoomComponent>();
                Room room = uiLSRoom.Room();
                if (uiLSRoom.frame != room.AuthorityFrame)
                {
                    uiLSRoom.frame = room.AuthorityFrame;
                    uiLSRoom.FrameCountText.text = room.AuthorityFrame.ToString();
                }

                if (!room.IsReplay)
                {
                    if (uiLSRoom.predictFrame != room.PredictionFrame)
                    {
                        uiLSRoom.predictFrame = room.PredictionFrame;
                        uiLSRoom.PredictText.text = room.PredictionFrame.ToString();
                    }
                }
            }
        }
    }
}
