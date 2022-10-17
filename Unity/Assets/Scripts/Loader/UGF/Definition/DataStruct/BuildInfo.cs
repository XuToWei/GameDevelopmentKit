//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using UnityEngine;

namespace UGF
{
    [CreateAssetMenu(menuName = "UGF/BuildInfo", fileName = "UGFBuildInfo", order = 0)]
    public class BuildInfo : ScriptableObject
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
