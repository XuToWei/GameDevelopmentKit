using GameFramework.Localization;

namespace UnityEngine.UI
{
    public abstract class Localize : MonoBehaviour
    {
#if UNITY_EDITOR
        //编辑器使用的数据
        public static string[] EditorLocalizationAllKey;
        public static Language[] EditorLocalizationReadyLanguage;
#endif
    }
}
