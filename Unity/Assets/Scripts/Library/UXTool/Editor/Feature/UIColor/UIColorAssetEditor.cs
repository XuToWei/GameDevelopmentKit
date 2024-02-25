using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using ThunderFireUITool;

[CustomEditor(typeof(UIColorAsset))]
public class UIColorAssetEditor : Editor
{
    // Start is called before the first frame update
    ReorderableList reorderableList;
    int activeIndex;
    Color activeColor;
    void OnEnable()
    {
        if (target == null) return;
        activeIndex = -1;
        SerializedProperty prop = serializedObject.FindProperty("defList");
        reorderableList = new ReorderableList(serializedObject, prop, true, true, true, true);

        reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            if (index == activeIndex)
            {
                var element = prop.GetArrayElementAtIndex(index);
                rect.height += 40;
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, rect.width, 20),
                    element.FindPropertyRelative("ColorDefName"), new GUIContent(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_颜色名)));
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y + 20, rect.width, 20),
                    element.FindPropertyRelative("colorValue"), new GUIContent(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_颜色)));
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y + 40, rect.width, 20),
                    element.FindPropertyRelative("ColorComment"), new GUIContent(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_颜色备注)));
            }
            else
            {
                var element = prop.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, rect.width, 20),
                    element.FindPropertyRelative("ColorDefName"), new GUIContent(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_颜色名)));
            }
        };
        reorderableList.elementHeightCallback = (index) =>
        {
            //Debug.Log("ele"+index+""+activeIndex);
            if (index == activeIndex)
            {
                return 60;
            }
            else return 20;
        };
        reorderableList.drawHeaderCallback = (rect) =>
        {
            EditorGUI.LabelField(rect, prop.displayName);
        };
        reorderableList.onSelectCallback = (ReorderableList l) =>
        {
            //Debug.Log("Select"+l.index);
            activeIndex = l.index;
            activeColor = l.serializedProperty.GetArrayElementAtIndex(l.index).FindPropertyRelative("colorValue").colorValue;
        };
        reorderableList.onAddCallback = (ReorderableList l) =>
        {
            var index = l.serializedProperty.arraySize;
            l.serializedProperty.arraySize++;
            l.index = index;
            var element = l.serializedProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("ColorDefName").stringValue = "";
            element.FindPropertyRelative("colorValue").colorValue = Color.white;
            element.FindPropertyRelative("ColorComment").stringValue = "";
        };
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        reorderableList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
    public Color GetCurrentColor()
    {
        return activeColor;
    }
}
