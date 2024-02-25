using System;
using System.Collections.Generic;
using ThunderFireUITool;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
// using UnityEngine.InputSystem;
// using UnityEngine.TextCore.Text;

[CustomEditor(typeof(UXBuilderFormSetting))]
public class UXBuilderFormSettingEditor : Editor
{
    ReorderableList reorderableList;
    private UXBuilderFormSetting formSetting;

    private void OnEnable()
    {
        SerializedProperty prop = serializedObject.FindProperty("List");
        reorderableList = new ReorderableList(serializedObject, prop, true, true, true, true);
        
        //设置单个元素的高度
        reorderableList.elementHeightCallback = index => DrawHeight(prop, index);
        // optionsProperties.Clear();
        //绘制单个元素
        reorderableList.drawElementCallback =
            (rect, index, isActive, isFocused) =>
            {
                DrawElement(prop, rect, index);
            };
        
        //头部
        reorderableList.drawHeaderCallback = (rect) =>
            EditorGUI.LabelField(rect, prop.displayName);

    }

    public override void OnInspectorGUI()
    {
        SerializedProperty prop = serializedObject.FindProperty("List");
        // optionsProperties.Clear();
        reorderableList.drawElementCallback =
            (rect, index, isActive, isFocused) =>
            {
                DrawElement(prop, rect, index);
            };
        reorderableList.elementHeightCallback = index => DrawHeight(prop, index);
        serializedObject.Update();
        reorderableList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawElement(SerializedProperty prop, Rect rect, int index)
    {
        formSetting = (UXBuilderFormSetting)target;
        var element = prop.GetArrayElementAtIndex(index);
        // rect.height -= 4;
        rect.width -= 8;
        rect.y += 4;
        rect.x += 8;
        EditorGUI.PropertyField(rect, element);
        formSetting = (UXBuilderFormSetting)target;
        if (formSetting.List[index].componentType == ComponentType.Select)
        {
            EditorGUI.indentLevel++;

            SerializedProperty optionsProperty = element.FindPropertyRelative("options");
            EditorGUI.PropertyField(new Rect(rect) { y = rect.y + 60 }, optionsProperty);

            EditorGUI.indentLevel--;
        }
    }

    private float DrawHeight(SerializedProperty prop, int index)
    {
        formSetting = (UXBuilderFormSetting)target;
        var element = prop.GetArrayElementAtIndex(index);
        if (formSetting.List[index].componentType == ComponentType.Select)
        {
            // rect.height += 30 + formSetting.List[index].options.Count * EditorGUIUtility.singleLineHeight;
            if (element.FindPropertyRelative("options").isExpanded)
            {
                int num = formSetting.List[index].options.Count;
                if (num == 0) num = 1;
                return 130 + num * EditorGUIUtility.singleLineHeight;
            }
            return 100;
        }

        if (formSetting.List[index].componentType == ComponentType.Slider) return 115;
        return 75;
    }
}
