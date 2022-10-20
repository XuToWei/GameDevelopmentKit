namespace ET.Client
{
    public partial class UIMatchRoomView
    {
        [UnityEngine.SerializeField] private TMPro.TextMeshProUGUI m_RoomCodeTMPProText;
        [UnityEngine.SerializeField] private UnityEngine.UI.Image m_MyPlayerImage;
        [UnityEngine.SerializeField] private UnityEngine.UI.Image m_EnemyPlayerImage;
        [UnityEngine.SerializeField] private UnityEngine.UI.Button m_FightStartButton;
        public TMPro.TextMeshProUGUI RoomCodeTMPProText => m_RoomCodeTMPProText;
        public UnityEngine.UI.Image MyPlayerImage => m_MyPlayerImage;
        public UnityEngine.UI.Image EnemyPlayerImage => m_EnemyPlayerImage;
        public UnityEngine.UI.Button FightStartButton => m_FightStartButton;
    }
}
