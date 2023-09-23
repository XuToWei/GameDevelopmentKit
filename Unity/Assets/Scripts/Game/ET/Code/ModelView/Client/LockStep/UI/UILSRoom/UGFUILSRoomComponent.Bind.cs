// This is an automatically generated class by CodeBind. Please do not modify it.

namespace ET.Client
{
    public partial class UGFUILSRoomComponent : CodeBind.ICSCodeBind
    {
        public CodeBind.CSCodeBindMono mono { get; private set; }

        public UnityEngine.Transform transform { get; private set; }

        public UnityEngine.UI.Text progressText { get; private set; }

        public UnityEngine.UI.Text predictText { get; private set; }

        public UnityEngine.UI.Text frameCountText { get; private set; }

        public UnityEngine.Transform replayTransform { get; private set; }

        public UnityEngine.UI.InputField jumpToCountInputField { get; private set; }

        public UnityEngine.UI.Button jumpButton { get; private set; }

        public UnityEngine.UI.Button speedButton { get; private set; }

        public UnityEngine.UI.Text speedText { get; private set; }

        public UnityEngine.Transform playTransform { get; private set; }

        public UnityEngine.UI.InputField saveNameInputField { get; private set; }

        public UnityEngine.UI.Button saveReplayButton { get; private set; }

        public void InitBind(CodeBind.CSCodeBindMono mono)
        {
            this.mono = mono;
            this.transform = mono.transform;
            this.progressText = this.mono.bindComponents[0] as UnityEngine.UI.Text;
            this.predictText = this.mono.bindComponents[1] as UnityEngine.UI.Text;
            this.frameCountText = this.mono.bindComponents[2] as UnityEngine.UI.Text;
            this.replayTransform = this.mono.bindComponents[3] as UnityEngine.Transform;
            this.jumpToCountInputField = this.mono.bindComponents[4] as UnityEngine.UI.InputField;
            this.jumpButton = this.mono.bindComponents[5] as UnityEngine.UI.Button;
            this.speedButton = this.mono.bindComponents[6] as UnityEngine.UI.Button;
            this.speedText = this.mono.bindComponents[7] as UnityEngine.UI.Text;
            this.playTransform = this.mono.bindComponents[8] as UnityEngine.Transform;
            this.saveNameInputField = this.mono.bindComponents[9] as UnityEngine.UI.InputField;
            this.saveReplayButton = this.mono.bindComponents[10] as UnityEngine.UI.Button;
        }

        public void ClearBind()
        {
            this.mono = null;
            this.transform = null;
            this.progressText = null;
            this.predictText = null;
            this.frameCountText = null;
            this.replayTransform = null;
            this.jumpToCountInputField = null;
            this.jumpButton = null;
            this.speedButton = null;
            this.speedText = null;
            this.playTransform = null;
            this.saveNameInputField = null;
            this.saveReplayButton = null;
        }
    }
}
