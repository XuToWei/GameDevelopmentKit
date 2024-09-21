using System;
using ThunderFireUnityEx;
using TMPro;

namespace UnityEngine.UI
{
    public class UXTextMeshPro : TextMeshProUGUI, ILocalizationText
    {
        [SerializeField]
        private LocalizationHelper.TextLocalizationType m_localizationType = LocalizationHelper.TextLocalizationType.RuntimeUse;
        public LocalizationHelper.TextLocalizationType localizationType { get { return m_localizationType; } set { m_localizationType = value; } }
        [SerializeField]
        private bool m_ignoreLocalization = true;
        public bool ignoreLocalization { get { return m_ignoreLocalization; } set { m_ignoreLocalization = value; } }
        /// <summary>
        /// 本地化用到的ID
        /// </summary>
        [SerializeField]
        private string m_localizationID = "";
        public string localizationID
        {
            get { return m_localizationID; }
            set { m_localizationID = value; }
        }
        [SerializeField]
        private string m_previewID = "";
        public string previewID
        {
            get { return m_previewID; }
            set
            {
                if (m_previewID != value)
                {
                    m_previewID = value;
                    ChangeLanguage(LocalizationHelper.GetLanguage());
                }
            }
        }
        private static readonly string need_replace = "UNFILLED TEXT";
        private static bool loaded = false;
        private int origin_len;

        protected override void Start()
        {
            base.Start();
            if (!Application.isPlaying) return;
            origin_len = text.Length;
            if (!loaded)
            {
                loaded = true;
            }
            ChangeLanguage(LocalizationHelper.GetLanguage());
        }

        public void ChangeLanguage(LocalizationHelper.LanguageType language)
        {

            if (language == LocalizationHelper.LanguageType.NoWord && !ignoreLocalization)
            {
                text = "";
                for (int i = 0; i < origin_len; i++)
                {
                    text += '□';
                }
                return;
            }
            string id = localizationType == LocalizationHelper.TextLocalizationType.RuntimeUse ? localizationID : m_previewID;
            if (language == LocalizationHelper.LanguageType.ShowKey && !ignoreLocalization)
            {
                text = id;
                return;
            }
            if (language >= 0 && id != "" && !ignoreLocalization)
            {
                text = LocalizationHelper.GetString(language, id, need_replace);
            }
        }
    }
}
