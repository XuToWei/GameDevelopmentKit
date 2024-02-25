using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UXScroller = UnityEngine.UIElements.Scroller;

namespace ThunderFireUITool
{
    public class PrefabTabs
    {
        public static VisualElement prefabTabsPanel;
        private static ScrollView TabsList;
        private static List<string> m_tabs;
        private static string m_selectedTab;
        private static int m_maxwidth = ThunderFireUIToolConfig.m_maxWidth;
        private static int m_minwidth = ThunderFireUIToolConfig.m_minWidth;
        private static SceneView sceneView = SceneView.lastActiveSceneView;
        public static int width;
        public static string SelectedGuid
        {
            get { return m_selectedTab; }
        }
        public static void InitPrefabTabs()
        {
            SceneView sceneView = SceneView.lastActiveSceneView;

            if (sceneView == null) return;

            VisualTreeAsset prefabTabsTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ThunderFireUIToolConfig.UIBuilderPath + "PrefabTabs.uxml");
            prefabTabsPanel = prefabTabsTreeAsset.CloneTree().Children().First();
            TabsList = prefabTabsPanel.Q<ScrollView>("TabsList");
#if UNITY_2021_3_OR_NEWER
            if (!SwitchSetting.CheckValid(SwitchSetting.SwitchType.ResolutionAdjustment))
            {
                prefabTabsPanel.style.right = 0;
            }
#else
            prefabTabsPanel.style.right = 0;
#endif
            prefabTabsPanel.contentContainer.RegisterCallback<MouseEnterEvent>((e) =>
            {
#if UNITY_2021_2_OR_NEWER
                TabsList.horizontalScrollerVisibility = ScrollerVisibility.Auto;
#endif
            });
            prefabTabsPanel.contentContainer.RegisterCallback<MouseLeaveEvent>((e) =>
            {
                Vector2 old = TabsList.scrollOffset;
#if UNITY_2021_2_OR_NEWER
                TabsList.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
#endif
                TabsList.scrollOffset = old;
                width = Math.Min(Math.Max(m_minwidth, (int)sceneView.position.width / m_tabs.Count), m_maxwidth);
                RefreshTabs(false, 0);
            });
            UXScroller scroller = TabsList.horizontalScroller;
            scroller.Remove(scroller.lowButton);
            scroller.Remove(scroller.highButton);
            scroller.slider.style.height = 5;
            scroller.slider.style.marginLeft = scroller.slider.style.marginRight = 0;
            scroller.style.height = 6;
            sceneView.rootVisualElement.Add(prefabTabsPanel);

            if (!SwitchSetting.CheckValid(SwitchSetting.SwitchType.PrefabMultiOpen)) return;
            m_tabs = new List<string>(PrefabTabsData.Tabs);
            m_selectedTab = "";
            var prefabStage = PrefabStageUtils.GetCurrentPrefabStage();
            if (prefabStage == null)
            {
                RefreshTabs(false,0);
                return;
            }
            OpenTab(AssetDatabase.AssetPathToGUID(prefabStage.GetAssetPath()), false);
        }

        public static void RefreshTabs(bool isclose,int width)
        {
            if (prefabTabsPanel == null) return;
#if UNITY_2021_3_OR_NEWER
            prefabTabsPanel.style.top = 0;
#else
            prefabTabsPanel.style.top = string.IsNullOrEmpty(m_selectedTab) ? 21 : 46;
#endif
            bool flag = TabsList.contentContainer.childCount == m_tabs.Count;
            TabsList.Clear();
            Button selectButton = null;
            foreach (var item in m_tabs)
            {
                string path = AssetDatabase.GUIDToAssetPath(item);
                if (path != "" && File.Exists(path))
                {
                    var tab = new PrefabSingleTab(new FileInfo(path), item, m_tabs.Count, isclose , width);
                    if (item == m_selectedTab)
                    {
                        tab.visual.style.backgroundColor = new Color(95f / 255, 95f / 255, 95f / 255, 1);
                        selectButton = tab.visual;
                    }
                    else
                    {
                        tab.visual.style.backgroundColor = new Color(60f / 255, 60f / 255, 60f / 255, 1);
                    }
                    TabsList.Add(tab.visual);
                }
            }
            if (selectButton != null)
            {
                if (flag)
                {
                    selectButton.RegisterCallback<GeometryChangedEvent, VisualElement>(FirstLayoutCallback, selectButton);
                }
                else
                {
                    TabsList.contentContainer.RegisterCallback<GeometryChangedEvent, VisualElement>(FirstLayoutCallback, selectButton);
                }
            }
            PrefabTabsData.SyncTab(m_tabs);
        }

        /// <summary>
        /// 打开prefab
        /// </summary>
        /// <param name="guid">prefab的GUID</param>
        /// <param name="flag">是否需要手动调用打开prefab</param>
        public static void OpenTab(string guid, bool flag)
        {
            m_selectedTab = guid;
            if (flag)
            {
                Utils.OpenPrefab(AssetDatabase.GUIDToAssetPath(guid));
                return;
            }
            if (!m_tabs.Contains(guid))
            {
                m_tabs.Add(guid);
                width = Math.Min(Math.Max(m_minwidth, (int)sceneView.position.width / m_tabs.Count), m_maxwidth);
            }
            RefreshTabs(false,0);
        }

        private static void FirstLayoutCallback(GeometryChangedEvent evt, VisualElement v)
        {
            if (!TabsList.Contains(v)) return;
            TabsList.ScrollTo(v);
        }



        /// <summary>
        /// 关闭prefab
        /// </summary>
        /// <param name="guid">prefab的GUID</param>
        /// <param name="flag">是否需要手动调用返回主场景</param>
        public static void CloseTab(string guid, bool flag)
        {
            if (!flag)
            {
                m_selectedTab = "";
            }
            else
            {
                if (m_selectedTab == guid)
                {
                    StageUtility.GoToMainStage();
                    if (StageUtility.GetMainStageHandle() != StageUtility.GetCurrentStageHandle()) return;
                    m_selectedTab = "";
                }
                m_tabs.Remove(guid);
            }
            RefreshTabs(true,width);
        }
    }
}