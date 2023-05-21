using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "Game/BuildInfo", fileName = "UGFBuildInfo", order = 0)]
    public class BuiltinBuildInfo : ScriptableObject
    {
        [SerializeField]
        private string m_GameVersion;
        public string GameVersion => m_GameVersion;

        [SerializeField]
        private int m_InternalGameVersion;
        public int InternalGameVersion => m_InternalGameVersion;

        [SerializeField]
        private string m_CheckVersionUrl;
        public string CheckVersionUrl => m_CheckVersionUrl;

        [SerializeField]
        private string m_WindowsAppUrl;
        public string WindowsAppUrl => m_WindowsAppUrl;

        [SerializeField]
        private string m_MacOSAppUrl;
        public string MacOSAppUrl => m_MacOSAppUrl;

        [SerializeField]
        private string m_IOSAppUrl;
        public string IOSAppUrl => IOSAppUrl;

        [SerializeField]
        private string m_AndroidAppUrl;
        public string AndroidAppUrl => m_AndroidAppUrl;
    }
}
