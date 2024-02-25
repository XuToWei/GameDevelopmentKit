using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ThunderFireUITool
{
    [CustomEditor(typeof(GuideArrowLine))]
    public class GuideArrowLineEditor : Editor
    {
        private SerializedProperty arrow;
        private SerializedProperty line1;
        private SerializedProperty line2;
        private SerializedProperty linearrow1;
        private SerializedProperty linearrow2;
        private void OnEnable()
        {
            arrow = serializedObject.FindProperty("arrow");
            line1 = serializedObject.FindProperty("oneLine");
            line2 = serializedObject.FindProperty("twoLine");
            linearrow1 = serializedObject.FindProperty("oneLineArrow");
            linearrow2 = serializedObject.FindProperty("twoLineArrow");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            GUI.enabled = false;
            EditorGUILayout.PropertyField(arrow);
            
            EditorGUILayout.PropertyField(line1);
            EditorGUILayout.PropertyField(line2);
            EditorGUILayout.PropertyField(linearrow1);
            EditorGUILayout.PropertyField(linearrow2);
            GUI.enabled = true;
        }
    }
}
