using Game;
using UnityEngine;

namespace ET.Client
{
    [FriendOf(typeof(UGFUILSRoom))]
    public static partial class UGFUILSRoomSystem
    {
        [UGFUIFormEvent(UGFUIFormId.UILSRoom)]
        private class UGFUILSRoomEvent : AUGFUIFormEvent
        {
            public override void OnInit(UGFUIForm uiForm, object userData)
            {
                base.OnInit(uiForm, userData);
                UGFUILSRoom uiLSRoom = uiForm.AddComponent<UGFUILSRoom, Transform>(uiForm.transform);
                Room room = uiLSRoom.Room();
                if (room.IsReplay)
                {
                    uiLSRoom.replayTransform.gameObject.SetActive(true);
                    uiLSRoom.playTransform.gameObject.SetActive(false);
                    uiLSRoom.jumpButton.Set(uiLSRoom.JumpReplay);
                    uiLSRoom.speedButton.Set(uiLSRoom.OnReplaySpeedClicked);
                    uiLSRoom.frameCountText.text = uiLSRoom.Room().Replay.FrameInputs.Count.ToString();
                }
                else
                {
                    uiLSRoom.replayTransform.gameObject.SetActive(false);
                    uiLSRoom.playTransform.gameObject.SetActive(true);
                    uiLSRoom.saveReplayButton.Set(uiLSRoom.OnSaveReplay);
                }
            }
            
            public override void OnOpen(UGFUIForm uiForm, object userData)
            {
                base.OnOpen(uiForm, userData);
                
            }

            public override void OnUpdate(UGFUIForm uiForm, float elapseSeconds, float realElapseSeconds)
            {
                base.OnUpdate(uiForm, elapseSeconds, realElapseSeconds);
                UGFUILSRoom uiLSRoom = uiForm.GetComponent<UGFUILSRoom>();
                Room room = uiLSRoom.Room();
                if (uiLSRoom.frame != room.AuthorityFrame)
                {
                    uiLSRoom.frame = room.AuthorityFrame;
                    uiLSRoom.frameCountText.text = room.AuthorityFrame.ToString();
                }

                if (!room.IsReplay)
                {
                    if (uiLSRoom.predictFrame != room.PredictionFrame)
                    {
                        uiLSRoom.predictFrame = room.PredictionFrame;
                        uiLSRoom.predictText.text = room.PredictionFrame.ToString();
                    }
                }
            }
        }
    }
}
