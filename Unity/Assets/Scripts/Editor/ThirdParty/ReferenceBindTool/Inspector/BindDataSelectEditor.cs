using System;
using System.Collections.Generic;
using System.Linq;
using ReferenceBindTool.Runtime;
using UnityEditor;
using UnityEngine;

namespace ReferenceBindTool.Editor
{
    [CustomEditor(typeof(BindDataSelect))]
    public class BindDataSelectEditor : UnityEditor.Editor
    {
        /// <summary>
        /// 排除列表
        /// </summary>
        private readonly Type[] m_ExcludeComponentTypes = new[]
        {
            typeof(BindDataSelect),
        };

        private Component[] m_AllComponents;
        private List<Component> m_NotSelectedComponents;
        private SerializedProperty m_BindComponents;
        private BindDataSelect Target => (BindDataSelect) target;

        private List<string> m_NotSelectedComponentTypeNames;
        private int m_Select;
        private bool m_IsShowSelect;

        private void OnEnable()
        {
            serializedObject.Update();
            m_AllComponents = Target.gameObject.GetComponents<Component>();
            m_BindComponents = serializedObject.FindProperty("m_BindComponents");
            if (m_BindComponents.arraySize != 0)
            {
                m_NotSelectedComponents = m_AllComponents.Where(_ => !Target.BindComponents.Contains(_)).ToList();
            }
            else
            {
                m_NotSelectedComponents = new List<Component>(m_AllComponents);
            }

            m_NotSelectedComponents =
                m_NotSelectedComponents.Where(_ => !m_ExcludeComponentTypes.Contains(_.GetType())).ToList();
            m_NotSelectedComponentTypeNames = m_NotSelectedComponents.Select(_ => _.GetType().Name).ToList();
            m_Select = 0;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawSelect();
            EditorGUILayout.Space();

            int needDeleteIndex = -1;
            List<int> needDeleteList = new List<int>();
            for (int i = 0; i < m_BindComponents.arraySize; i++)
            {
                if (m_BindComponents.GetArrayElementAtIndex(i).objectReferenceValue == null)
                {
                    needDeleteList.Add(i);
                    continue;
                }
                EditorGUILayout.BeginHorizontal();
                GUI.enabled = false;
                EditorGUILayout.LabelField("Type：", GUILayout.Width(40));
                EditorGUILayout.LabelField(
                    m_BindComponents.GetArrayElementAtIndex(i).objectReferenceValue.GetType().Name,
                    GUILayout.Width(150));
                EditorGUILayout.ObjectField(m_BindComponents.GetArrayElementAtIndex(i).objectReferenceValue,
                    typeof(Component), true);
                GUI.enabled = true;
                if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20)))
                {
                    //将元素下标添加进删除list
                    needDeleteIndex = i;
                }

                EditorGUILayout.EndHorizontal();
            }
            
            if (needDeleteList.Count > 0)
            {
                for (int i = needDeleteList.Count - 1; i >= 0; i--)
                {
                    m_BindComponents.DeleteArrayElementAtIndex(needDeleteList[i]);
                }
            }

            if (needDeleteIndex != -1)
            {
                Component component =
                    (Component) m_BindComponents.GetArrayElementAtIndex(needDeleteIndex).objectReferenceValue;
                m_BindComponents.DeleteArrayElementAtIndex(needDeleteIndex);
                m_NotSelectedComponents.Add(component);
                m_NotSelectedComponentTypeNames.Add(component.GetType().Name);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawSelect()
        {
            m_IsShowSelect = EditorGUILayout.Foldout(m_IsShowSelect, "选择绑定组件");
            if (!m_IsShowSelect)
            {
                return;
            }

            if (m_NotSelectedComponentTypeNames.Count == 0)
            {
                EditorGUILayout.LabelField("没有可以绑定的组件了。");
                return;
            }

            m_Select = EditorGUILayout.Popup("选择绑定组件", m_Select, m_NotSelectedComponentTypeNames.ToArray());

            if (GUILayout.Button("绑定"))
            {
                int index = m_BindComponents.arraySize;
                m_BindComponents.InsertArrayElementAtIndex(index);
                m_BindComponents.GetArrayElementAtIndex(index).objectReferenceValue = m_NotSelectedComponents[m_Select];
                m_NotSelectedComponents.RemoveAt(m_Select);
                m_NotSelectedComponentTypeNames.RemoveAt(m_Select);
                m_Select = 0;
            }
        }
    }
}