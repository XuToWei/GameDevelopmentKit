using CodeBind;
using GameFramework.Localization;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Game
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [AddComponentMenu("Localize")]
    [RequireComponent(typeof(TMP_Text))]
    [CodeBindName("LocalizeTMPText")]
    public class LocalizeTMPText : Localize
    {
        [SerializeField]
        [HideInInspector]
        private TMP_Text m_TMP_Text;
        
        [SerializeField]
        [ValueDropdown("EditorLocalizationAllKey", DropdownTitle = "Localization Key")]
        private string m_LocalizationKey;

#if UNITY_EDITOR
        [SerializeField]
        [ValueDropdown("EditorLocalizationReadyLanguage", DropdownTitle = "Localization Language")]
        public Language EditorLocalizationLanguage;

        public string LocalizationKey => m_LocalizationKey;
        public TMP_Text TMP_Text => m_TMP_Text;
        
        private void Awake()
        {
            if (Application.isPlaying)
                return;
            m_TMP_Text = GetComponent<TMP_Text>();
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
            m_TMP_Text.text = GameEntry.Localization.GetString(m_LocalizationKey);
        }
    }
}
