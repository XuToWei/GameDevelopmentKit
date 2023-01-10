using System;
using UnityEditor;

namespace UnityGameFramework.Extension.Editor
{
    [CustomEditor(typeof(VersionInfoData))]
    public class VersionInfoDataEditor : UnityEditor.Editor
    {
        private SerializedProperty m_ForceUpdateGame;
        private SerializedProperty m_LatestGameVersion;
        private SerializedProperty m_InternalGameVersion;
        private SerializedProperty m_UpdatePrefixUri;
        private SerializedProperty m_InternalResourceVersion;
        private SerializedProperty m_VersionListLength;
        private SerializedProperty m_VersionListHashCode;
        private SerializedProperty m_VersionListCompressedLength;
        private SerializedProperty m_VersionListCompressedHashCode;
        private SerializedProperty m_IsShowCanNotChangeProperty;

        private void OnEnable()
        {
            m_ForceUpdateGame = serializedObject.FindProperty("m_ForceUpdateGame");
            m_LatestGameVersion = serializedObject.FindProperty("m_LatestGameVersion");
            m_InternalGameVersion = serializedObject.FindProperty("m_InternalGameVersion");
            m_UpdatePrefixUri = serializedObject.FindProperty("m_UpdatePrefixUri");
            m_InternalResourceVersion = serializedObject.FindProperty("m_InternalResourceVersion");
            m_VersionListLength = serializedObject.FindProperty("m_VersionListLength");
            m_VersionListHashCode = serializedObject.FindProperty("m_VersionListHashCode");
            m_VersionListCompressedLength = serializedObject.FindProperty("m_VersionListCompressedLength");
            m_VersionListCompressedHashCode = serializedObject.FindProperty("m_VersionListCompressedHashCode");
            m_IsShowCanNotChangeProperty = serializedObject.FindProperty("m_IsShowCanNotChangeProperty");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_ForceUpdateGame);
            EditorGUILayout.PropertyField(m_LatestGameVersion);
            EditorGUILayout.PropertyField(m_InternalGameVersion);

            EditorGUILayout.PropertyField(m_UpdatePrefixUri);
            bool isValidUri = Utility.Uri.CheckUri(m_UpdatePrefixUri.stringValue);
            if (!isValidUri)
            {
                EditorGUILayout.HelpBox("UpdatePrefixUri is Not Valid!", MessageType.Error);
            }
            EditorGUILayout.PropertyField(m_IsShowCanNotChangeProperty);
            if (m_IsShowCanNotChangeProperty.boolValue)
            {
                EditorGUILayout.PropertyField(m_InternalResourceVersion);
                EditorGUILayout.PropertyField(m_VersionListLength);
                EditorGUILayout.PropertyField(m_VersionListHashCode);
                EditorGUILayout.PropertyField(m_VersionListCompressedLength);
                EditorGUILayout.PropertyField(m_VersionListCompressedHashCode);
            }
            serializedObject.ApplyModifiedProperties();

        }
    }
}