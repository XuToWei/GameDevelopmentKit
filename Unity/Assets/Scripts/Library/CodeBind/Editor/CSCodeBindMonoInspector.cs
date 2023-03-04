using UnityEditor;
using UnityEngine;

namespace CodeBind.Editor
{
    [CustomEditor(typeof(CSCodeBindMono))]
    public class CSCodeBindMonoInspector : UnityEditor.Editor
    {
        private SerializedProperty m_SeparatorChar;
        private SerializedProperty m_BindScript;
        
        private SerializedProperty m_BindComponents;
        private SerializedProperty m_BindComponentNames;

        private bool m_ShowBindComponets;
        
        private void OnEnable()
        {
            this.m_SeparatorChar = serializedObject.FindProperty("m_SeparatorChar");
            this.m_BindScript = serializedObject.FindProperty("m_BindScript");
            
            this.m_BindComponents = serializedObject.FindProperty("m_BindComponents");
            this.m_BindComponentNames = serializedObject.FindProperty("m_BindComponentNames");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                if (GUILayout.Button("生成绑定代码和数据"))
                {
                    CSCodeBinder codeBinder = new CSCodeBinder(this.m_BindScript.objectReferenceValue as MonoScript, (this.target as MonoBehaviour).transform, (char)this.m_SeparatorChar.intValue);
                    codeBinder.TryGenerateBindCode();
                    codeBinder.TrySetSerialization();
                }

                EditorGUILayout.PropertyField(this.m_SeparatorChar);
                EditorGUILayout.PropertyField(this.m_BindScript);
                this.m_ShowBindComponets = EditorGUILayout.BeginFoldoutHeaderGroup(this.m_ShowBindComponets, $"Bind Data (count:{this.m_BindComponents.arraySize})");
                {
                    if (this.m_ShowBindComponets)
                    {
                        EditorGUI.BeginDisabledGroup(true);
                        {
                            GUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Name");
                            EditorGUILayout.LabelField("Component");
                            GUILayout.EndHorizontal();
                            for (int i = 0; i < this.m_BindComponents.arraySize; i++)
                            {
                                GUILayout.BeginHorizontal();
                                string cName = this.m_BindComponentNames.GetArrayElementAtIndex(i).stringValue;
                                EditorGUILayout.TextField(cName);
                                EditorGUILayout.ObjectField(this.m_BindComponents.GetArrayElementAtIndex(i).objectReferenceValue, typeof (Component), true);
                                GUILayout.EndHorizontal();
                            }
                        }
                        EditorGUI.EndDisabledGroup();
                    }
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
            EditorGUI.EndDisabledGroup();
            
            serializedObject.ApplyModifiedProperties();
        }

        private void RefreshTypeNames()
        {

            serializedObject.ApplyModifiedProperties();
        }
    }
}
