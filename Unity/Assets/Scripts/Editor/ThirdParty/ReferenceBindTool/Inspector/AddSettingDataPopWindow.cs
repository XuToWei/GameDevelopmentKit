using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ReferenceBindTool.Editor
{
    public class AddSettingDataPopWindow : PopupWindowContent
    {
        private Action<string> m_AddCallback;
        private List<string> m_ExistNames;
        private string m_Name;
        private bool m_Error;

        public AddSettingDataPopWindow(Action<string> addCallback, List<string> existNames)
        {
            m_AddCallback = addCallback;
            m_ExistNames = existNames;
        }

        public static void Show(Rect rect, List<string> existNames, Action<string> addCallback)
        {
            AddSettingDataPopWindow addSettingDataPopWindow = new AddSettingDataPopWindow(addCallback, existNames);
            rect = new Rect(rect.x + 100, rect.y,rect.width, rect.height);

            PopupWindow.Show(rect, addSettingDataPopWindow);
        }

        public override Vector2 GetWindowSize()
        {
            int helpboxH = m_Error ? 36 : 0;
            return new Vector2(300, 20+EditorGUIUtility.singleLineHeight + helpboxH);
        }

        public override void OnGUI(Rect rect)
        {
            
            EditorGUI.PrefixLabel(new Rect(rect.x, rect.y+10, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Name"));
            m_Name = EditorGUI.TextField(new Rect(rect.x+40
                , rect.y+10, rect.width - 100, EditorGUIUtility.singleLineHeight), m_Name);
            GUI.enabled = !m_Error;
            if (GUI.Button(new Rect(rect.x + rect.width - 60, rect.y+10, 50, EditorGUIUtility.singleLineHeight), "确定"))
            {
                m_AddCallback.Invoke(m_Name);
                EditorWindow.focusedWindow.Close();
            }
            
            GUI.enabled = true;
            if (m_ExistNames.Contains(m_Name))
            {
                EditorWindow.focusedWindow.Repaint();
                EditorGUI.HelpBox(new Rect(rect) { y = rect.y+10 + EditorGUIUtility.singleLineHeight },
                    $"已经存在名为{m_Name}的配置", MessageType.Error);
                m_Error = true;
            }
            else
            {
                m_Error = false;
            }
        }
    }
}