using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(UXMask), true)]
    [CanEditMultipleObjects]
    public class UXMaskEditor : Editor
    {
        SerializedProperty m_ShowMaskGraphic;
        SerializedProperty m_IsReverseMask;
        SerializedProperty m_UseSpriteAlpha;
        protected virtual void OnEnable()
        {
            m_ShowMaskGraphic = serializedObject.FindProperty("m_ShowMaskGraphic");
            m_IsReverseMask = serializedObject.FindProperty("m_IsReverseMask");
            m_UseSpriteAlpha = serializedObject.FindProperty("m_UseSpriteAlpha");
        }
        public override void OnInspectorGUI()
        {
            var graphic = (target as Mask).GetComponent<Graphic>();
            if (graphic && !graphic.IsActive())
                EditorGUILayout.HelpBox("Masking disabled due to Graphic component being disabled.", MessageType.Warning);
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_ShowMaskGraphic);
            var trans = graphic.transform;
            bool hasChildMask = false;
            for (int i = 0;i<trans.childCount;i++){
                var child = trans.GetChild(i);
                if (child.GetComponentInChildren<UXMask>()){
                    hasChildMask = true;
                    break;
                }
            }
            if (hasChildMask){
                EditorGUILayout.HelpBox("Child's IsReverse driven by Child UXMask.", MessageType.Warning);
            }
            EditorGUILayout.PropertyField(m_IsReverseMask);
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