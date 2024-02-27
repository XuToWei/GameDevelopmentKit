using System.Linq;
using GameFramework.Localization;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace Game.Editor
{
    [CustomEditor(typeof(LocalizeText))]
    public class LocalizeTextInspector : OdinEditor
    {
        private LocalizeText m_Localize_Text;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_Localize_Text = (LocalizeText)target;
            if (!LocalizationReadyLanguage.Languages.Contains(m_Localize_Text.EditorLocalizationLanguage))
            {
                m_Localize_Text.EditorLocalizationLanguage = LocalizationReadyLanguage.Languages.Length > 0 ? LocalizationReadyLanguage.Languages[0] : Language.Unspecified;
                serializedObject.ApplyModifiedProperties();
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (!string.IsNullOrEmpty(m_Localize_Text.LocalizationKey))
            {
                string text = LocalizationTool.GetString(m_Localize_Text.EditorLocalizationLanguage, m_Localize_Text.LocalizationKey);
                m_Localize_Text.Text.text = text;
                EditorGUILayout.LabelField("Text", text);
            }
        }
    }
}
