namespace MultiPlay
{
    internal static class Settings
    {
        //Settings: Hard coded
        public static MultiPlaySettings SettingsAsset;
        public static readonly int MaxClonesLimit = 30;
        public const Licence productLicence = Licence.Full;

        //Settings Preferences
        public static int MaxClones;
        public static string ClonesPath;
        public static bool LinkLibrary;

        public static void SaveSettings()
        {
            SettingsAsset.clonesPath = ClonesPath;
            SettingsAsset.maxNumberOfClones = MaxClones;
            SettingsAsset.linkLibrary = LinkLibrary;
        }

        public static void LoadSettings(MultiPlayEditor multiPlayEditor)
        {
            if (string.IsNullOrEmpty(ClonesPath))
            {
                ClonesPath = SettingsAsset.clonesPath;
            }
            MaxClones = productLicence == Licence.Default? 1 : SettingsAsset.maxNumberOfClones;
            LinkLibrary = SettingsAsset.linkLibrary;

            if (string.IsNullOrEmpty(ClonesPath))
            {
                ClonesPath = ClonesPath.Replace(@"/", @"\");
            }
        }

        public enum Licence { Default, Full }

    }
}