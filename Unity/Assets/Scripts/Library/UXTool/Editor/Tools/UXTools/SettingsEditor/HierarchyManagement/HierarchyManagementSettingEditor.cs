using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace ThunderFireUITool
{
    [CustomEditor(typeof(HierarchyManagementSetting))]
    public class HierarchyManagementSettingEditor : Editor
    {
        //SerializedProperty channelsProperty;
        //SerializedProperty levelProperty;
        SerializedProperty tagColorProperty;

        private ReorderableList reorderableList;
        private void OnEnable()
        {
            //channelsProperty = serializedObject.FindProperty("managementChannelList");
            //levelProperty = serializedObject.FindProperty("managementLevelList");
            tagColorProperty = serializedObject.FindProperty("tagColors");

            reorderableList = new ReorderableList(serializedObject, tagColorProperty, false, true, false, false);
            reorderableList.drawHeaderCallback = (Rect rect) =>
            {
                GUI.Label(rect, "TagColors");
            };
            reorderableList.drawElementCallback =
                (rect, index, isActive, isFocused) =>
                {
                    DrawTagColor(tagColorProperty, rect, index);
                };
        }

        private void DrawTagColor(SerializedProperty prop, Rect rect, int index)
        {
            var element = prop.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, element);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            //EditorGUILayout.PropertyField(channelsProperty);
            //EditorGUILayout.PropertyField(levelProperty);
            reorderableList.DoLayoutList();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                HierarchyManagementEvent.SaveSetting();
            }
            //if (GUILayout.Button("Save"))
            //{
            //    HierarchyManagementSetting setting = HierarchyManagementEvent.hierarchyManagementSetting;
            //    if (setting != null)
            //    {
            //        JsonAssetManager.SaveAssets(setting);
            //    }
            //}
        }
    }

}