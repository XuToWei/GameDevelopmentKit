// This is an automatically generated class by CodeBind. Please do not modify it.

namespace ET.Client
{
    public partial class ETMonoUILSLogin : ETMonoUGFUIForm
    {
        public CodeBind.CSCodeBindMono Mono { get; private set; }
        public UnityEngine.Transform Transform { get; private set; }

        public UnityEngine.UI.InputField AccountInputField { get; private set; }
        public UnityEngine.UI.InputField PasswordInputField { get; private set; }
        public UnityEngine.UI.Button LoginButton { get; private set; }


        public void InitBind(CodeBind.CSCodeBindMono mono)
        {
            Mono = mono;
            Transform = mono.transform;
            AccountInputField = Mono.BindComponents[0] as UnityEngine.UI.InputField;
            PasswordInputField = Mono.BindComponents[1] as UnityEngine.UI.InputField;
            LoginButton = Mono.BindComponents[2] as UnityEngine.UI.Button;
        }

        public void ClearBind()
        {
            Mono = null;
            Transform = null;
            AccountInputField = null;
            PasswordInputField = null;
            LoginButton = null;
        }
    }
}
