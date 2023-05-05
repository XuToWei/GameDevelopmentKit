using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(ExButton))]
    public class ExButtonInspector : ButtonEditor
    {
        SerializedProperty m_OnPointerDownProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_OnPointerDownProperty = serializedObject.FindProperty("m_OnPointerDown");
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.PropertyField(m_OnPointerDownProperty);
        }
    }
}
