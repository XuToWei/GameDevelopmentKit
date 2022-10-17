using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ReferenceBindTool.Editor
{
    public class SelectComponentTreePopWindow : PopupWindowContent
    {
        private TreeViewState m_TreeViewState;
        private ComponentTreeView m_TreeView;
        private Dictionary<int, bool> m_Select;

        private Action m_OnComplete;
        public override Vector2 GetWindowSize()
        {
            return new Vector2(400, 600);
        }

        public void Show(Transform transform,Dictionary<int, bool> select,Action onComplete)
        {
            m_OnComplete = onComplete;
            m_Select = select;
            if (m_TreeView == null)
            {
                m_TreeViewState = new TreeViewState();
                m_TreeView = new ComponentTreeView(m_TreeViewState,transform,select);
            }
            else
            {
                m_TreeView.Reload(transform,select);
            }
            Rect rect = EditorGUILayout.GetControlRect();
            PopupWindow.Show(new Rect(rect.x - 400, rect.y, rect.width, rect.height), this);
        }


        public override void OnGUI(Rect rect)
        {
            DoToolbar();
            DoTreeView();
        }

        void DoTreeView()
        {
            Rect rect = GUILayoutUtility.GetRect(0, 100000, 0, 100000);
            m_TreeView.OnGUI(rect);
        }

        void DoToolbar()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        public override void OnClose()
        {
            m_OnComplete.Invoke();
            base.OnClose();
        }
    }
}