using GameFramework.Localization;
using UnityEngine;

namespace Game
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
