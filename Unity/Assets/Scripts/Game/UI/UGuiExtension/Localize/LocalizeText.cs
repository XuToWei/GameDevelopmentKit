using CodeBind;
using GameFramework.Localization;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [AddComponentMenu("Localize")]
    [RequireComponent(typeof(Text))]
    [CodeBindName("LocalizeText")]
    public class LocalizeText : Localize
    {
        [SerializeField]
        [HideInInspector]
        private Text m_Text;
        
        [SerializeField]
        [ValueDropdown("EditorLocalizationAllKey", DropdownTitle = "Localization Key")]
        private string m_LocalizationKey;

#if UNITY_EDITOR
        [SerializeField]
        [ValueDropdown("EditorLocalizationReadyLanguage", DropdownTitle = "Localization Language")]
        public Language EditorLocalizationLanguage;

        public string LocalizationKey => m_LocalizationKey;
        public Text Text => m_Text;
        
        private void Awake()
        {
            if (Application.isPlaying)
                return;
            m_Text = GetComponent<Text>();
        }
#endif
        
        private void OnEnable()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif
            m_Text.text = GameEntry.Localization.GetString(m_LocalizationKey);
        }
    }
}
