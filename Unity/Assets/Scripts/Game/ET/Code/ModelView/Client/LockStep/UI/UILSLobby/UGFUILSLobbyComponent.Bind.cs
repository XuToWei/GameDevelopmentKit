// This is an automatically generated class by CodeBind. Please do not modify it.

namespace ET.Client
{
    public partial class UGFUILSLobbyComponent : CodeBind.ICSCodeBind
    {
        public CodeBind.CSCodeBindMono Mono { get; private set; }
        public UnityEngine.Transform Transform { get; private set; }

        public UnityEngine.UI.Button EnterMapButton { get; private set; }
        public UnityEngine.UI.InputField ReplayPathInputField { get; private set; }
        public UnityEngine.UI.Button ReplayButton { get; private set; }


        public void InitBind(CodeBind.CSCodeBindMono mono)
        {
            Mono = mono;
            Transform = mono.transform;
            EnterMapButton = Mono.BindComponents[0] as UnityEngine.UI.Button;
            ReplayPathInputField = Mono.BindComponents[1] as UnityEngine.UI.InputField;
            ReplayButton = Mono.BindComponents[2] as UnityEngine.UI.Button;
        }

        public void ClearBind()
        {
            Mono = null;
            Transform = null;
            EnterMapButton = null;
            ReplayPathInputField = null;
            ReplayButton = null;
        }
    }
}
