namespace ET.Client
{
    public partial class UGFUILogin : CodeBind.ICSCodeBind
    {
        public CodeBind.CSCodeBindMono mono { get; private set; }

        public UnityEngine.Transform transform { get; private set; }

        public UnityEngine.UI.InputField AccountInputField { get; private set; }

        public UnityEngine.UI.InputField PasswordInputField { get; private set; }

        public UnityEngine.UI.Button LoginButton { get; private set; }

        public void InitBind(CodeBind.CSCodeBindMono mono)
        {
            this.mono = mono;
            this.transform = mono.transform;
            this.AccountInputField = this.mono.bindComponents[0] as UnityEngine.UI.InputField;
            this.PasswordInputField = this.mono.bindComponents[1] as UnityEngine.UI.InputField;
            this.LoginButton = this.mono.bindComponents[2] as UnityEngine.UI.Button;
        }

        public void ClearBind()
        {
            this.mono = null;
            this.transform = null;
            this.AccountInputField = null;
            this.PasswordInputField = null;
            this.LoginButton = null;
        }
    }
}
