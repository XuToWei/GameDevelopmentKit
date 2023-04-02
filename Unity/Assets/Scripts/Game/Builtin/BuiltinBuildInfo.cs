using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "UGF/BuildInfo", fileName = "UGFBuildInfo", order = 0)]
    public class BuiltinBuildInfo
    {
        public string GameVersion
        {
            get;
            set;
        }

        public int InternalGameVersion
        {
            get;
            set;
        }

        public string CheckVersionUrl
        {
            get;
            set;
        }

        public string WindowsAppUrl
        {
            get;
            set;
        }

        public string MacOSAppUrl
        {
            get;
            set;
        }

        public string IOSAppUrl
        {
            get;
            set;
        }

        public string AndroidAppUrl
        {
            get;
            set;
        }
    }
}
