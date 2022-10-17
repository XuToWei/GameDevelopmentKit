namespace ET
{
    public partial class MainForm
    {
        [UnityEngine.SerializeField] private TMPro.TextMeshProUGUI m_NameTMPProText;
        [UnityEngine.SerializeField] private UnityEngine.UI.Button m_SkinButton;
        [UnityEngine.SerializeField] private UnityEngine.UI.Button m_RewardButton;
        [UnityEngine.SerializeField] private UnityEngine.UI.Button m_FreeMatchButton;
        [UnityEngine.SerializeField] private UnityEngine.UI.Button m_FriendMatchButton;
        public TMPro.TextMeshProUGUI NameTMPProText => m_NameTMPProText;
        public UnityEngine.UI.Button SkinButton => m_SkinButton;
        public UnityEngine.UI.Button RewardButton => m_RewardButton;
        public UnityEngine.UI.Button FreeMatchButton => m_FreeMatchButton;
        public UnityEngine.UI.Button FriendMatchButton => m_FriendMatchButton;
    }
}
