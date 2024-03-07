using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace StateController.Editor
{
    [CustomEditor(typeof(StateController))]
    public class SateControllerInspector : OdinEditor
    {
        private SerializedProperty m_Name;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_Name = serializedObject.FindProperty("m_Name");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
        }
    }
}
