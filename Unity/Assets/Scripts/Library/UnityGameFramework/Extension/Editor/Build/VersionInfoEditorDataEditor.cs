using System;
using System.Collections.Generic;
using System.IO;
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
                EditorGUILayout.HelpBox("????????????????????????!", MessageType.Error);
            }
            EditorGUILayout.PropertyField(m_VersionInfos,true);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_IsGenerateToFullPath);
            bool isValidUri = Utility.Uri.CheckUri(m_VersionInfoEditorData.VersionInfos[m_ActiveIndex.intValue].Value.UpdatePrefixUri);
            if (!m_IsGenerateToFullPath.boolValue)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("??????????????????:", m_OutPath.stringValue);
                if (GUILayout.Button("????????????"))
                {
                    m_OutPath.stringValue = EditorUtility.SaveFilePanel("??????????????????", String.Empty, $"{m_VersionInfoEditorData.VersionInfos[m_ActiveIndex.intValue].Value.Platform}Version", "txt");
                }
                EditorGUILayout.EndHorizontal();
                if (string.IsNullOrEmpty(m_OutPath.stringValue))
                {
                    EditorGUILayout.HelpBox("OutPath is Not Valid!", MessageType.Error);
                }
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                if (GUILayout.Button("??????"))
                {
                    if (m_VersionInfoEditorData.Generate(m_OutPath.stringValue))
                    {
                        EditorUtility.RevealInFinder(m_OutPath.stringValue);
                    }
                }
            }else if(!isValidUri)
            {
                m_IsGenerateToFullPath.boolValue = false;
                EditorUtility.DisplayDialog("??????", "??????VersionInfo--UpdatePrefixUri ???????????????????????????", "??????");
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}