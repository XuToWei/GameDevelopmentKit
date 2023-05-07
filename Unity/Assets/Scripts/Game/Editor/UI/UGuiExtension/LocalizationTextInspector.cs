using System.Linq;
using Game.Editor;
using GameFramework.Localization;
using Sirenix.OdinInspector.Editor;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(LocalizationText))]
    public class LocalizationTextInspector : OdinEditor
    {
        private LocalizationText m_LocalizationText;

        protected override void OnEnable()
        {
            LocalizationTool.TryRefreshData();
            base.OnEnable();
            m_LocalizationText = (LocalizationText)target;
            if (!LocalizationReadyLanguage.Languages.Contains(m_LocalizationText.EditorLocalizationLanguage))
            {
                m_LocalizationText.EditorLocalizationLanguage = LocalizationReadyLanguage.Languages.Length > 0 ? LocalizationReadyLanguage.Languages[0] : Language.Unspecified;
                serializedObject.ApplyModifiedProperties();
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (!string.IsNullOrEmpty(m_LocalizationText.LocalizationKey))
            {
                string text = LocalizationTool.GetString(m_LocalizationText.EditorLocalizationLanguage, m_LocalizationText.LocalizationKey);
                m_LocalizationText.Text.text = text;
                EditorGUILayout.LabelField("Text", text);
            }
        }
    }
}
