using UnityEngine;
using UnityEditor;

namespace ThunderFireUITool {
    [CustomEditor(typeof(Reddot))]
    public class ReddotEditor : Editor {
        private SerializedProperty m_Path;
        private SerializedProperty m_ReddotFlag;

        void OnEnable(){
            m_Path = serializedObject.FindProperty("path");
            m_ReddotFlag = serializedObject.FindProperty("reddotFlag");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(m_Path);
            EditorGUILayout.PropertyField(m_ReddotFlag);
            serializedObject.ApplyModifiedProperties();
        }
    }
    
}