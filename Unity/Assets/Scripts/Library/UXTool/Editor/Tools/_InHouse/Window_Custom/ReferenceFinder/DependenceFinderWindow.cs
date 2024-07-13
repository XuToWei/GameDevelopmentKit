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
    public class DependenceFinderWindow : EditorWindow
    {
        private static DependenceFinderWindow _mWindow;
        private static bool _clickFlag;

        private const int WindowWidth = 1272 + 13 + 12;
        private const int WindowHeight = 636;
        private const int InfoBarHeight = 20;

        private static UnityEngine.Object _selectObj;
        private static readonly DependenceFinderData Data = new DependenceFinderData();

        private static readonly HashSet<string> DependFileExtension = new HashSet<string>()
        {
            ".prefab",
            ".mat",
            ".jpg",
            ".png",
            ".anim",
            ".controller"
        };

        // UI
        private VisualElement _leftContainer;
        private VisualElement _rightContainer;
        private ScrollView _labelScroll;
        private ScrollView _widgetScroll; // 存放依赖资源的内容窗口
        private UXBuilderSlider _slider;
        private VisualElement _infoBar;
        private Image _infoIcon;
        private UXBuilderText _infoText;

        // Data
        public ResourceType filtration = ResourceType.Default; // 当前选择过滤器
        private readonly List<AssetItemBase> _assetsItems = new List<AssetItemBase>();
        private List<AssetItemBase> _filterItems = new List<AssetItemBase>();

        //查找资源引用信息
        [MenuItem("Assets/==查找依赖的资源== (Find Dependence)", priority = -801)]
        private static void FindDepend()
        {
            _mWindow = GetWindow<DependenceFinderWindow>();
            _mWindow.minSize = new Vector2(WindowWidth, WindowHeight);
            _mWindow.titleContent.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_查找依赖资源);
            _selectObj = Selection.activeObject;
            _mWindow.RefreshWindow();
        }

        [MenuItem("Assets/==查找依赖的资源== (Find Dependence)", true)]
        private static bool FindDependValidation()
        {
            var obj = Selection.activeObject;
            if (obj == null) return false;

            var path = AssetDatabase.GetAssetPath(obj);
            return AssetDatabase.GetMainAssetTypeAtPath(path) == typeof(GameObject);
        }

        static DependenceFinderWindow()
        {
            EditorApplication.playModeStateChanged += _ =>
            {
                if (HasOpenInstances<DependenceFinderWindow>())
                    _mWindow = GetWindow<DependenceFinderWindow>();
                if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    if (_mWindow)
                        _mWindow.RefreshWindow();
                }
            };
        }

        [UnityEditor.Callbacks.DidReloadScripts(0)]
        private static void OnScriptReload()
        {
            if (HasOpenInstances<DependenceFinderWindow>())
            {
                _mWindow = GetWindow<DependenceFinderWindow>();
                _mWindow.RefreshWindow();
            }
        }

        private void OnEnable()
        {
            InitWindowUI();
            EditorApplication.delayCall += RefreshWindow;
        }

        #region UI

        private void InitWindowUI()
        {
            VisualElement root = rootVisualElement;
            var row = UXBuilder.Row(root, new UXBuilderRowStruct()
            {
                style = new UXStyle() { height = Length.Percent(100) }
            });

            _leftContainer = UXBuilder.Div(row, new UXBuilderDivStruct()
            {
                style = new UXStyle()
                {
                    borderTopWidth = 10,
                    height = Length.Percent(100),
                    width = 204,
                    backgroundColor = new Color(49f / 255f, 49f / 255f, 49f / 255f)
                }
            });

            _labelScroll = UXBuilder.ScrollView(_leftContainer, new UXBuilderScrollViewStruct());

            _rightContainer = UXBuilder.Div(row, new UXBuilderDivStruct()
            {
                style =
                {
                    height = Length.Percent(100),
                    backgroundColor = new Color(56f / 255f, 56f / 255f, 56f / 255f),
                }
            });

            _rightContainer.RegisterCallback<MouseDownEvent>(_ =>
            {
                if (_clickFlag)
                {
                    _clickFlag = false;
                }
                else
                {
                    foreach (var t in _filterItems)
                    {
                        t.SetSelected(false);
                        ShowResInfo();
                    }
                }
            });

            // 添加右侧资源滚动界面
            {
                if (_widgetScroll != null) _rightContainer.Remove(_widgetScroll);
                _widgetScroll = UXBuilder.ScrollView(_rightContainer, new UXBuilderScrollViewStruct()
                {
                    style = new UXStyle()
                    {
                        marginBottom = 20,
                        whiteSpace = WhiteSpace.NoWrap
                    }
                });
                _widgetScroll.style.position = Position.Relative;
                var ve = _widgetScroll.contentContainer;
                ve.style.flexDirection = FlexDirection.Row;
                ve.style.flexWrap = Wrap.Wrap;
                ve.style.overflow = Overflow.Visible;
                var viewport = _widgetScroll.contentViewport;
                viewport.style.marginLeft = 36;
                viewport.style.marginTop = 36;
            }

            _infoBar = UXBuilder.Div(_rightContainer, new UXBuilderDivStruct()
            {
                style = new UXStyle()
                {
                    position = Position.Absolute,
                    width = Length.Percent(100),
                    height = InfoBarHeight,
                    backgroundColor = new Color(64f / 255f, 64f / 255f, 64f / 255f),
                    flexDirection = FlexDirection.Row,
                    paddingLeft = 3,
                }
            });
            _infoBar.style.bottom = 0;
#if UNITY_2020_3_OR_NEWER
            _slider = UXBuilder.Slider(_infoBar, new UXBuilderSliderStruct()
            {
                style = new UXStyle()
                {
                    position = Position.Absolute,
                    width = 50,
                    right = 20,
                },
                onChange = (x) =>
                {
#if UNITY_2020_3_OR_NEWER
                    if (x / _slider.highValue < 0.2f)
                    {
                        _slider.value = 0;
                    }

                    _widgetScroll.contentContainer.style.flexDirection = _slider.value == 0 ? FlexDirection.Column : FlexDirection.Row;
                    _widgetScroll.contentContainer.style.flexWrap = _slider.value == 0 ? Wrap.NoWrap : Wrap.Wrap;
                    _widgetScroll.contentViewport.style.marginTop = _slider.value == 0 ? 5 : 36;
                    _widgetScroll.contentViewport.style.marginLeft = _slider.value == 0 ? 15 : 36;
                    RefreshRightPrefabContainer();
#endif
                }
            });
            _slider.value = _slider.highValue;
#endif

            root.RegisterCallback<GeometryChangedEvent, KeyValuePair<VisualElement, VisualElement>>(GeometryChangedEvent,
                new KeyValuePair<VisualElement, VisualElement>(_rightContainer, root));
        }

        private void ShowResInfo(Texture iconTexture = null, string filePath = "")
        {
            if (_infoIcon == null)
            {
                _infoIcon = new Image()
                {
                    style =
                    {
                        width = InfoBarHeight,
                        height = InfoBarHeight,
                        bottom = 0,
                    },
                };
                _infoBar.Add(_infoIcon);

                _infoText = UXBuilder.Text(_infoBar, new UXBuilderTextStruct()
                {
                    style =
                    {
                        height = InfoBarHeight,
                        unityTextAlign = TextAnchor.MiddleLeft,
                    },
                });
            }

            if (!iconTexture)
            {
                _infoBar.Remove(_infoIcon);
                _infoIcon = null;
                _infoBar.Remove(_infoText);
                _infoText = null;
            }
            else
            {
                _infoIcon.image = iconTexture;
                _infoText.text = filePath;
            }
        }

        private static void GeometryChangedEvent(GeometryChangedEvent evt, KeyValuePair<VisualElement, VisualElement> pair)
        {
            pair.Key.style.width = pair.Value.resolvedStyle.width - 204;
            pair.Key.UnregisterCallback<GeometryChangedEvent, KeyValuePair<VisualElement, VisualElement>>(GeometryChangedEvent);
        }

        private void RefreshWindow()
        {
            RefreshLeftTypeList();
            if (!_selectObj) return;
            var path = AssetDatabase.GetAssetPath(_selectObj);
            Data.CollectDependenciesInfo(path);
            RefreshRightPrefabContainer();
        }

        private void RefreshLeftTypeList()
        {
            _labelScroll.Clear();

            // 添加左侧分类按钮
            CreateBtn(ResourceType.Default, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_全部类型));
            CreateBtn(ResourceType.Picture, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_图片类型));
            CreateBtn(ResourceType.Prefab, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_Prefab类型));
            CreateBtn(ResourceType.Anim, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_动画类型));
            CreateBtn(ResourceType.Material, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_材质类型));
            return;

            void CreateBtn(ResourceType resourceType, string text)
            {
                var resourceTypeBtn = new UXBuilderButtonStruct
                {
                    className = resourceType.ToString(),
                    type = ButtonType.Default,
                    OnClick = () =>
                    {
                        filtration = resourceType;
                        ShowResInfo();
                        EditorApplication.delayCall += RefreshWindow;
                    },
                    style = new UXStyle
                    {
                        width = 180,
                        height = 35,
                        fontSize = 15,
                        marginLeft = 12,
                        marginTop = 12,
                        backgroundColor =
                            filtration == resourceType ? new Color(36f / 255f, 99f / 255f, 193f / 255f) : Color.white,
                        color =
                            filtration == resourceType ? Color.white : new Color(51f / 255f, 51f / 255f, 51f / 255f),
                    },
                    text = text
                };

                UXBuilder.Button(_labelScroll, resourceTypeBtn);
            }
        }

        private void RefreshRightPrefabContainer()
        {
            InitWindowData();
            _widgetScroll.Clear();

            _filterItems = filtration == ResourceType.Default ? _assetsItems : _assetsItems.Where(item => item.ResourceType == filtration).ToList();

            if (_filterItems.Count == 0) return;
            for (var i = 0; i < _filterItems.Count; i++)
            {
                var tmp = i;
                _filterItems[i].RegisterCallback((MouseDownEvent _) =>
                {
                    foreach (var item in _filterItems)
                        item.SetSelected(false);
                    _filterItems[tmp].SetSelected(true, out var icon, out var filePath);
                    ShowResInfo(icon, filePath);
                    _clickFlag = true;
                });
                _widgetScroll.Add(_filterItems[i]);
            }

            Repaint();
        }

        #endregion

        #region Data

        private void InitWindowData()
        {
            _assetsItems.Clear();
            foreach (var dependencyGuid in Data.Dependencies)
            {
                var path = AssetDatabase.GUIDToAssetPath(dependencyGuid);
                var extLowerStr = Path.GetExtension(path).ToLower();
                if (string.IsNullOrEmpty(path) || !File.Exists(path) || !DependFileExtension.Contains(extLowerStr)) continue;
                var fileInfo = new FileInfo(path);

                AssetItemBase assetItem;
                var res = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
                if (res is GameObject)
                {
#if UNITY_2020_3_OR_NEWER
                    assetItem = new AssetItemPrefabFinder(fileInfo, _slider.value / _slider.highValue);
#else
                    assetItem = new AssetItemPrefabFinder(fileInfo);
#endif
                    _assetsItems.Add(assetItem);
                }
                else if (res is Texture2D)
                {
#if UNITY_2020_3_OR_NEWER
                    assetItem = new AssetItemPicFinder(fileInfo, _slider.value / _slider.highValue);
#else
                    assetItem = new AssetItemPicFinder(fileInfo);
#endif
                    _assetsItems.Add(assetItem);
                }
                else if (res is AnimatorController)
                {
                    var animData = new DependenceFinderData();
                    animData.CollectDependenciesInfo(path);
                    foreach (var childGuid in animData.Dependencies)
                    {
                        var clipPath = AssetDatabase.GUIDToAssetPath(childGuid);
                        var childRes = AssetDatabase.LoadAssetAtPath(clipPath, typeof(UnityEngine.Object));
                        if (!(childRes is AnimationClip)) continue;
#if UNITY_2020_3_OR_NEWER
                        assetItem = new AssetItemAnimFinder(new FileInfo(clipPath), _slider.value / _slider.highValue);
#else
                        assetItem = new AssetItemAnimFinder(new FileInfo(clipPath));
#endif
                        _assetsItems.Add(assetItem);
                    }
                }
                else if (res is AnimationClip)
                {
#if UNITY_2020_3_OR_NEWER
                    assetItem = new AssetItemAnimFinder(fileInfo, _slider.value / _slider.highValue);
#else
                    assetItem = new AssetItemAnimFinder(fileInfo);
#endif
                    _assetsItems.Add(assetItem);
                }
                else if (res is Material)
                {
#if UNITY_2020_3_OR_NEWER
                    assetItem = new AssetItemMatFinder(fileInfo, _slider.value / _slider.highValue);
#else
                    assetItem = new AssetItemMatFinder(fileInfo);
#endif
                    _assetsItems.Add(assetItem);
                }
            }

            // 按类型、名称
            _assetsItems.Sort((item1, item2) =>
            {
                if (item1.ResourceType != item2.ResourceType)
                {
                    return item1.ResourceType.CompareTo(item2.ResourceType);
                }
                else
                {
                    return string.Compare(item1.FileName, item2.FileName, StringComparison.Ordinal);
                }
            });
        }

        #endregion
    }
}
#endif