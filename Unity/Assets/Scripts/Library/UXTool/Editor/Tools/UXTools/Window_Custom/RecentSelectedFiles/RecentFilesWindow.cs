#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    public class RecentFilesWindow : EditorWindow
    {
        private static RecentFilesWindow instance;
        public static bool clickFlag = false;

        [MenuItem(ThunderFireUIToolConfig.Menu_RecentlySelected, false, 154)]
        public static void ShowWindow()
        {
            instance = GetWindow<RecentFilesWindow>();
            instance.titleContent.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_最近选中文件); ;
            instance.titleContent.image = ToolUtils.GetIcon("clock_w");
            instance.minSize = new Vector2(600, 350);
        }

        static RecentFilesWindow()
        {
            EditorApplication.playModeStateChanged += (obj) =>
            {
                if (HasOpenInstances<RecentFilesWindow>())
                    instance = GetWindow<RecentFilesWindow>();
                if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    if (instance)
                        instance.RefreshWindow();
                }
            };
        }


        //private int maxRecentSelectedFiles = 15;
        private RecentFilesSetting list;
        private List<string> List = new List<string>();
        private List<AssetItemBase> assetsItems = new List<AssetItemBase>();
        private ScrollView scrollView;
        private UXBuilderSlider slider;
        private VisualElement container;

        private void OnEnable()
        {
            InitWindowUI();
            InitWindowData();
            EditorApplication.delayCall += RefreshWindow;
        }

        private void InitWindowData()
        {
            list = JsonAssetManager.GetAssets<RecentFilesSetting>();

            List = list.List;
            assetsItems.Clear();
            foreach (string guid in List)
            {
                AssetItemBase assetItem;
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var fileInfo = new FileInfo(path);
                var fileObj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
                if (fileObj is GameObject)
                {
#if UNITY_2020_3_OR_NEWER
                    assetItem = new AssetItemPrefabFile(fileInfo, slider.value / slider.highValue);
#else
                    assetItem = new AssetItemPrefabFile(fileInfo);
#endif
                    assetsItems.Add(assetItem);
                }
                else if (fileObj is Texture2D)
                {
#if UNITY_2020_3_OR_NEWER
                    assetItem = new AssetItemPicFile(fileInfo, slider.value / slider.highValue);
#else
                    assetItem = new AssetItemPicFile(fileInfo);
#endif
                    assetsItems.Add(assetItem);
                }
                else if (fileObj is AnimationClip)
                {
#if UNITY_2020_3_OR_NEWER
                    assetItem = new AssetItemAnimFile(fileInfo, slider.value / slider.highValue);
#else
                    assetItem = new AssetItemAnimFile(fileInfo);
#endif
                    assetsItems.Add(assetItem);
                }
                else if (fileObj is Material)
                {
#if UNITY_2020_3_OR_NEWER
                    assetItem = new AssetItemMatFile(fileInfo, slider.value / slider.highValue);
#else
                    assetItem = new AssetItemMatFile(fileInfo);
#endif
                    assetsItems.Add(assetItem);
                }
                else
                {
#if UNITY_2020_3_OR_NEWER
                    assetItem = new AssetItemOthersFile(fileInfo, slider.value / slider.highValue);
#else   
                    assetItem = new AssetItemOthersFile(fileInfo);
#endif                   
                    assetsItems.Add(assetItem);
                }

            }
        }


        private void InitWindowUI()
        {
            VisualElement root = rootVisualElement;
            root.style.paddingBottom = 36;
            root.style.paddingTop = 36;
            root.style.paddingLeft = 36;
            root.style.paddingRight = 36;
            container = UXBuilder.Row(root, new UXBuilderRowStruct()
            {
                style = new UXStyle() { height = Length.Percent(100) }
            });
            ChangeScrollView();

#if UNITY_2020_3_OR_NEWER
            slider = UXBuilder.Slider(container, new UXBuilderSliderStruct()
            {
                style = new UXStyle()
                {
                    position = Position.Absolute,
                    width = 50,
                    right = 20,
                    top = -30
                },
                onChange = OnSliderValueChanged
            });
            slider.value = slider.highValue;
#endif
        }

        public void RefreshWindow()
        {
            InitWindowData();
            scrollView.Clear();
            if (assetsItems.Count == 0) return;
            for (int i = 0; i < assetsItems.Count; i++)
            {
                int tmp = i;

                assetsItems[i].RegisterCallback((MouseDownEvent e) =>
                {
                    for (int j = 0; j < assetsItems.Count; j++)
                        assetsItems[j].SetSelected(false);
                    assetsItems[tmp].SetSelected(true);
                    var fileObj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetsItems[tmp].FilePath);
                    EditorGUIUtility.PingObject(fileObj);
                });
                scrollView.Add(assetsItems[i]);
            }
            Repaint();
        }

        public void OnSliderValueChanged(float x)
        {
#if UNITY_2020_3_OR_NEWER
            if (x / slider.highValue < 0.2f)
            {
                slider.value = 0;
            }
            scrollView.contentContainer.style.flexDirection = slider.value == 0 ? FlexDirection.Column : FlexDirection.Row;
#if UNITY_2021_3_OR_NEWER
            scrollView.mode = slider.value == 0 ? ScrollViewMode.Vertical : ScrollViewMode.Horizontal;
#endif

            ChangeScrollView();
            RefreshWindow();
#endif
        }

        private void ChangeScrollView()
        {
            if (scrollView != null) rootVisualElement.Remove(scrollView);
#if UNITY_2020_3_OR_NEWER
            scrollView = UXBuilder.ScrollView(rootVisualElement, new UXBuilderScrollViewStruct()
            {
                style = new UXStyle()
                {
                    width = Length.Percent(100),
                    marginTop = 30,
                    paddingLeft = 36,
                }
            });
#else
            scrollView = UXBuilder.ScrollView(container, new UXBuilderScrollViewStruct()
            {
                style = new UXStyle() 
                    { 
                        width = Length.Percent(100),
                    }
            });
#endif

            var ve = scrollView.contentContainer;
            ve.style.flexDirection = FlexDirection.Row;
            ve.style.flexWrap = Wrap.Wrap;
            ve.style.overflow = Overflow.Visible;
            scrollView.RegisterCallback<MouseDownEvent>(evt =>
            {
                if (clickFlag)
                {
                    clickFlag = false;
                }
                else
                {
                    for (int i = 0; i < assetsItems.Count; i++)
                    {
                        assetsItems[i].SetSelected(false);
                    }
                }
            });
        }

        [UnityEditor.Callbacks.DidReloadScripts(0)]
        private static void OnScriptReload()
        {
            if (HasOpenInstances<RecentFilesWindow>())
                instance = GetWindow<RecentFilesWindow>();
        }
        public static RecentFilesWindow GetInstance()
        {
            return instance;
        }

    }
}
#endif
