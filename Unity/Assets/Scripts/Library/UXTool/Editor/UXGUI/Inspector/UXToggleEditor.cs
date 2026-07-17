using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(UXToggle))]
    [CanEditMultipleObjects]
    public sealed class UXToggleEditor : ToggleEditor
    {
        private SerializedProperty m_ToggleAnimatorProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_ToggleAnimatorProperty = serializedObject.FindProperty("m_ToggleAnimator");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_ToggleAnimatorProperty);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
