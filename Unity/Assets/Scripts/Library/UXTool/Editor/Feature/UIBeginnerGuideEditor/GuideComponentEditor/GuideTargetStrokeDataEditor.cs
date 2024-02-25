using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ThunderFireUITool
{
    [CustomEditor(typeof(GuideTargetStrokeData))]
    public class GuideTargetStrokeDataEditor : Editor
    {
        SerializedProperty openProperty;
        SerializedProperty strokeTypeProperty;
        SerializedProperty targetTypeProperty;
        SerializedProperty targetGameObjectProperty;
        SerializedProperty playAnimatorProperty;

        // UIBeginnerGuide guide;
        GuideTargetStrokeData data;

        private void OnEnable()
        {
            openProperty = serializedObject.FindProperty("Open");
            strokeTypeProperty = serializedObject.FindProperty("strokeType");
            targetTypeProperty = serializedObject.FindProperty("targetType");
            targetGameObjectProperty = serializedObject.FindProperty("targetGameObject");
            playAnimatorProperty = serializedObject.FindProperty("playAnimator");
            
            data = target as GuideTargetStrokeData;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            
            EditorGUILayout.PropertyField(openProperty, new GUIContent(EditorLocalization.GetLocalization("UIBeginnerGuide","OpenComponent")));
            string[] value = {EditorLocalization.GetLocalization("GuideHighLightData","Square"), EditorLocalization.GetLocalization("GuideHighLightData","Circle")};
            strokeTypeProperty.intValue = Utils.EnumPopupLayoutEx(EditorLocalization.GetLocalization("TargetStroke","StrokeType"), typeof(StrokeType),
                strokeTypeProperty.intValue, value);
            //EditorGUILayout.PropertyField(strokeTypeProperty);
            string[] value1 = {EditorLocalization.GetLocalization("GestureData","auto"), EditorLocalization.GetLocalization("GestureData","select")};
            targetTypeProperty.intValue = Utils.EnumPopupLayoutEx(EditorLocalization.GetLocalization("TargetStroke","TargetType"), typeof(TargetType),
                targetTypeProperty.intValue, value1);
            //EditorGUILayout.PropertyField(targetTypeProperty);

            if (targetTypeProperty.intValue == 1)
            {
                EditorGUILayout.PropertyField(targetGameObjectProperty);
                if (targetGameObjectProperty.objectReferenceValue != null)
                {
                    //data.gameObject.GetComponent<GuideTargetStroke>().transform.SetPositionAndRotation(((GameObject)targetGameObjectProperty.objectReferenceValue).transform.position, ((GameObject)targetGameObjectProperty.objectReferenceValue).transform.rotation);
                    data.gameObject.GetComponent<GuideTargetStroke>().transform.position = ((GameObject)targetGameObjectProperty.objectReferenceValue).transform.position;
                    data.gameObject.GetComponent<GuideTargetStroke>().transform.eulerAngles = ((GameObject)targetGameObjectProperty.objectReferenceValue).transform.eulerAngles;
                    data.gameObject.GetComponent<GuideTargetStroke>().transform.localScale = ((GameObject)targetGameObjectProperty.objectReferenceValue).transform.localScale;
                    data.gameObject.GetComponent<GuideTargetStroke>().transform.GetComponent<RectTransform>().sizeDelta = ((GameObject)targetGameObjectProperty.objectReferenceValue).transform.GetComponent<RectTransform>().sizeDelta;
                }
            }
            EditorGUILayout.PropertyField(playAnimatorProperty, new GUIContent(EditorLocalization.GetLocalization("TargetStroke","PlayAnimator")));
            
            serializedObject.ApplyModifiedProperties();
            
            if (EditorGUI.EndChangeCheck())
            {
                data.gameObject.SetActive(openProperty.boolValue);
                data.GetComponent<GuideTargetStroke>().square.SetActive(strokeTypeProperty.intValue == 0);
                data.GetComponent<GuideTargetStroke>().circle.SetActive(strokeTypeProperty.intValue == 1);
                data.GetComponent<GuideTargetStroke>().square.GetComponent<Animator>().enabled = playAnimatorProperty.boolValue;
                data.GetComponent<GuideTargetStroke>().circle.GetComponent<Animator>().enabled = playAnimatorProperty.boolValue;
            }
        }
    }
}
