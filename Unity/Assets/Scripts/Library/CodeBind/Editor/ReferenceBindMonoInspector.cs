using UnityEditor;
using UnityEngine;

namespace CodeBind.Editor
{
    [CustomEditor(typeof(ReferenceBindMono))]
    public class ReferenceBindMonoInspector : UnityEditor.Editor
    {
        private SerializedProperty m_BindNames;
        private SerializedProperty m_BindGameObjects;

        private string m_AddBindName;
        private GameObject m_AddBindGameObject;

        private void OnEnable()
        {
            this.m_BindNames = serializedObject.FindProperty("m_BindNames");
            this.m_BindGameObjects = serializedObject.FindProperty("m_BindGameObjects");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Name");
                EditorGUILayout.LabelField("Component");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                this.m_AddBindName = EditorGUILayout.TextField(this.m_AddBindName);
                this.m_AddBindGameObject = (GameObject)EditorGUILayout.ObjectField(this.m_AddBindGameObject, typeof(GameObject), true);
                if (GUILayout.Button("+") && !string.IsNullOrEmpty(this.m_AddBindName))
                {
                    bool isRepeated = false;
                    for (int i = 0; i < this.m_BindNames.arraySize; i++)
                    {
                        string goName = this.m_BindNames.GetArrayElementAtIndex(i).stringValue;
                        if (this.m_AddBindName == goName)
                        {
                            isRepeated = true;
                            this.m_BindNames.GetArrayElementAtIndex(i).stringValue = this.m_AddBindName;
                            this.m_BindGameObjects.GetArrayElementAtIndex(i).objectReferenceValue = this.m_AddBindGameObject;
                            this.m_AddBindName = string.Empty;
                            this.m_AddBindGameObject = null;
                            break;
                        }
                    }

                    if (!isRepeated)
                    {
                        this.m_BindNames.InsertArrayElementAtIndex(0);
                        this.m_BindNames.GetArrayElementAtIndex(0).stringValue = this.m_AddBindName;
                        this.m_BindGameObjects.InsertArrayElementAtIndex(0);
                        this.m_BindGameObjects.GetArrayElementAtIndex(0).objectReferenceValue = this.m_AddBindGameObject;
                        this.m_AddBindName = string.Empty;
                        this.m_AddBindGameObject = null;
                    }

                    serializedObject.ApplyModifiedProperties();
                }

                GUILayout.EndHorizontal();

                for (int i = 0; i < this.m_BindNames.arraySize; i++)
                {
                    GUILayout.BeginHorizontal();
                    string goName = this.m_BindNames.GetArrayElementAtIndex(i).stringValue;
                    EditorGUILayout.TextField(goName);
                    EditorGUILayout.ObjectField(this.m_BindGameObjects.GetArrayElementAtIndex(i).objectReferenceValue, typeof(GameObject), true);
                    if (GUILayout.Button("-"))
                    {
                        this.m_BindNames.DeleteArrayElementAtIndex(i);
                        this.m_BindGameObjects.DeleteArrayElementAtIndex(i);
                        serializedObject.ApplyModifiedProperties();
                    }

                    GUILayout.EndHorizontal();
                }

                if (GUILayout.Button("Clear Serialization"))
                {
                    this.m_BindNames.ClearArray();
                    this.m_BindGameObjects.ClearArray();
                    serializedObject.ApplyModifiedProperties();
                }
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}
