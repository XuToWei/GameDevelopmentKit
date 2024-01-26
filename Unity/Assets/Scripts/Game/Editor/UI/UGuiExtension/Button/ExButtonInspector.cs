using UnityEditor;
using UnityEditor.UI;

namespace Game.Editor
{
    [CustomEditor(typeof(ExButton), true)]
    [CanEditMultipleObjects]
    public class ExButtonInspector : ButtonEditor
    {
        private SerializedProperty m_OnPointerDownProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_OnPointerDownProperty = serializedObject.FindProperty("m_OnPointerDown");
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_OnPointerDownProperty);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
