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
            // 鼠标滚轮横向滚动标签列表（默认滚轮只滚纵向，这里把纵向滚动量转为横向偏移）
            TabsList.RegisterCallback<WheelEvent>((e) =>
            {
                Vector2 offset = TabsList.scrollOffset;
                float maxX = Mathf.Max(0, TabsList.contentContainer.layout.width - TabsList.contentViewport.layout.width);
                offset.x = Mathf.Clamp(offset.x + e.delta.y * 30f, 0, maxX);
                TabsList.scrollOffset = offset;
                // 阻止冒泡到 SceneView，避免滚轮缩放场景
                e.StopPropagation();
            });
            sceneView.rootVisualElement.Add(prefabTabsPanel);

            if (!SwitchSetting.CheckValid(SwitchSetting.SwitchType.PrefabMultiOpen)) return;
            m_tabs = new List<string>(PrefabTabsData.Tabs);
            m_selectedTab = "";
            var prefabStage = PrefabStageUtils.GetCurrentPrefabStage();
            if (prefabStage == null)
            {
                // 已保存的标签可能超过上限，加载后裁剪一次
                EnforceMaxTabs();
                RefreshTabs(false, 0);
                return;
            }
            OpenTab(AssetDatabase.AssetPathToGUID(prefabStage.GetAssetPath()), false);
        }

        public static void RefreshTabs(bool isclose, int width)
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
                    var tab = new PrefabSingleTab(new FileInfo(path), item, m_tabs.Count, isclose, width);
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
                // 新打开的标签插到最左边
                m_tabs.Insert(0, guid);
            }
            // 无论是否新增，都按上限裁剪（保护当前选中），保证已保存的超量标签也能被裁剪
            EnforceMaxTabs();
            width = Math.Min(Math.Max(m_minwidth, (int)sceneView.position.width / m_tabs.Count), m_maxwidth);
            RefreshTabs(false, 0);
        }

        /// <summary>
        /// 限制标签栏中保留的 Prefab 数量，超出上限时移除最早打开（且非当前选中）的标签。
        /// </summary>
        private static void EnforceMaxTabs()
        {
            int max = GetMaxOpenedTabs();
            while (m_tabs.Count > max)
            {
                // 新标签在左边，最旧的在右边（末尾），超限时从末尾移除（保护当前选中）
                int removeIndex = -1;
                for (int i = m_tabs.Count - 1; i >= 0; i--)
                {
                    if (m_tabs[i] != m_selectedTab)
                    {
                        removeIndex = i;
                        break;
                    }
                }
                if (removeIndex < 0) break;
                m_tabs.RemoveAt(removeIndex);
            }
        }

        private static int GetMaxOpenedTabs()
        {
            var data = AssetDatabase.LoadAssetAtPath<UXToolCommonData>(ThunderFireUIToolConfig.UXToolCommonDataPath);
            int max = data != null ? data.MaxOpenedPrefabTabs : 5;
            return max < 1 ? 1 : max;
        }

        private static void FirstLayoutCallback(GeometryChangedEvent evt, VisualElement v)
        {
            // 只在首次布局时滚动到选中标签；触发后立即注销，否则之后每次布局变化都会把滚动位置拽回选中项，
            // 与鼠标滚轮滚动相互拉扯，造成来回小幅度抖动。
            if (evt.currentTarget is VisualElement target)
            {
                target.UnregisterCallback<GeometryChangedEvent, VisualElement>(FirstLayoutCallback);
            }
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
            RefreshTabs(true, width);
        }
    }
}