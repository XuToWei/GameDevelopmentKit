#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.UIElements;
using System.IO;
using UnityEngine;
using System;

namespace ThunderFireUITool
{
    public class PrefabSingleTab : VisualElement
    {
        //private static int m_maxCharacters = 20;
        public Button visual;
        private string m_guid;
        private static int m_maxwidth = ThunderFireUIToolConfig.m_maxWidth;
        private static int m_minwidth = ThunderFireUIToolConfig.m_minWidth;
        private static int m_maxcharacters = ThunderFireUIToolConfig.m_maxCharacters;
        private static int m_mincharacters = ThunderFireUIToolConfig.m_minCharacters;

        public PrefabSingleTab(FileInfo info, string guid, int prefabcounts, bool isclose, int width)
        {
            SceneView sceneView = SceneView.lastActiveSceneView;
            int prewidth = (int)sceneView.position.width / prefabcounts;
            VisualTreeAsset tabTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ThunderFireUIToolConfig.UIBuilderPath + "PrefabSingleTab.uxml");
            visual = tabTreeAsset.CloneTree().Q<Button>("Tab");
            Label label = visual.Q<Label>("Label");
            Button close = visual.Q<Button>("Close");
            int nowwidth = m_maxwidth;
            if(isclose)
            {
                label.style.width = width;
                nowwidth = width;
            }
            else
            {
                label.style.width = Math.Min(Math.Max(m_minwidth,prewidth),m_maxwidth);
                nowwidth = Math.Min(Math.Max(m_minwidth, prewidth), m_maxwidth);
            }
            close.style.left = label.style.right;
            int nowCharacterscounts = m_maxcharacters;
            if (nowwidth < m_maxwidth)
                nowCharacterscounts = Math.Max(m_maxcharacters - (int)((m_maxwidth - nowwidth) / 10), m_mincharacters);

            m_guid = guid;
            string fileName = Path.GetFileNameWithoutExtension(info.Name);
            label.text = SetTextWithEllipsis(nowCharacterscounts, fileName);

            close.clickable.activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
            close.clicked += OnClose;
            close.RegisterCallback<MouseEnterEvent, VisualElement>(OnHoverClose, close);
            close.RegisterCallback<MouseLeaveEvent, VisualElement>(OnHoverClose, close);
            visual.RegisterCallback<MouseEnterEvent>(OnHoverChange);
            visual.RegisterCallback<MouseLeaveEvent>(OnHoverChange);
            visual.clickable.activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
            visual.clicked += OnClick;
        }

        private void OnHoverChange(EventBase e)
        {
            if(m_guid == PrefabTabs.SelectedGuid) return;
            if(e.eventTypeId == MouseEnterEvent.TypeId())
            {
                visual.style.backgroundColor = new Color(78f / 255, 78f / 255, 78f / 255, 1);
            }
            else if(e.eventTypeId == MouseLeaveEvent.TypeId())
            {
                visual.style.backgroundColor = new Color(60f / 255, 60f / 255, 60f / 255, 1);
            }
        }
        private void OnHoverClose(EventBase e, VisualElement close)
        {
            if(e.eventTypeId == MouseEnterEvent.TypeId())
            {
                close.style.backgroundColor = new Color(60f / 255, 60f / 255, 60f / 255, 1);
            }
            else if(e.eventTypeId == MouseLeaveEvent.TypeId())
            {
                close.style.backgroundColor = new Color(60f / 255, 60f / 255, 60f / 255, 0);
            }
        }

        private void OnClick()
        {
            PrefabTabs.OpenTab(m_guid, true);
        }
        private void OnClose()
        {
            PrefabTabs.CloseTab(m_guid, true);
        }

        private string SetTextWithEllipsis(int count,string name)
        {
            if(name.Length <= count)
            {
                return name;
            }
            else
            {
                string ans = name.Substring(0, count - 3);
                ans += "...";
                return ans;
            }
        }
    }
}
#endif