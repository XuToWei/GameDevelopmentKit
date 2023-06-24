using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityGameFramework.Extension.Editor
{
    [CustomEditor(typeof(VersionInfoEditorData))]
    public class VersionInfoEditorDataEditor : UnityEditor.Editor
    {
        private VersionInfoEditorData m_VersionInfoEditorData;

        private SerializedProperty m_Active;
        private SerializedProperty m_VersionInfos;
        private SerializedProperty m_ActiveIndex;
        private SerializedProperty m_IsGenerateToFullPath;
        private SerializedProperty m_OutPath;
        
        private void OnEnable()
        {
            m_VersionInfoEditorData = target as VersionInfoEditorData;
            m_Active = serializedObject.FindProperty("m_Active");
            m_VersionInfos = serializedObject.FindProperty("m_VersionInfos");
            m_ActiveIndex = serializedObject.FindProperty("m_ActiveIndex");
            m_IsGenerateToFullPath = serializedObject.FindProperty("m_IsGenerateToFullPath");
            m_OutPath = serializedObject.FindProperty("m_OutPath");

            if (!(m_VersionInfoEditorData is null) && (m_VersionInfoEditorData.VersionInfos == null || m_VersionInfoEditorData.VersionInfos.Count == 0))
            {
                m_VersionInfoEditorData.VersionInfos = new List<VersionInfoWrapData>
                    { new VersionInfoWrapData() { Key = "Normal", Value = new VersionInfoData() } };
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            string[] strings = m_VersionInfoEditorData.VersionInfos.Select(_ => _.Key).ToArray();
            if (strings.Length != 0)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Active");
                m_ActiveIndex.intValue = EditorGUILayout.Popup(m_ActiveIndex.intValue,strings);
                m_Active.stringValue = strings[m_ActiveIndex.intValue];
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.LabelField("Active");
                m_Active.stringValue = string.Empty;
                EditorGUILayout.HelpBox("当前没有可用数据!", MessageType.Error);
            }
            EditorGUILayout.PropertyField(m_VersionInfos, true);
            EditorGUILayout.PropertyField(m_IsGenerateToFullPath);

            if (m_VersionInfoEditorData.VersionInfos.Count > 0)
            {
                string updatePrefixUri = m_VersionInfoEditorData.VersionInfos[m_ActiveIndex.intValue].Value.UpdatePrefixUri;
                if (UriUtility.CheckUri(updatePrefixUri))
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("文件生成地址:", m_OutPath.stringValue);
                    if (GUILayout.Button("选择路径"))
                    {
                        m_OutPath.stringValue = EditorUtility.SaveFilePanel("选择生成地址", String.Empty,
                            $"{m_VersionInfoEditorData.VersionInfos[m_ActiveIndex.intValue].Value.Platform}Version",
                            "txt");
                    }

                    EditorGUILayout.EndHorizontal();
                    if (string.IsNullOrEmpty(m_OutPath.stringValue))
                    {
                        EditorGUILayout.HelpBox("OutPath is Not Valid!", MessageType.Error);
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    if (GUILayout.Button("生成"))
                    {
                        if (m_VersionInfoEditorData.Generate(m_OutPath.stringValue))
                        {
                            EditorUtility.RevealInFinder(m_OutPath.stringValue);
                        }
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}