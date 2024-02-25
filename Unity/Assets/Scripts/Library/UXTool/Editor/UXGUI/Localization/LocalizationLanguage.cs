using UnityEngine;
using ThunderFireUITool;

public class LocalizationLanguage
{
    private static readonly long[] m_languages = new long[] {
        EditorLocalizationStorage.Def_简体中文,
        EditorLocalizationStorage.Def_繁体中文,
        EditorLocalizationStorage.Def_英文,
        EditorLocalizationStorage.Def_日语,
        EditorLocalizationStorage.Def_韩语,
        EditorLocalizationStorage.Def_法语,
        EditorLocalizationStorage.Def_德语,
        EditorLocalizationStorage.Def_西班牙语,
        EditorLocalizationStorage.Def_俄语,
        EditorLocalizationStorage.Def_土耳其语,
        EditorLocalizationStorage.Def_葡萄牙语,
        EditorLocalizationStorage.Def_越南语,
        EditorLocalizationStorage.Def_泰语,
        EditorLocalizationStorage.Def_阿拉伯语
    };
    public static int Length
    {
        get
        {
            return m_languages.Length;
        }
    }
    public static string GetLanguage(int index)
    {
        return EditorLocalization.GetLocalization(m_languages[index]);
    }
}