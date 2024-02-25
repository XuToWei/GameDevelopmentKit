using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ThunderFireUITool
{
    [CustomEditor(typeof(GuideHighLightData))]
    public class GuideHighLightDataEditor : Editor
    {
        SerializedProperty openProperty;
        SerializedProperty highLightTypeProperty;
        SerializedProperty m_UseCustomTarget;
        SerializedProperty m_target;

        // UIBeginnerGuide guide;
        GuideHighLightData data;

        private void OnEnable()
        {
            openProperty = serializedObject.FindProperty("Open");
            highLightTypeProperty = serializedObject.FindProperty("highLightType");
            m_UseCustomTarget = serializedObject.FindProperty("UseCustomTarget");
            m_target = serializedObject.FindProperty("target");

            data = target as GuideHighLightData;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(openProperty, new GUIContent(EditorLocalization.GetLocalization("UIBeginnerGuide","OpenComponent")));
            string[] value = {EditorLocalization.GetLocalization("GuideHighLightData","Circle"), EditorLocalization.GetLocalization("GuideHighLightData","Square")};
            highLightTypeProperty.intValue = Utils.EnumPopupLayoutEx(EditorLocalization.GetLocalization("GuideHighLightData","HighLightType"), typeof(HighLightType),
                highLightTypeProperty.intValue, value);
            //EditorGUILayout.PropertyField(highLightTypeProperty, new GUIContent(EditorLocalization.GetLocalization("GuideHighLightData","HighLightType")));
            EditorGUILayout.PropertyField(m_UseCustomTarget,new GUIContent(EditorLocalization.GetLocalization("GuideHighLightData","UseCustomTarget")));
            if(m_UseCustomTarget.boolValue==true){
                EditorGUILayout.PropertyField(m_target,new GUIContent(EditorLocalization.GetLocalization("GuideHighLightData","target")));
            }
            else {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(EditorLocalization.GetLocalization("GuideHighLightData","target"), GUILayout.Width(EditorGUIUtility.labelWidth));
                if(GUILayout.Button(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_编辑))){
                    GameObject obj = data.transform.GetChild(0).gameObject;
                    Selection.activeGameObject = obj;
                }
                EditorGUILayout.EndHorizontal();
            }
            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
            {
                data.gameObject.SetActive(openProperty.boolValue);
            }
        }
    }
}
