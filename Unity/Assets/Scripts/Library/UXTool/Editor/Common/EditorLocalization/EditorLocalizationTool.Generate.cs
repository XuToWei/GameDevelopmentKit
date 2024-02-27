// This is an automatically generated class by Share.Tool. Please do not modify it.

using GameFramework.Localization;

namespace ThunderFireUITool
{
    public static partial class EditorLocalizationTool
    {
        public static LocalizationHelper.LanguageType[] ReadyLanguageTypes => new LocalizationHelper.LanguageType[]
        {
            LocalizationHelper.LanguageType.ChineseSimplified,
            LocalizationHelper.LanguageType.ChineseTraditional,
            LocalizationHelper.LanguageType.English,
            LocalizationHelper.LanguageType.Korean,
        };

        public static string GetLocalizationAsset(LocalizationHelper.LanguageType languageType)
        {
            return $"Assets/Res/Localization/{languageType}/Localization.bytes";
        }
    }
}
