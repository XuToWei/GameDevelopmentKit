using UnityEditor;

namespace Game.Editor
{
    [CustomEditor(typeof(RaycastGraphic))]
    public sealed class RaycastGraphicInspector : UnityEditor.Editor
    {
        SerializedProperty m_RaycastTarget;
        
        private void OnEnable()
        {
            m_RaycastTarget = serializedObject.FindProperty("m_RaycastTarget");
        }
        
        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(m_RaycastTarget);
        }
    }
}
