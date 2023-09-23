// This is an automatically generated class by CodeBind. Please do not modify it.

namespace ET.Client
{
    public partial class UGFUILSLoginComponent : CodeBind.ICSCodeBind
    {
        public CodeBind.CSCodeBindMono mono { get; private set; }

        public UnityEngine.Transform transform { get; private set; }

        public UnityEngine.UI.InputField accountInputField { get; private set; }

        public UnityEngine.UI.InputField passwordInputField { get; private set; }

        public UnityEngine.UI.Button loginButton { get; private set; }

        public void InitBind(CodeBind.CSCodeBindMono mono)
        {
            this.mono = mono;
            this.transform = mono.transform;
            this.accountInputField = this.mono.bindComponents[0] as UnityEngine.UI.InputField;
            this.passwordInputField = this.mono.bindComponents[1] as UnityEngine.UI.InputField;
            this.loginButton = this.mono.bindComponents[2] as UnityEngine.UI.Button;
        }

        public void ClearBind()
        {
            this.mono = null;
            this.transform = null;
            this.accountInputField = null;
            this.passwordInputField = null;
            this.loginButton = null;
        }
    }
}
