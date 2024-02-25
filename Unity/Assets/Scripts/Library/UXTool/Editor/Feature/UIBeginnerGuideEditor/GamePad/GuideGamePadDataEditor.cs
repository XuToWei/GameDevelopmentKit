using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace ThunderFireUITool
{
    [CustomEditor(typeof(GuideGamePadData))]
    public class GuideGamePadDataEditor : Editor
    {
        private SerializedProperty openProperty;
        private SerializedProperty gamePadAnimStrProperty;
        private SerializedProperty guideListProperty;
        private ReorderableList reorderableList;

        private GuideGamePadData data;

        private void OnEnable()
        {
            openProperty = serializedObject.FindProperty("Open");
            gamePadAnimStrProperty = serializedObject.FindProperty("GamePadAnimStr");
            guideListProperty = serializedObject.FindProperty("guideList");

            reorderableList = new ReorderableList(serializedObject, guideListProperty, true, true, true, true);
            reorderableList.drawHeaderCallback = (Rect rect) =>
            {
                GUI.Label(rect, EditorLocalization.GetLocalization("GuideGamePadData", "GuideList"));
                GUI.Label(new Rect(rect.x + rect.width - 150, rect.y, 150, rect.height), EditorLocalization.GetLocalization("GuideGamePadData", "GamePadTip"));
            };
            reorderableList.elementHeightCallback = index => DrawHeight(guideListProperty, index);
            reorderableList.drawElementCallback =
                (rect, index, isActive, isFocused) =>
                {
                    DrawElement(guideListProperty, rect, index);
                };
            reorderableList.onAddCallback = (ReorderableList l) =>
            {
                var index = l.serializedProperty.arraySize;
                l.serializedProperty.arraySize++;
                l.index = index;
                var element = l.serializedProperty.GetArrayElementAtIndex(index);
                element.FindPropertyRelative("time").floatValue = 0;
                //element.FindPropertyRelative("keys").objectReferenceValue = null;
            };
            data = target as GuideGamePadData;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(openProperty, new GUIContent(EditorLocalization.GetLocalization("UIBeginnerGuide", "OpenComponent")));
            GUI.enabled = false;
            EditorGUILayout.PropertyField(gamePadAnimStrProperty, new GUIContent(EditorLocalization.GetLocalization("GuideGamePadData", "AnimationStr")));
            GUI.enabled = true;
            EditorGUILayout.Space();

            reorderableList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
            {
                data.gameObject.SetActive(openProperty.boolValue);
            }
        }

        private void DrawElement(SerializedProperty prop, Rect rect, int index)
        {
            var element = prop.GetArrayElementAtIndex(index);
            rect.width -= 8;
            rect.x += 8;
            rect.y += 4;
            rect.height -= 4;
            EditorGUI.PropertyField(rect, element);
        }

        private float DrawHeight(SerializedProperty prop, int index)
        {
            var element = prop.GetArrayElementAtIndex(index);

            if (element.FindPropertyRelative("keys").isExpanded)
            {
                int num = data.guideList[index].keys.Count;
                if (num == 0) num = 1;
                return 60 + num * 20f;
            }
            return 25;
        }

        private void OnDisable()
        {
            ReorderableListDrawer.ClearCache();
        }
    }
}
