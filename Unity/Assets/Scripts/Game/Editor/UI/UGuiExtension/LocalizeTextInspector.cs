using System.Linq;
using Game.Editor;
using GameFramework.Localization;
using Sirenix.OdinInspector.Editor;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(LocalizeText))]
    public class LocalizeTextInspector : OdinEditor
    {
        private LocalizeText m_LocalizeText;

        protected override void OnEnable()
        {
            LocalizationTool.TryRefreshData();
            base.OnEnable();
            m_LocalizeText = (LocalizeText)target;
            if (!LocalizationReadyLanguage.Languages.Contains(m_LocalizeText.EditorLocalizationLanguage))
            {
                m_LocalizeText.EditorLocalizationLanguage = LocalizationReadyLanguage.Languages.Length > 0 ? LocalizationReadyLanguage.Languages[0] : Language.Unspecified;
                serializedObject.ApplyModifiedProperties();
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (!string.IsNullOrEmpty(m_LocalizeText.LocalizationKey))
            {
                string text = LocalizationTool.GetString(m_LocalizeText.EditorLocalizationLanguage, m_LocalizeText.LocalizationKey);
                m_LocalizeText.Text.text = text;
                EditorGUILayout.LabelField("Text", text);
            }
        }
    }
}
