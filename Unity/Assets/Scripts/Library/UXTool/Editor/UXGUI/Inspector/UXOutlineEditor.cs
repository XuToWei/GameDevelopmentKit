using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(UXOutline), true)]
    [CanEditMultipleObjects]
    public class UXOutlineEditor : Editor
    {
        SerializedProperty m_OutlineMaterial;
        SerializedProperty m_Color;
        SerializedProperty m_Width;
        SerializedProperty m_Expand;
        protected virtual void OnEnable()
        {
            m_OutlineMaterial = serializedObject.FindProperty("outlineMaterial");
            m_Color = serializedObject.FindProperty("m_EffectColor");
            m_Width = serializedObject.FindProperty("m_EffectWidth");
            m_Expand = serializedObject.FindProperty("m_EffectExpand");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_OutlineMaterial);
            EditorGUILayout.PropertyField(m_Color);
            EditorGUILayout.BeginHorizontal();
            Rect position = EditorGUILayout.GetControlRect();
            EditorGUI.LabelField(new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height),"Width");
            float width = (float)m_Width.doubleValue;
            m_Width.doubleValue = EditorGUI.Slider(new Rect(position.x+EditorGUIUtility.labelWidth, position.y, position.width-EditorGUIUtility.labelWidth, position.height),width, 0, 50);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            Rect position2 = EditorGUILayout.GetControlRect();
            EditorGUI.LabelField(new Rect(position2.x, position2.y, EditorGUIUtility.labelWidth, position2.height), "Expand");
            float expand = (float)m_Expand.doubleValue;
            m_Expand.doubleValue = EditorGUI.Slider(new Rect(position2.x+EditorGUIUtility.labelWidth, position2.y, position2.width-EditorGUIUtility.labelWidth, position2.height), expand, 0, 50);
            EditorGUILayout.EndHorizontal();
            // EditorGUI.BeginChangeCheck();
            // EditorGUILayout.PropertyField(m_UseSpriteAlpha);
            // if (EditorGUI.EndChangeCheck())
            // {
            //     // 在这里处理属性值的变化
            //     (target as UXMask).UseSpriteAlpha = m_UseSpriteAlpha.boolValue;
            // }
            serializedObject.ApplyModifiedProperties();
        }
    }

}