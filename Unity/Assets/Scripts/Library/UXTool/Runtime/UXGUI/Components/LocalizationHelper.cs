using System;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityGameFramework.Runtime;

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
#if UNITY_6000_0_OR_NEWER
        ILocalization[] allObjects = UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID).OfType<ILocalization>().ToArray();
        foreach (var obj in allObjects)
        {
            obj.ChangeLanguage(GetLanguage());
        }
#elif UNITY_2020_3_OR_NEWER
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

    public static string GetString(LanguageType languageType, string key, string defaultString)
    {
#if UNITY_EDITOR
        if (s_LocalizationComponent != null)
        {
            if(!string.Equals(s_BaseComponent.EditorLanguage.ToString(), languageType.ToString()))
            {
                throw new Exception($"UXTool设置语言 {languageType} 错误（编辑器语言为：{s_BaseComponent.EditorLanguage}）!");
            }
        }
        else
        {
            return s_EditorGetStringFunc.Invoke(languageType, key, defaultString);
        }
#endif
        if (s_LocalizationComponent.HasRawString(key))
        {
            return s_LocalizationComponent.GetRawString(key);
        }
        return defaultString;
    }

    private static LocalizationComponent s_LocalizationComponent;
    private static BaseComponent s_BaseComponent;
#if UNITY_EDITOR
    private static Func<LanguageType, string, string, string> s_EditorGetStringFunc;
    public static void SetEditorGetStringFunc(Func<LanguageType, string, string, string> func)
    {
        s_EditorGetStringFunc = func;
    }
#endif
    public static async UniTask InitAsync()
    {
        await UniTask.CompletedTask;
        s_LocalizationComponent = GameEntry.GetComponent<LocalizationComponent>();
        s_BaseComponent = GameEntry.GetComponent<BaseComponent>();
    }

    public static void Clear()
    {
        s_LocalizationComponent = null;
        s_BaseComponent = null;
    }
}
