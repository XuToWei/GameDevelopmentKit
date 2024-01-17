using Luban;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace Game.Editor
{
    [CustomEditor(typeof(TablesComponent))]
    public class TablesComponentInspector : OdinEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            EditorGUILayout.LabelField($"All Tables memory size about(≈) {TablesMemory.MemorySize / (1024 * 1024 * 1f):0.00} mb.");
        }
    }
}
