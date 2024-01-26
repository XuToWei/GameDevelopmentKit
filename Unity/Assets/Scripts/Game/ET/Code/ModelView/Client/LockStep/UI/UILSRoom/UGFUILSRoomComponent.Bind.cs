// This is an automatically generated class by CodeBind. Please do not modify it.

namespace ET.Client
{
    public partial class UGFUILSRoomComponent : CodeBind.ICSCodeBind
    {
        public CodeBind.CSCodeBindMono Mono { get; private set; }
        public UnityEngine.Transform Transform { get; private set; }

        public UnityEngine.UI.Text ProgressText { get; private set; }
        public UnityEngine.UI.Text PredictText { get; private set; }
        public UnityEngine.UI.Text FrameCountText { get; private set; }
        public UnityEngine.Transform ReplayTransform { get; private set; }
        public UnityEngine.UI.InputField JumpToCountInputField { get; private set; }
        public UnityEngine.UI.Button JumpButton { get; private set; }
        public UnityEngine.UI.Button SpeedButton { get; private set; }
        public UnityEngine.UI.Text SpeedText { get; private set; }
        public UnityEngine.Transform PlayTransform { get; private set; }
        public UnityEngine.UI.InputField SaveNameInputField { get; private set; }
        public UnityEngine.UI.Button SaveReplayButton { get; private set; }


        public void InitBind(CodeBind.CSCodeBindMono mono)
        {
            Mono = mono;
            Transform = mono.transform;
            ProgressText = Mono.BindComponents[0] as UnityEngine.UI.Text;
            PredictText = Mono.BindComponents[1] as UnityEngine.UI.Text;
            FrameCountText = Mono.BindComponents[2] as UnityEngine.UI.Text;
            ReplayTransform = Mono.BindComponents[3] as UnityEngine.Transform;
            JumpToCountInputField = Mono.BindComponents[4] as UnityEngine.UI.InputField;
            JumpButton = Mono.BindComponents[5] as UnityEngine.UI.Button;
            SpeedButton = Mono.BindComponents[6] as UnityEngine.UI.Button;
            SpeedText = Mono.BindComponents[7] as UnityEngine.UI.Text;
            PlayTransform = Mono.BindComponents[8] as UnityEngine.Transform;
            SaveNameInputField = Mono.BindComponents[9] as UnityEngine.UI.InputField;
            SaveReplayButton = Mono.BindComponents[10] as UnityEngine.UI.Button;
        }

        public void ClearBind()
        {
            Mono = null;
            Transform = null;
            ProgressText = null;
            PredictText = null;
            FrameCountText = null;
            ReplayTransform = null;
            JumpToCountInputField = null;
            JumpButton = null;
            SpeedButton = null;
            SpeedText = null;
            PlayTransform = null;
            SaveNameInputField = null;
            SaveReplayButton = null;
        }
    }
}
