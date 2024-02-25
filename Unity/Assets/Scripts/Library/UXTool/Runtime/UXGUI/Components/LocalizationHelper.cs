using UnityEngine;
using System.Linq;

public interface ILocalization
{
    bool ignoreLocalization { get; }
    Transform transform { get; }
    void ChangeLanguage(LocalizationHelper.LanguageType language);
}

public interface ILocalizationText : ILocalization
{
    string localizationID { get; }
    string text { get; }
    LocalizationHelper.TextLocalizationType localizationType { get; }
}

public class LocalizationHelper
{
    /// <summary>
    /// 游戏内设置的全局语言
    /// </summary>
    private static LanguageType globalLanguage = LanguageType.None;
    /// <summary>
    /// GameView右上角设置的预览语言
    /// </summary>
    private static LanguageType previewLangugage = LanguageType.None;
    public enum LanguageType
    {
        ShowKey = -3,
        NoWord = -2,
        None = -1,
        ChineseSimplified = 0,// 简体中文
        ChineseTraditional = 1,// 繁体中文
        English = 2,// 英语
        Japanese = 3,// 日语
        Korean = 4,// 韩语
        French = 5,// 法语
        German = 6,// 德语
        Spanish = 7,// 西班牙语
        Russian = 8,// 俄语
        Turkish = 9,// 土耳其语
        PortuguesePortugal = 10,// 葡萄牙语
        Vietnamese = 11,// 越南语
        Thai = 12,// 泰语
        Arabic = 13,// 阿拉伯语
        Italian = 14,// 意大利语
        Indonesian = 15,// 印尼语
    }
    public enum TextLocalizationType
    {
        [InspectorName("静态文本 (Runtime-Use)")]
        RuntimeUse = 0,//不需要程序代码生成的文字
        [InspectorName("动态文本 (Preview)")]
        Preview = 1,//需要程序代码生成的文字
    }

    /// <summary>
    /// 更新所有ILocalization的语言
    /// </summary>
    private static void ChangeILocalization()
    {
#if UNITY_2020_3_OR_NEWER
        ILocalization[] allObjects = UnityEngine.Object.FindObjectsOfType<MonoBehaviour>(true).OfType<ILocalization>().ToArray();
        foreach (var obj in allObjects)
        {
            obj.ChangeLanguage(GetLanguage());
        }
#else
        ILocalization[] allObjects = Resources.FindObjectsOfTypeAll<MonoBehaviour>().OfType<ILocalization>().ToArray();
        foreach (var obj in allObjects)
        {
            if(obj.transform.gameObject.scene.name == UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
            {
                obj.ChangeLanguage(GetLanguage());
            }
        }
#endif
    }

    public static LanguageType GetLanguage()
    {
        return previewLangugage == LanguageType.None ? globalLanguage : previewLangugage;
    }

    /// <summary>
    /// 切换预览语言
    /// </summary>
    /// <param name="type">语言编号</param>
    public static void SetPreviewLanguage(LanguageType type)
    {
        previewLangugage = type;
        ChangeILocalization();
    }
    /// <summary>
    /// 切换语言（用户接口）
    /// </summary>
    /// <param name="type">语言类型</param>
    public static void SetLanguage(LanguageType type)
    {
        globalLanguage = type;
        ChangeILocalization();
    }
    /// <summary>
    /// 切换语言（用户接口）
    /// </summary>
    /// <param name="type">语言编号</param>
    public static void SetLanguage(int type)
    {
        SetLanguage((LanguageType)type);
    }

    public static string GetLanguage(string key, LanguageType languageType, string defaultText = "")
    {
        return defaultText;
    }
}