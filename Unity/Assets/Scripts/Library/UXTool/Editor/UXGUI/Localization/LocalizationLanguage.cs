using System.Collections.Generic;

using ThunderFireUITool;

public class LocalizationLanguage
{
    private static readonly Dictionary<LocalizationHelper.LanguageType, long> m_languages = new Dictionary<LocalizationHelper.LanguageType, long>
    {
        { LocalizationHelper.LanguageType.ChineseSimplified, EditorLocalizationStorage.Def_简体中文 },
        { LocalizationHelper.LanguageType.ChineseTraditional, EditorLocalizationStorage.Def_繁体中文 },
        { LocalizationHelper.LanguageType.English, EditorLocalizationStorage.Def_英文 },
        { LocalizationHelper.LanguageType.Japanese, EditorLocalizationStorage.Def_日语 },
        { LocalizationHelper.LanguageType.Korean, EditorLocalizationStorage.Def_韩语 },
        { LocalizationHelper.LanguageType.French, EditorLocalizationStorage.Def_法语 },
        { LocalizationHelper.LanguageType.German, EditorLocalizationStorage.Def_德语 },
        { LocalizationHelper.LanguageType.Spanish, EditorLocalizationStorage.Def_西班牙语 },
        { LocalizationHelper.LanguageType.Russian, EditorLocalizationStorage.Def_俄语 },
        { LocalizationHelper.LanguageType.Turkish, EditorLocalizationStorage.Def_土耳其语 },
        { LocalizationHelper.LanguageType.PortuguesePortugal, EditorLocalizationStorage.Def_葡萄牙语 },
        { LocalizationHelper.LanguageType.Vietnamese, EditorLocalizationStorage.Def_越南语 },
        { LocalizationHelper.LanguageType.Thai, EditorLocalizationStorage.Def_泰语 },
        { LocalizationHelper.LanguageType.Arabic, EditorLocalizationStorage.Def_阿拉伯语 }
    };

    public static Dictionary<LocalizationHelper.LanguageType, long>.KeyCollection LanguageTypes
    {
        get
        {
            return m_languages.Keys;
        }
    }

    public static string GetLanguage(LocalizationHelper.LanguageType languageType)
    {
        if (m_languages.TryGetValue(languageType, out long localizationKey))
        {
            return EditorLocalization.GetLocalization(localizationKey);
        }
        return string.Empty;
    }
}