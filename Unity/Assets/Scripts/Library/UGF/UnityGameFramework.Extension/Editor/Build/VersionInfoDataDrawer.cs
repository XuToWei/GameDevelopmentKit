using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityGameFramework.Extension.Editor
{
    [CustomPropertyDrawer(typeof(VersionInfoData))]
    public class VersionInfoDataDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property,
            GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            // VersionInfoData versionInfoData = property.objectReferenceValue as VersionInfoData;
            var rect = new Rect(position.x, position.y - 20, position.width, 18);
            DrawProperty(ref rect, property.FindPropertyRelative("m_ForceUpdateGame"));
            DrawProperty(ref rect, property.FindPropertyRelative("m_LatestGameVersion"));
            DrawProperty(ref rect, property.FindPropertyRelative("m_InternalGameVersion"));
            DrawProperty(ref rect, property.FindPropertyRelative("m_ServerPath"));
            DrawProperty(ref rect, property.FindPropertyRelative("m_ResourceVersion"));
            DrawProperty(ref rect, property.FindPropertyRelative("m_Platform"));

            rect = new Rect(rect.x, rect.y + 20, rect.width, 18);
            string server = property.FindPropertyRelative("m_ServerPath").stringValue;
            string resourceVersion = property.FindPropertyRelative("m_ResourceVersion").stringValue;
            var platform = property.FindPropertyRelative("m_Platform");
            string platformStr = platform.enumNames[platform.enumValueIndex];
            string updatePrefixUri =
                GameFramework.Utility.Path.GetRegularPath(Path.Combine(server, resourceVersion, platformStr));
            EditorGUI.LabelField(rect, "UpdatePrefixUri", updatePrefixUri);

            bool isValidUri = UriUtility.CheckUri(updatePrefixUri);
            if (!isValidUri)
            {
                rect = new Rect(rect.x + 30, rect.y + 20, rect.width, 35);
                EditorGUI.HelpBox(rect, "UpdatePrefixUri is Not Valid!", MessageType.Error);
                rect.y += 20;
                rect.x -= 30;
            }
            
            EditorGUI.BeginDisabledGroup(true);
            DrawProperty(ref rect, property.FindPropertyRelative("m_InternalResourceVersion"));
            DrawProperty(ref rect, property.FindPropertyRelative("m_VersionListLength"));
            DrawProperty(ref rect, property.FindPropertyRelative("m_VersionListHashCode"));
            DrawProperty(ref rect, property.FindPropertyRelative("m_VersionListCompressedLength"));
            DrawProperty(ref rect, property.FindPropertyRelative("m_VersionListCompressedHashCode"));
            EditorGUI.EndDisabledGroup();

            EditorGUI.EndProperty();
        }

        private void DrawProperty(ref Rect position, SerializedProperty property)
        {
            position = new Rect(position.x, position.y + 20, position.width, 18);
            EditorGUI.PropertyField(position, property);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = 300;

            string server = property.FindPropertyRelative("m_ServerPath").stringValue;
            string resourceVersion = property.FindPropertyRelative("m_ResourceVersion").stringValue;
            var platform = property.FindPropertyRelative("m_Platform");
            string platformStr = platform.enumNames[platform.enumValueIndex];
            string updatePrefixUri =
                GameFramework.Utility.Path.GetRegularPath(Path.Combine(server, resourceVersion, platformStr));
            bool isValidUri = UriUtility.CheckUri(updatePrefixUri);
            if (isValidUri)
            {
                height -= 40;
            }

            return height;
        }
    }
}