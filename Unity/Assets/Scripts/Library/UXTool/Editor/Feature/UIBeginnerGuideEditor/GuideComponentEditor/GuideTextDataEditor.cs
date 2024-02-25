using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ThunderFireUITool
{
    [CustomEditor(typeof(GuideTextData))]
    public class GuideTextDataEditor : Editor
    {
        SerializedProperty openProperty;
        SerializedProperty textBgStyleProperty;
        
        GuideTextData data;
        
        private void OnEnable()
        {
            openProperty = serializedObject.FindProperty("Open");
            textBgStyleProperty = serializedObject.FindProperty("textBgStyle");
            
            data = target as GuideTextData;
            //data.GetComponent<GuideText>().defaultStyle.name = EditorLocalization.GetLocalization("GuideTextData","default");
            //data.GetComponent<GuideText>().withTitleStyle.name =EditorLocalization.GetLocalization("GuideTextData","withTitle");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            
            EditorGUILayout.PropertyField(openProperty, new GUIContent(EditorLocalization.GetLocalization("UIBeginnerGuide","OpenComponent")));
            string[] value1 = {EditorLocalization.GetLocalization("GuideTextData","default"), EditorLocalization.GetLocalization("GuideTextData","withTitle")};
            textBgStyleProperty.intValue = Utils.EnumPopupLayoutEx(EditorLocalization.GetLocalization("GuideTextData","TextBgStyle"), typeof(TextBgStyle),
                textBgStyleProperty.intValue, value1);
            //EditorGUILayout.PropertyField(textBgStyleProperty, new GUIContent(EditorLocalization.GetLocalization("GuideTextData","TextBgStyle")));
            
            serializedObject.ApplyModifiedProperties();
            
            if (EditorGUI.EndChangeCheck())
            {
                data.gameObject.SetActive(openProperty.boolValue);
                data.GetComponent<GuideText>().defaultStyle.SetActive(textBgStyleProperty.intValue == 0);
                data.GetComponent<GuideText>().withTitleStyle.SetActive(textBgStyleProperty.intValue == 1);
            }
        }
    }
}
