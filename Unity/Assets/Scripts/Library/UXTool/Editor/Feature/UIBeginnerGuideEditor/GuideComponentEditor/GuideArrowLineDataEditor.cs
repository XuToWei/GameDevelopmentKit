using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ThunderFireUITool
{
    [CustomEditor(typeof(GuideArrowLineData))]
    public class GuideArrowLineDataEditor : Editor
    {
        private SerializedProperty m_open;
        private SerializedProperty m_lineType;
        private int haslineold;
        private GuideArrowLineData data;
        private void OnEnable()
        {
            m_open = serializedObject.FindProperty("Open");
            m_lineType = serializedObject.FindProperty("lineType");
            haslineold = m_lineType.intValue;
            data = target as GuideArrowLineData;
        }
        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            //只在project窗口中选中时，不显示编辑器
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            
            EditorGUILayout.PropertyField(m_open, new GUIContent(EditorLocalization.GetLocalization("UIBeginnerGuide", "OpenComponent")));
            EditorGUILayout.PropertyField(m_lineType);
            serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                data.gameObject.SetActive(m_open.boolValue);

                if (haslineold != m_lineType.intValue)
                {
                    data.gameObject.GetComponent<GuideArrowLine>().changeAct(m_lineType.intValue,haslineold);
                    haslineold = m_lineType.intValue;
                    //data.transform.GetChild(m_lineType.intValue).gameObject.SetActive(true);
                }
            }
        }
    }
}
