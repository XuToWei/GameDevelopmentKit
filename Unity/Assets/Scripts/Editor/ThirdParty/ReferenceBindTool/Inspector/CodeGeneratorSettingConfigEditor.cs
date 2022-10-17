using System;
using System.Collections.Generic;
using ReferenceBindTool.Runtime;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace ReferenceBindTool.Editor
{
    [CustomEditor(typeof(CodeGeneratorSettingConfig))]
    public class CodeGeneratorSettingConfigEditor : UnityEditor.Editor
    {
        private ReorderableList m_ReorderableList;
        // private SerializedProperty m_DataListProperty;

        private void OnEnable()
        {
            var dataListProperty = serializedObject.FindProperty("m_Settings");
            m_ReorderableList = new ReorderableList(serializedObject, dataListProperty)
            {
                draggable = false,
                drawElementCallback = DrawElementHandler,
                onAddDropdownCallback = DrawAddPopPanel,
                onCanRemoveCallback = CanRemove,
                elementHeightCallback = ElementHeight,
                drawHeaderCallback = DrawHeader,
            };
        }

        private void DrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect,"Settings");
        }

        private float ElementHeight(int index)
        {
            var element = m_ReorderableList.serializedProperty.GetArrayElementAtIndex(index);
            var h = EditorGUIUtility.singleLineHeight;
            if (element.isExpanded)
                h += EditorGUI.GetPropertyHeight(element);
            return h;
        }

        private bool CanRemove(ReorderableList list)
        {
            return list.index>0;
        }

        private void DrawAddPopPanel(Rect buttonrect, ReorderableList list)
        {
            List<string> names = new List<string>(list.count);
            for (int i = 0; i < list.count; i++)
            {
                names.Add(list.serializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative("m_Name").stringValue);
            }
            AddSettingDataPopWindow.Show(buttonrect,names, value =>
            {
                int index = list.count;
                list.serializedProperty.arraySize++;
                var newElement = list.serializedProperty.GetArrayElementAtIndex(index);
                newElement.FindPropertyRelative("m_Name").stringValue = value;
                newElement.isExpanded = true;
                list.index = index;
                EditorUtility.SetDirty(list.serializedProperty.serializedObject.targetObject);
                list.serializedProperty.serializedObject.ApplyModifiedProperties();
            });
        }

        private void DrawElementHandler(Rect rect, int index, bool isactive, bool isfocused)
        {
            var element = m_ReorderableList.serializedProperty.GetArrayElementAtIndex(index);
            var labelRect = new Rect(rect)
            {
                x = rect.x + 10, //左边距
                height = EditorGUIUtility.singleLineHeight
            };
            element.isExpanded = EditorGUI.Foldout(labelRect, element.isExpanded, 
                $"{element.FindPropertyRelative("m_Name").stringValue}", true);
            if (element.isExpanded)
            {
                var propertyRect = new Rect(rect)
                {
                    x = rect.x + 10,
                    y = rect.y + EditorGUIUtility.singleLineHeight,
                    height = rect.height - EditorGUIUtility.singleLineHeight
                };
                EditorGUI.PropertyField(propertyRect, element, true);
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            m_ReorderableList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}