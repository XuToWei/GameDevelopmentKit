namespace ET.Client
{
    public partial class LoginForm
    {
        [UnityEngine.SerializeField] private TMPro.TMP_InputField m_AccountTMPInputField;
        [UnityEngine.SerializeField] private TMPro.TMP_InputField m_PasswordTMPInputField;
        [UnityEngine.SerializeField] private UnityEngine.UI.Button m_LoginButton;
        [UnityEngine.SerializeField] private UnityEngine.UI.Button m_RegisterButton;
        [UnityEngine.SerializeField] private UnityEngine.UI.Image m_TestImage;
        public TMPro.TMP_InputField AccountTMPInputField => m_AccountTMPInputField;
        public TMPro.TMP_InputField PasswordTMPInputField => m_PasswordTMPInputField;
        public UnityEngine.UI.Button LoginButton => m_LoginButton;
        public UnityEngine.UI.Button RegisterButton => m_RegisterButton;
        public UnityEngine.UI.Image TestImage => m_TestImage;
    }
}
