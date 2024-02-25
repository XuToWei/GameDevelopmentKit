using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace ThunderFireUITool
{
    public class HierarchyManagementWindow : EditorWindow
    {
        private static HierarchyManagementWindow _managementWindow;

        public static string _searchText = "";
        private static List<PrefabDetail> _searchList = new List<PrefabDetail>();
        public static List<string> recentTags = new List<string>();

        public bool _firstFlag = true;
        public bool _refreshFlag = false;

        const int windowWidth = 1200;
        const int windowHeight = 700;



        [MenuItem(ThunderFireUIToolConfig.Menu_HierarchyManage, false, 52)]
        public static void OpenHierarchyManage()
        {
            OpenWindow(false, null);
        }

        public static void OpenWindow(bool isDemo, Action OnSaveAction = null)
        {
            HierarchyManagementEvent.isDemo = isDemo;
            HierarchyManagementEvent.Init();
            HierarchyManagementEvent.OnSave = null;
            if (OnSaveAction != null)
            {
                HierarchyManagementEvent.OnSave += () => { OnSaveAction(); };
            }
            OpenWindow();
        }


        public static void OpenWindow()
        {
            _managementWindow = GetWindow<HierarchyManagementWindow>();
            _managementWindow.minSize = new Vector2(windowWidth, windowHeight);
            _managementWindow.titleContent.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_层级管理);

            UXToolAnalysis.SendUXToolLog(UXToolAnalysisLog.HierarchyManage);
        }

        //[UnityEditor.Callbacks.DidReloadScripts(0)]
        //private static void OnScriptReload()
        //{
        //    if (HasOpenInstances<HierarchyManagementWindow>())
        //    {
        //        _managementWindow = GetWindow<HierarchyManagementWindow>();
        //        HierarchyManagementEvent.Init();
        //    }
        //}

        public static HierarchyManagementWindow GetInstance()
        {
            return _managementWindow;
        }

        private void OnEnable()
        {
            DrawWindowUI();
        }

        #region UI

        public PrefabDetail chosenPrefab = new PrefabDetail() { ID = -1 };
        public ManagementLevel chosenLevel = new ManagementLevel() { ID = -1 };

        private VisualElement _chosenVisualElement;
        private ScrollView _scrollViewContent;
        private ScrollView _scrollView;
        private VisualElement _searchContent;
        private VisualElement _prefabContent;
        private VisualElement _lineContent;
        private VisualElement _buttonContent;
        public VisualElement resultDiv;
        private Label _searchResultText;
        private Label _prefabDrag;
        private const float SpaceWidth = 18f;
        private const float SingleWidth = 120f;
        private const float SinglePrefabHeight = 40f;
        private const float SpaceHeight = 6f;
        private const float BorderWidth = 4f;

        private List<PrefabVisualElement> _prefabVisualElements = new List<PrefabVisualElement>();

        private readonly Color _mSearchBlack = new Color(42 / 255f, 42 / 255f, 42 / 255f);
        private readonly Color _mBorderGrey = new Color(102 / 255f, 102 / 255f, 102 / 255f);
        private readonly Color _mSearchTipWhite = new Color(192 / 255f, 192 / 255f, 192 / 255f);
        private readonly Color _mPrefabBlack = new Color(51 / 255f, 51 / 255f, 51 / 255f);
        private readonly Color _mLineBlue = new Color(94 / 255f, 177 / 255f, 195 / 255f);
        private readonly Color _mRed = new Color(248 / 255f, 97 / 255f, 97 / 255f);
        private readonly Color _mGrey = new Color(196 / 255f, 196 / 255f, 196 / 255f);
        private readonly Color _mYellow = new Color(212 / 255f, 250 / 255f, 104 / 255f);
        private readonly Color _mBlue = new Color(79 / 255f, 213 / 255f, 255 / 255f);
        private readonly Color _mBlack = new Color(40 / 255f, 40 / 255f, 40 / 255f);
        private readonly Color _mLineRed = new Color(235 / 255f, 126 / 255f, 126 / 255f);

        private int _searchIndex = 0;
        public bool _scrollFlag = false;
        public bool _focusFlag = true;
        public bool _focusWindowFlag = true;
        public bool _chosenFlag = true;
        public bool _startDrag = false;

        private Vector2 lastPos = new Vector2(0, 0);

        private void OnDestroy()
        {
            if (HierarchyManagementEvent.CheckNeedSave())
            {
                if (EditorUtility.DisplayDialog("messageBox", EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_检测未保存的数据Tip),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_保存), EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_不保存)))
                {
                    HierarchyManagementEvent.Save();
                }
            }
            HierarchyManagementEvent.isDemo = false;
        }

        private void OnDisable()
        {
            _searchText = "";
            _searchList.Clear();
        }

        //通过flag来进行唯一一次的scrollOffset改变
        private void OnGUI()
        {
            wantsMouseEnterLeaveWindow = true;
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            if (_scrollFlag && _chosenVisualElement != null)
            {
                if (_scrollView.layout.width < _scrollView.contentContainer.layout.width)
                {
                    var mPosition = _chosenVisualElement.worldBound.position;
                    _scrollView.scrollOffset +=
                        new Vector2(mPosition.x - _scrollView.worldBound.x - _scrollView.layout.width / 4, 0);

                    _scrollFlag = false;
                }
                if (_scrollViewContent.layout.height < _scrollViewContent.contentContainer.layout.height)
                {
                    var mPosition = _chosenVisualElement.worldBound.position;
                    _scrollViewContent.scrollOffset +=
                        new Vector2(0, mPosition.y - _scrollViewContent.worldBound.y - _scrollView.layout.height / 8);

                    _scrollFlag = false;
                }
            }

            if (_chosenFlag)
            {
                _chosenFlag = false;
                DrawLines();
            }

            if (Event.current.type == EventType.MouseEnterWindow)
            {
                _focusWindowFlag = true;
                if (DragAndDrop.objectReferences.Length == 1)
                {
                    if (DragAndDrop.objectReferences[0] == null)
                    {
                        _startDrag = true;
                        return;
                    }
                }

                _startDrag = false;
                _firstFlag = true;
                chosenLevel = new ManagementLevel() { ID = -1 };
                DrawAllPrefabs();
            }
#if UNITY_2020_3_OR_NEWER
            if (_managementWindow.hasFocus)
            {
                if (PrefabDetailWindow.GetInstance() != null)
                {
                    PrefabDetailWindow.GetInstance().Focus();
                }
                if (ManagementChannelWindow.GetInstance() != null)
                {
                    ManagementChannelWindow.GetInstance().Focus();
                }
            }
#else
            if (EditorWindow.focusedWindow == _managementWindow)
            {
                if (PrefabDetailWindow.GetInstance() != null)
                {
                    PrefabDetailWindow.GetInstance().Focus();
                }
                if (ManagementChannelWindow.GetInstance() != null)
                {
                    ManagementChannelWindow.GetInstance().Focus();
                }
            }
#endif
            if (_startDrag)
            {
                var currentPos = Event.current.mousePosition;
                _prefabDrag.style.visibility = Visibility.Visible;
                _prefabDrag.style.top = currentPos.y;
                _prefabDrag.style.left = currentPos.x;
            }
            else
            {
                var ve = rootVisualElement.Q<VisualElement>("DragTip");

                if (ve != null)
                    rootVisualElement.Remove(ve);
            }
        }

        private void DrawWindowUI()
        {
            var root = rootVisualElement;
            root.Clear();

            _searchContent = UXBuilder.Div(root, new UXBuilderDivStruct()
            {
                style = new UXStyle()
                {
                    height = 34,
                    width = Length.Percent(100),
                    borderBottomColor = Color.black,
                    borderBottomWidth = 2,
                    paddingTop = 6,
                    paddingLeft = 18,
                    paddingBottom = 6,
                    paddingRight = 18,
                    alignItems = Align.Center,
                }
            });

            var content = UXBuilder.Div(root, new UXBuilderDivStruct()
            {
                style = new UXStyle()
                {
                    // marginLeft = Length.Percent(1), 
                    width = Length.Percent(100),
                }
            });

            // _buttonContent = UXBuilder.Div(root, new UXBuilderDivStruct()
            // {
            //     style = new UXStyle()
            //     {
            //         marginLeft = Length.Percent(5), height = Length.Percent(10), width = Length.Percent(90),
            //     }
            // });
            _scrollView = UXBuilder.ScrollView(content, new UXBuilderScrollViewStruct()
            {
                style = new UXStyle()
                {
                    height = Length.Percent(100),
                    width = Length.Percent(100),
                }
            });
            _scrollView.contentContainer.style.height = Length.Percent(100);

            var rowTop = UXBuilder.Row(_searchContent, new UXBuilderRowStruct()
            {
                align = Align.Center,
                style = new UXStyle()
                {
                    height = Length.Percent(100),
                    width = Length.Percent(100),
                }
            });
            DrawSearch(rowTop);
            DrawTopButtons(rowTop);
            RefreshPaint();
            DrawButton();
        }

        public void RefreshPaint()
        {
            _scrollView.Clear();

            DrawTitles();
            DrawContent();

            _scrollView.contentContainer.style.width =
                _scrollView.contentContainer.ElementAt(1).style.width.value.value + 12;
        }

        private void DrawSearch(VisualElement rowSearch)
        {
            UXBuilder.Text(rowSearch, new UXBuilderTextStruct()
            {
                text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_搜索),
                style = new UXStyle()
                {
                    height = 20,
                    marginRight = 6,
                    fontSize = 14,
                    unityTextAlign = TextAnchor.MiddleCenter,
                    color = Color.white,
                }
            });
            resultDiv = new VisualElement();
            var search = UXBuilder.Input(rowSearch, new UXBuilderInputStruct()
            {
                style = new UXStyle()
                {
                    height = 20,
                    width = 158,
                    marginBottom = Length.Percent(0),
                    marginLeft = Length.Percent(0),
                    marginRight = Length.Percent(0),
                    marginTop = Length.Percent(0),
                    fontSize = 12,
                    color = Color.white,
                },
                onChange = s =>
                {
                    Search(s, resultDiv);
                }
            });
            search.value = _searchText;
            var input = search.Q<VisualElement>("unity-text-input");
            input.style.backgroundColor = _mSearchBlack;
            input.style.borderRightWidth = 0;
            input.style.borderTopWidth = 1;
            input.style.borderBottomWidth = 1;
            input.style.borderLeftWidth = 1;
            input.style.borderTopColor = Color.black;
            input.style.borderBottomColor = Color.black;
            input.style.borderLeftColor = Color.black;
            input.style.borderTopRightRadius = Length.Percent(0);
            input.style.borderBottomRightRadius = Length.Percent(0);
            resultDiv = UXBuilder.Row(rowSearch, new UXBuilderRowStruct()
            {
                align = Align.Center,
                justify = Justify.FlexEnd,
                style = new UXStyle()
                {
                    height = 20,
                    width = 90,
                    backgroundColor = _mSearchBlack,
                    borderTopRightRadius = 3,
                    borderBottomRightRadius = 3,
                    paddingRight = 6,
                    borderRightWidth = 1,
                    borderTopWidth = 1,
                    borderBottomWidth = 1,
                    borderRightColor = Color.black,
                    borderTopColor = Color.black,
                    borderBottomColor = Color.black,
                }
            });
            resultDiv.RegisterCallback<MouseDownEvent>(evt =>
            {
                input.Focus();
            });
            DrawSearchResult(resultDiv);
        }

        private void DrawSearchResult(VisualElement visualElement)
        {
            visualElement.Clear();
            if (!string.IsNullOrEmpty(_searchText))
            {
                if (_searchList.Count != 0)
                {
                    UXBuilder.Button(visualElement, new UXBuilderButtonStruct()
                    {
                        type = ButtonType.Text,
                        text = "<",
                        style = new UXStyle()
                        {
                            fontSize = 12,
                            marginLeft = 6,
                            paddingLeft = Length.Percent(0),
                            paddingRight = Length.Percent(0),
                            color = Color.white,
                            paddingTop = Length.Percent(0),
                            paddingBottom = Length.Percent(0),
                            marginTop = Length.Percent(0),
                            marginBottom = Length.Percent(0),
                        },
                        OnClick = () =>
                        {
                            _searchIndex = _searchIndex == 0 ? _searchList.Count - 1 : _searchIndex - 1;
                            chosenPrefab = _searchList[_searchIndex];
                            _firstFlag = true;
                            DrawAllPrefabs();
                            DrawSearchResult(visualElement);
                            _scrollFlag = true;
                        }
                    });
                }
                _searchResultText = UXBuilder.Text(visualElement, new UXBuilderTextStruct()
                {
                    text = _searchList.Count == 0 ? EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_无结果) : _searchIndex + 1 + " / " + _searchList.Count,
                    style = new UXStyle()
                    {
                        fontSize = 12,
                        unityTextAlign = TextAnchor.MiddleLeft,
                        color = _searchList.Count == 0 ? _mRed : _mSearchTipWhite,
                    }
                });
                if (_searchList.Count != 0)
                {
                    UXBuilder.Button(visualElement, new UXBuilderButtonStruct()
                    {
                        type = ButtonType.Text,
                        text = ">",
                        style = new UXStyle()
                        {
                            fontSize = 12,
                            paddingLeft = Length.Percent(0),
                            paddingRight = Length.Percent(0),
                            color = Color.white,
                            paddingTop = Length.Percent(0),
                            paddingBottom = Length.Percent(0),
                        },
                        OnClick = () =>
                        {
                            _searchIndex = _searchIndex == _searchList.Count - 1 ? 0 : _searchIndex + 1;
                            chosenPrefab = _searchList[_searchIndex];
                            _firstFlag = true;
                            DrawAllPrefabs();
                            DrawSearchResult(visualElement);
                            _scrollFlag = true;
                        }
                    });
                }
            }
        }

        private void DrawTopButtons(VisualElement rowTop)
        {
            var topButtonContent = UXBuilder.Row(rowTop, new UXBuilderRowStruct()
            {
                justify = Justify.FlexEnd,
                align = Align.Center,
                style = new UXStyle()
                {
                    height = Length.Percent(100),
                    width = Length.Percent(40),
                    position = Position.Absolute,
                    right = Length.Percent(0),
                }
            });

            // DrawRangeRefresh(topButtonContent);

            UXBuilder.Button(topButtonContent, new UXBuilderButtonStruct()
            {
                // type = ButtonType.Primary,
                style = new UXStyle()
                {
                    height = 20,
                    width = 78,
                    marginRight = 6,
                    marginLeft = Length.Percent(0)
                },
                text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_添加Prefab),
                disabled = HierarchyManagementEvent._managementLevels.Count == 0,
                OnClick = () =>
                {
                    var level = HierarchyManagementEvent._managementLevels[0];
                    HierarchyManagementEvent.AddNewPrefab(level);
                },
            });

            //添加层级的按钮，暂时不开放，但请勿删除
            /*
             UXBuilder.Button(topButtonContent, new UXBuilderButtonStruct()
            {
                style = new UXStyle() { height = 30, width = 80, marginRight = 5 }, 
                text = "添加层级", OnClick = ManagementLevelWindow.OpenWindow,
            });
            */

            UXBuilder.Button(topButtonContent, new UXBuilderButtonStruct()
            {
                type = ButtonType.Primary,
                status = UXBuilderStatus.Success,
                style = new UXStyle()
                {
                    height = 20,
                    width = 54,
                    marginRight = 6,
                    marginLeft = Length.Percent(0)
                },
                text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_保存),
                OnClick = () =>
                {
                    _refreshFlag = false;
                    _firstFlag = true;

                    HierarchyManagementEvent.Save();
                    RefreshPaint();
                }
            });

            UXBuilder.Button(topButtonContent, new UXBuilderButtonStruct()
            {
                type = ButtonType.Icon,
                status = UXBuilderStatus.Default,
                style = new UXStyle()
                {
                    height = 20,
                    width = 20,
                    marginRight = Length.Percent(0),
                    marginLeft = Length.Percent(0),
                    backgroundImage = (StyleBackground)ToolUtils.GetIcon("ToolBar/setting"),
                },
                OnClick = () =>
                    {
                        HierarchyManagementSettingWindow.ShowWindow();
                    }
            });
        }

        //可视化范围选择的作用，暂时不开放接口，但请勿删除
        private void DrawRangeRefresh(VisualElement topButtonContent)
        {
            var input = UXBuilder.Input(topButtonContent, new UXBuilderInputStruct()
            {
                style = new UXStyle()
                {
                    height = 25,
                    width = 40,
                    marginRight = 5
                },
            });
            input.value = HierarchyManagementEvent._range.ToString();

            UXBuilder.Text(topButtonContent, new UXBuilderTextStruct()
            {
                text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_层级) + "/Channel"
            });

            UXBuilder.Button(topButtonContent, new UXBuilderButtonStruct()
            {
                style = new UXStyle()
                {
                    height = 30,
                    width = 40,
                    marginRight = 5
                },
                type = ButtonType.Outline,
                text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_刷新),
                disabled = HierarchyManagementEvent._managementLevels.Count == 0,
                OnClick = () =>
                {
                    if (int.TryParse(input.value, out var num))
                    {
                        HierarchyManagementEvent.hierarchyManagementSetting.range = num;
                    }
                    DrawWindowUI();
                },
            });
        }
        private void DrawTitles()
        {
            List<ManagementChannel> managementChannels = HierarchyManagementEvent._managementChannels;
            List<ManagementLevel> managementLevels = HierarchyManagementEvent._managementLevels;

            var rowTitle = UXBuilder.Div(_scrollView, new UXBuilderDivStruct()
            {
                className = "rowTitle",
                style = new UXStyle()
                {
                    height = SpaceHeight + SinglePrefabHeight * 0.5f + 48 + BorderWidth,
                    minWidth = Length.Percent(100),
                    // backgroundColor = m_Yellow,
                    flexDirection = FlexDirection.Row,
                    flexWrap = Wrap.NoWrap,
                    // overflow = Overflow.Visible,
                    width = managementChannels.FindAll(s => s.LevelIDList.Count > 0).Count * SpaceWidth +
                            managementLevels.Count * (SingleWidth + SpaceWidth),
                    borderBottomWidth = BorderWidth,
                    borderBottomColor = _mBorderGrey,
                }
            });
            foreach (var channel in HierarchyManagementEvent._managementChannels)
            {
                if (channel.LevelIDList.Count == 0) continue;
                var div = UXBuilder.Div(rowTitle, new UXBuilderDivStruct()
                {
                    style = new UXStyle()
                    {
                        minWidth = SingleWidth + SpaceWidth + SpaceWidth,
                        width = channel.LevelIDList.Count * (SingleWidth + SpaceWidth) + SpaceWidth + BorderWidth,
                        height = Length.Percent(100),
                        borderRightWidth = managementChannels.Count > 0 &&
                                           channel == managementChannels[managementChannels.Count - 1]
                            ? 0
                            : BorderWidth,
                        borderRightColor = _mBorderGrey,
                        justifyContent = Justify.Center,
                        alignItems = Align.Center,
                        paddingBottom = 6
                    }
                });
                var channelName = channel.Name;
                var text = UXBuilder.Text(div, new UXBuilderTextStruct()
                {
                    style = new UXStyle()
                    {
                        width = Length.Percent(80),
                        height = 48,
                        fontSize = 24,
                        unityTextAlign = TextAnchor.MiddleCenter,
                        overflow = Overflow.Hidden,
                        whiteSpace = WhiteSpace.NoWrap,
                        color = Color.white,
                    },
                    text = channelName,
                });
                text.tooltip = channelName;
                UIElementUtils.TextOverflowWithEllipsis(text);

                text.RegisterCallback<MouseDownEvent>(evt =>
                {
                    if (evt.button != 1) return;
                    if (PrefabDetailWindow.GetInstance() != null && PrefabDetailWindow.GetInstance() != null) return;
                    //Right Mouse Button
                    var menu = new GenericMenu();

                    menu.AddItem(new GUIContent(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_修改Channel名称)),
                        false, () =>
                        {
                            ManagementChannelWindow.OpenWindow(channel, text);
                        });
                    menu.ShowAsContext();
                });
                DrawLevelsForSingleChannel(channel, div);
            }
        }

        private void DrawLevelsForSingleChannel(ManagementChannel channel, VisualElement visualElement)
        {
            List<ManagementChannel> managementChannels = HierarchyManagementEvent._managementChannels;
            List<ManagementLevel> managementLevels = HierarchyManagementEvent._managementLevels;
            int range = HierarchyManagementEvent._range;

            var div = UXBuilder.Div(visualElement, new UXBuilderDivStruct()
            {
                style = new UXStyle()
                {
                    width = Length.Percent(100),
                    height = Length.Percent(60),
                    flexDirection = FlexDirection.Row,
                    flexWrap = Wrap.NoWrap,
                    justifyContent = Justify.Center,
                    alignItems = Align.Center,
                }
            });
            foreach (var levelID in channel.LevelIDList)
            {
                var level = managementLevels.Where(t => t.ID == levelID).FirstOrDefault();
                var levelDiv = UXBuilder.Text(div, new UXBuilderTextStruct()
                {
                    style = new UXStyle()
                    {
                        marginLeft = SpaceWidth,
                        width = SingleWidth,
                        backgroundColor = _mRed,
                        marginRight =
                            channel.LevelIDList.Count > 0 &&
                            levelID == channel.LevelIDList[channel.LevelIDList.Count - 1]
                                ? SpaceWidth
                                : 0,
                        height = SinglePrefabHeight * 0.5f,
                        unityTextAlign = TextAnchor.MiddleCenter,
                        fontSize = 16,
                        color = Color.white,
                    },
                    // text = level.Index + "",
                    text = channel.ID * range + channel.LevelIDList.IndexOf(levelID) + "",
                });
                levelDiv.RegisterCallback<MouseDownEvent>(evt =>
                {
                    if (evt.button != 1) return;
                    if (PrefabDetailWindow.GetInstance() != null && PrefabDetailWindow.GetInstance() != null) return;
                    //Right Mouse Button
                    var menu = new GenericMenu();
                    //修改层级信息，暂时不开放，但请勿删除
                    /*
                    menu.AddItem(new GUIContent("修改层级信息"),
                        false, () =>
                        {
                            ManagementLevelWindow.OpenWindow(level);
                        });
                    */
                    menu.AddItem(new GUIContent(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_向前添加层级)),
                        false, () =>
                        {
                            var list = managementChannels.Where(t => t.ID == level.ChannelID).FirstOrDefault().LevelIDList;
                            if (list.IndexOf(level.ID) == 0 && level.Index == level.ChannelID * range)
                                HierarchyManagementEvent.AddLevel(level, level.Index);
                            else if (level.ID == 0 || managementLevels.Where(t => t.ID == level.ID).FirstOrDefault().Index != level.Index - 1)
                                HierarchyManagementEvent.AddLevel(level, level.Index - 1);
                            else HierarchyManagementEvent.AddLevel(level, level.Index);
                        });
                    menu.AddItem(new GUIContent(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_向后添加层级)),
                        false, () =>
                        {
                            var list = managementChannels.Where(t => t.ID == level.ChannelID).FirstOrDefault().LevelIDList;
                            if (list.IndexOf(level.ID) == list.Count - 1 && level.Index + 1 == (level.ChannelID + 1) * range)
                                HierarchyManagementEvent.AddLevel(level, level.Index, true);
                            else HierarchyManagementEvent.AddLevel(level, level.Index + 1, true);
                        });
                    //在指定添加节点，暂时不开放，但请勿删除
                    // menu.AddItem(new GUIContent("向该层级添加节点"),
                    //     false, () =>
                    //     {
                    //         HierarchyManagementEvent.AddNewPrefab(level);
                    //     });
                    menu.AddItem(new GUIContent(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_删除)),
                        false, () =>
                        {
                            HierarchyManagementEvent.DeleteLevel(level);
                        });
                    menu.ShowAsContext();
                });

            }
        }

        private void DrawContent()
        {
            List<ManagementChannel> managementChannels = HierarchyManagementEvent._managementChannels;
            List<ManagementLevel> managementLevels = HierarchyManagementEvent._managementLevels;

            var rowContent = UXBuilder.Row(_scrollView, new UXBuilderRowStruct()
            {
                style = new UXStyle()
                {
                    // height = Length.Percent(100),
                    minWidth = Length.Percent(100),
                    position = Position.Absolute,
                    top = SpaceHeight + SinglePrefabHeight * 0.5f + 48 + BorderWidth,
                    bottom = Length.Percent(0),
                    // backgroundColor = Color.green,
                    // overflow = Overflow.Visible,
                    width = managementChannels.FindAll(s => s.LevelIDList.Count > 0).Count * SpaceWidth +
                            managementLevels.Count * (SingleWidth + SpaceWidth) + 13,
                }
            });
            _scrollViewContent = new ScrollView();
            _scrollViewContent = UXBuilder.ScrollView(rowContent, new UXBuilderScrollViewStruct()
            {
                className = "scrollViewContent",
                style = new UXStyle()
                {
                    height = Length.Percent(100),
                    width = Length.Percent(100),
                },
            });
            _scrollViewContent.contentContainer.style.minHeight = Length.Percent(100);
            DrawAllPrefabs();
        }

        public void DrawAllPrefabs()
        {
            List<ManagementChannel> managementChannels = HierarchyManagementEvent._managementChannels;
            List<ManagementLevel> managementLevels = HierarchyManagementEvent._managementLevels;
            int maxPrefabNum = HierarchyManagementEvent.maxPrefabNum;

            if (!_firstFlag)
            {
                _refreshFlag = true;
            }
            _firstFlag = false;
            _chosenFlag = true;
            _prefabVisualElements.Clear();

            var ve = rootVisualElement.Q<VisualElement>("PrefabTooltip");
            if (ve != null)
            {
                rootVisualElement.Remove(ve);
            }
            _scrollViewContent.Clear();
            _prefabContent = UXBuilder.Div(_scrollViewContent, new UXBuilderDivStruct()
            {
                style = new UXStyle()
                {
                    minWidth = Length.Percent(100),
                    height = maxPrefabNum * (SinglePrefabHeight + SpaceHeight),
                    minHeight = Length.Percent(100),
                    flexDirection = FlexDirection.Row,
                    flexWrap = Wrap.NoWrap,
                }
            });
            foreach (var channel in managementChannels)
            {
                if (channel.LevelIDList.Count == 0) continue;
                var div = UXBuilder.Div(_prefabContent, new UXBuilderDivStruct()
                {
                    style = new UXStyle()
                    {
                        minWidth = SingleWidth + SpaceWidth + SpaceWidth,
                        width = channel.LevelIDList.Count * (SingleWidth + SpaceWidth) + SpaceWidth + BorderWidth,
                        height = Length.Percent(100),
                        borderRightWidth = managementChannels.Count > 0 &&
                                           channel == managementChannels[managementChannels.Count - 1]
                            ? 0
                            : BorderWidth,
                        borderRightColor = _mBorderGrey,
                        flexDirection = FlexDirection.Row,
                        flexWrap = Wrap.NoWrap,
                    }
                });
                foreach (var levelID in channel.LevelIDList)
                {
                    var level = managementLevels.Where(t => t.ID == levelID).FirstOrDefault();
                    var prefabsDiv = UXBuilder.Div(div, new UXBuilderDivStruct()
                    {
                        className = "prefabsDiv",
                        style = new UXStyle()
                        {
                            marginLeft = SpaceWidth,
                            width = SingleWidth,
                            backgroundColor = chosenLevel.ID == level.ID
                                ? new Color(99 / 255f, 99 / 255f, 99 / 255f)
                                : Color.clear,
                            marginRight =
                                channel.LevelIDList.Count > 0 &&
                                levelID == channel.LevelIDList[channel.LevelIDList.Count - 1]
                                    ? SpaceWidth
                                    : 0,
                            height = Length.Percent(100)
                        }
                    });
                    prefabsDiv.RegisterCallback<DragPerformEvent>(evt =>
                    {
                        // Debug.Log(level.ID + " " + chosenPrefab.LevelID);
                        _startDrag = false;
                        var v = rootVisualElement.Q<VisualElement>("DragTip");
                        if (v != null)
                            rootVisualElement.Remove(v);
                        if (level.ID != chosenPrefab.LevelID && DragAndDrop.objectReferences.Length == 1)
                        {
                            if (DragAndDrop.objectReferences[0] == null)
                                HierarchyManagementEvent.DragSubmit(chosenPrefab, level);
                        }
                        else
                        {
                            chosenLevel = new ManagementLevel() { ID = -1 };
                            _firstFlag = true;
                            DrawAllPrefabs();
                        }
                    });
                    prefabsDiv.RegisterCallback<DragEnterEvent>(evt =>
                    {
                        chosenLevel = level;
                        var v = rootVisualElement.Q<VisualElement>("PrefabTooltip");
                        if (v != null)
                            rootVisualElement.Remove(v);
                        _firstFlag = true;
                        DrawAllPrefabs();
                    });

                    prefabsDiv.RegisterCallback<DragLeaveEvent>(evt =>
                    {
                        chosenLevel = new ManagementLevel() { ID = -1 };
                        var v = rootVisualElement.Q<VisualElement>("PrefabTooltip");
                        if (v != null)
                            rootVisualElement.Remove(v);
                        _firstFlag = true;
                        DrawAllPrefabs();
                    });

                    rootVisualElement.RegisterCallback<DragExitedEvent>(evt =>
                    {
                        if ((evt.target as VisualElement)?.name != "prefabsDiv")
                        {
                            // DragAndDrop.PrepareStartDrag();
                            _startDrag = false;
                            _firstFlag = true;
                            chosenLevel = new ManagementLevel() { ID = -1 };
                            DrawAllPrefabs();
                        }
                    });
                    DrawPrefabsForSingle(level, prefabsDiv);
                }
            }
            _lineContent = UXBuilder.Div(_prefabContent, new UXBuilderDivStruct()
            {
                style = new UXStyle() { position = Position.Absolute }
            });
            _lineContent.pickingMode = PickingMode.Ignore;
            _scrollViewContent.contentContainer.style.height =
                _scrollViewContent.contentContainer.ElementAt(0).style.height;
        }

        private void DrawPrefabsForSingle(ManagementLevel level, VisualElement visualElement)
        {
            visualElement.Clear();
            var tagsls = new List<TagDetail>();
            foreach (var item in chosenPrefab.Tags)
                tagsls.Add(item);
            var ind = 0;

            List<PrefabDetail> prefabDetails = HierarchyManagementEvent._prefabDetails;
            List<TagColor> tagColors = HierarchyManagementEvent._mTagColors;

            while (ind != tagsls.Count && tagsls.Count > 0)
            {
                foreach (var item in prefabDetails)
                {
                    if (item.Tags.Contains(tagsls[ind]))
                    {
                        foreach (var it in item.Tags)
                        {
                            if (!tagsls.Contains(it)) tagsls.Add(it);
                        }
                    }
                }
                ind += 1;
            }
            foreach (var prefabDetailID in level.PrefabDetailIDList)
            {
                var flag = prefabDetailID == chosenPrefab.ID;
                var prefabDetail = prefabDetails.Where(t => t.ID == prefabDetailID).FirstOrDefault();
                var color = _mGrey;
                if (!flag)
                    foreach (var tag in tagsls)
                    {
                        if (prefabDetail.Tags.Contains(tag))
                        {
                            color = _mLineBlue;
                            break;
                        }
                    }

                color = _searchList.Find(s => s.ID == prefabDetail.ID) != null ? _mYellow : color;
                var path = Path.GetFileNameWithoutExtension(
                    AssetDatabase.GUIDToAssetPath(prefabDetail.Guid));
                var prefabName = string.IsNullOrEmpty(path) ? prefabDetail.Guid : path;

                // //uielement对于中文的超行省略号存在bug，用以下办法解决
                // var str = prefabName;
                // if (prefabName.Length > 4 && Regex.IsMatch(str.ToString(), @"[\u4e00-\u9fbb]+"))
                // {
                //     str = str.Substring(0, 3) + "...";
                // }
                // else if (prefabName.Length > 10)
                // {
                //     str = str.Substring(0, 8) + "...";
                // }

                var prefabDiv = UXBuilder.Text(visualElement, new UXBuilderTextStruct()
                {
                    style = new UXStyle()
                    {
                        borderBottomWidth = flag ? 3 : 0,
                        borderTopWidth = flag ? 3 : 0,
                        borderLeftWidth = flag ? 3 : 0,
                        borderRightWidth = flag ? 3 : 0,
                        borderBottomColor = flag ? _mBlue : Color.clear,
                        borderTopColor = flag ? _mBlue : Color.clear,
                        borderRightColor = flag ? _mBlue : Color.clear,
                        borderLeftColor = flag ? _mBlue : Color.clear,
                        width = Length.Percent(100),
                        height = SinglePrefabHeight,
                        backgroundColor = color,
                        unityTextAlign = TextAnchor.MiddleCenter,
                        overflow = Overflow.Hidden,
                        whiteSpace = WhiteSpace.NoWrap,
                        fontSize = 16,
                        paddingLeft = Length.Percent(0),
                        paddingRight = Length.Percent(0),
                        marginTop = SpaceHeight,
                        color = _mPrefabBlack,
                    },
                    text = prefabName,
                });
                // var tooltipText = prefabName;
                UIElementUtils.TextOverflowWithEllipsis(prefabDiv);
                // prefabDiv.tooltip = tooltipText + "\n(" + prefabDetail.Guid + ")";
                var ve = new VisualElement();
                var root = rootVisualElement;
                prefabDiv.RegisterCallback<MouseDownEvent>(evt =>
                {
                    if (PrefabDetailWindow.GetInstance() != null && PrefabDetailWindow.GetInstance() != null) return;
                    if (evt.button == 0)
                    {
                        DragAndDrop.PrepareStartDrag();
                        DragAndDrop.StartDrag("prefab");
                        DragAndDrop.objectReferences = new Object[] { null };
                        _startDrag = true;


                        _prefabDrag = UXBuilder.Text(rootVisualElement, new UXBuilderTextStruct()
                        {
                            className = "DragTip",
                            style = new UXStyle()
                            {
                                width = SingleWidth,
                                height = SinglePrefabHeight,
                                backgroundColor = _mGrey,
                                unityTextAlign = TextAnchor.MiddleCenter,
                                overflow = Overflow.Hidden,
                                whiteSpace = WhiteSpace.NoWrap,
                                fontSize = 16,
                                paddingLeft = Length.Percent(0),
                                paddingRight = Length.Percent(0),
                                color = _mPrefabBlack,
                                position = Position.Absolute,
                            },
                            text = prefabName,
                        });
                        UIElementUtils.TextOverflowWithEllipsis(_prefabDrag);
                        _prefabDrag.pickingMode = PickingMode.Ignore;
                        _prefabDrag.style.visibility = Visibility.Hidden;

                        //Left Mouse Button
                        chosenPrefab = prefabDetail;
                        if (!string.IsNullOrEmpty(_searchText))
                        {
                            if (_searchList.Contains(prefabDetail))
                            {
                                _searchIndex = _searchList.IndexOf(prefabDetail);
                                _searchResultText.text = _searchIndex + 1 + " / " + _searchList.Count;
                            }
                            else if (_searchList.Count > 0)
                            {
                                _searchResultText.text = "* / " + _searchList.Count;
                            }
                        }
                        _firstFlag = true;
                        DrawAllPrefabs();
                    }
                    else if (evt.button == 1)
                    {
                        //Right Mouse Button
                        var menu = new GenericMenu();
                        menu.AddItem(new GUIContent(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_修改信息)),
                            false, () =>
                            {
                                chosenPrefab = prefabDetail;
                                _firstFlag = true;
                                DrawAllPrefabs();
                                HierarchyManagementEvent.OpenPrefabDetail(prefabDetail);
                            });
                        menu.AddItem(new GUIContent(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_删除)),
                            false, () =>
                            {
                                HierarchyManagementEvent.DeletePrefab(prefabDetail);

                            });
                        menu.ShowAsContext();
                    }
                });
                prefabDiv.RegisterCallback<MouseEnterEvent>(evt =>
                {
                    if (!_focusFlag) return;
                    var width = 310;
                    var height = 250;
                    var innerVisualElement = (VisualElement)evt.currentTarget;
                    var innerPosition = new Vector2(0, 0);
                    if (innerVisualElement.worldBound.x + innerVisualElement.worldBound.width - root.worldBound.x +
                        width > _managementWindow.position.width)
                    {
                        innerPosition.x = innerVisualElement.worldBound.x - width - root.worldBound.x;
                    }
                    else innerPosition.x = innerVisualElement.worldBound.x + innerVisualElement.worldBound.width - root.worldBound.x;

                    innerPosition.y = innerVisualElement.worldBound.y + innerVisualElement.worldBound.height - root.worldBound.y;
                    ve = UXBuilder.Row(root, new UXBuilderRowStruct()
                    {
                        className = "PrefabTooltip",
                        align = Align.Center,
                        justify = Justify.Center,
                        style = new UXStyle()
                        {
                            // height = height,
                            width = width,
                            flexDirection = FlexDirection.Column,
                            position = Position.Absolute,
                            left = innerPosition.x,
                            backgroundColor = _mBlack,
                            paddingTop = 6,
                            paddingBottom = 6,
                            paddingLeft = 6,
                            paddingRight = 6,
                        }
                    });
                    if (innerVisualElement.worldBound.y + innerVisualElement.worldBound.height - root.worldBound.y +
                        height > _managementWindow.position.height)
                    {
                        ve.style.bottom = _managementWindow.position.height - innerVisualElement.worldBound.y
                                          + root.worldBound.y;
                    }
                    else ve.style.top = innerPosition.y;

                    UXBuilder.Text(ve, new UXBuilderTextStruct()
                    {
                        style = new UXStyle()
                        {
                            alignSelf = Align.Center,
                            fontSize = 18,
                            color = Color.white,
                            width = Length.Percent(100),
                            // unityTextAlign = TextAnchor.MiddleCenter,
                            // overflow = Overflow.Hidden,
                            // textOverflow = TextOverflow.Ellipsis,
                            // whiteSpace = WhiteSpace.NoWrap,
                        },
                        text = prefabName,
                    });
                    var div = UXBuilder.Row(ve, new UXBuilderRowStruct()
                    {
                        align = Align.Center,
                        justify = Justify.FlexStart,
                        style = new UXStyle() { marginTop = 6, }
                    });

                    foreach (var it in prefabDetail.Tags)
                    {
                        var tagName = it.Name;
                        if (tagColors.Find(s => s.Name == tagName) == null)
                            tagColors.Add(new TagColor() { Name = tagName, Color = Color.black, });
                        var tagColor = tagColors.Find(s => s.Name == tagName).Color;
                        var tagPrefab = UXBuilder.Text(div, new UXBuilderTextStruct()
                        {
                            style = new UXStyle()
                            {
                                alignSelf = Align.Center,
                                fontSize = 12,
                                color = Color.white,
                                backgroundColor = tagColor,
                                width = 40,
                                height = 20,
                                unityTextAlign = TextAnchor.MiddleCenter,
                                overflow = Overflow.Hidden,
                                whiteSpace = WhiteSpace.NoWrap,
                                marginRight = 6,
                                marginTop = 6,
                                paddingLeft = 2,
                                paddingRight = 2,
                            },
                            text = tagName,
                        });
                        tagPrefab.tooltip = tagName;
                        UIElementUtils.TextOverflowWithEllipsis(tagPrefab);
                    }

                    UXBuilder.Div(ve, new UXBuilderDivStruct()
                    {
                        style = new UXStyle()
                        {
                            borderBottomColor = Color.white,
                            borderBottomWidth = 1,
                            height = 1,
                            marginTop = 6,
                            marginBottom = 6
                        }
                    });
                    var imageDiv = UXBuilder.Div(ve, new UXBuilderDivStruct()
                    {
                        style = new UXStyle()
                        {
                            height = 160,
                            width = 298,
                            backgroundColor = new Color(82 / 255f, 92 / 255f, 92 / 255f),
                            alignItems = Align.Center,
                            justifyContent = Justify.Center
                        }
                    });
                    var thumbnail = new Image()
                    {
                        style = { width = 160, height = 160 }
                    };
                    // thumbnail.style.position = Position.Absolute;
                    if (File.Exists(AssetDatabase.GUIDToAssetPath(prefabDetail.Guid)))
                    {
                        Texture previewTex = Utils.GetAssetsPreviewTexture(prefabDetail.Guid, 160);
                        thumbnail.image = previewTex;
                        imageDiv.Add(thumbnail);
                    }
                });
                prefabDiv.RegisterCallback<MouseLeaveEvent>(evt =>
                {
                    if (root.Contains(ve))
                        root.Remove(ve);
                });
                if (flag) _chosenVisualElement = prefabDiv;
                DrawTagDetail(prefabDetail, prefabDiv, tagsls);
            }
        }

        private void DrawTagDetail(PrefabDetail prefabDetail, VisualElement visualElement, List<TagDetail> tagsls)
        {
            List<TagColor> tagColors = HierarchyManagementEvent._mTagColors;

            var row = UXBuilder.Row(visualElement, new UXBuilderRowStruct()
            {
                justify = Justify.FlexStart,
                align = Align.FlexEnd,
                style = new UXStyle()
                {
                    height = Length.Percent(100),
                    flexWrap = prefabDetail.Tags.Count < 6 ? Wrap.Wrap : Wrap.WrapReverse
                }
            });
            foreach (var tag in tagsls)
            {
                if (prefabDetail.Tags.Contains(tag))
                {
                    _prefabVisualElements.Add(new PrefabVisualElement()
                    {
                        Prefab = prefabDetail,
                        VisualElement = visualElement,
                    });
                    break;
                }
            }
            foreach (var tag in prefabDetail.Tags)
            {
                if (tagsls.Contains(tag))
                {
                    var t = prefabDetail.Tags.Find(s => s.Equals(tag));
                    if (tagColors.Find(s => s.Name == t.Name) == null)
                    {
                        tagColors.Add(new TagColor()
                        {
                            Color = Color.black,
                            Name = t.Name,
                        });
                    }
                    var color = tagColors.Find(s => s.Name == t.Name).Color;
                    var text = UXBuilder.Text(row, new UXBuilderTextStruct()
                    {
                        text = t.Num.ToString(),
                        style = new UXStyle()
                        {
                            width = 20,
                            height = 10,
                            backgroundColor = color,
                            color = Color.white,
                            fontSize = 10
                        }
                    });
                    text.tooltip = t.Name;
                }
            }
        }

        private void DrawLines()
        {
            for (var i = 0; i < _prefabVisualElements.Count; i++)
            {
                for (var j = i + 1; j < _prefabVisualElements.Count; j++)
                {
                    var prefabI = _prefabVisualElements[i].Prefab;
                    var prefabJ = _prefabVisualElements[j].Prefab;
                    var veI = _prefabVisualElements[i].VisualElement;
                    var veJ = _prefabVisualElements[j].VisualElement;
                    var positionI = new Vector2(veI.worldBound.x - _lineContent.worldBound.x,
                        veI.worldBound.y - _lineContent.worldBound.y);
                    var positionJ = new Vector2(veJ.worldBound.x - _lineContent.worldBound.x,
                        veJ.worldBound.y - _lineContent.worldBound.y);

                    foreach (var tagI in prefabI.Tags)
                    {
                        var tagJ = prefabJ.Tags.Find(s => s.Name == tagI.Name);
                        if (tagJ != null)
                        {
                            if (tagI.Num == tagJ.Num)
                            {
                                var from = positionI + new Vector2(veI.worldBound.width / 2, veI.worldBound.height);
                                var to = positionJ + new Vector2(veJ.worldBound.width / 2, 0);
                                // DrawLineWithSameIndex(from, to);
                                DrawSlantLine(from, to);
                            }
                            if (tagI.Num == tagJ.Num + 1)
                            {
                                var from = positionJ + new Vector2(veJ.worldBound.width, veJ.worldBound.height / 2);
                                var to = positionI + new Vector2(0, veI.worldBound.height / 2);
                                // DrawLineWithDifferentIndex(from, to);
                                DrawSlantLine(from, to);
                            }
                            if (tagI.Num == tagJ.Num - 1)
                            {
                                var from = positionI + new Vector2(veI.worldBound.width, veI.worldBound.height / 2);
                                var to = positionJ + new Vector2(0, veJ.worldBound.height / 2);
                                // DrawLineWithDifferentIndex(from, to);
                                DrawSlantLine(from, to);
                            }
                        }
                    }
                }
            }
        }

        private void DrawLineWithSameIndex(Vector2 from, Vector2 to)
        {
            Vector2 middleFrom = new Vector2(from.x, ((from + to) / 2).y);
            Vector2 middleTo = new Vector2(to.x, ((from + to) / 2).y);
            DrawLineVertical(from, middleFrom);
            if (middleFrom != middleTo)
                DrawLineHorizontal(middleFrom, middleTo);
            DrawLineVertical(middleTo, to);
        }

        private void DrawLineWithDifferentIndex(Vector2 from, Vector2 to)
        {
            Vector2 middleFrom = new Vector2(((from + to) / 2).x, from.y);
            Vector2 middleTo = new Vector2(((from + to) / 2).x, to.y);
            DrawLineHorizontal(from, middleFrom);
            if (middleFrom != middleTo)
                DrawLineVertical(middleFrom, middleTo);
            DrawLineHorizontal(middleTo, to);
        }

        private void DrawLineVertical(Vector2 point1, Vector2 point2)
        {
            var start = point1.y > point2.y ? point2 : point1;
            var end = point1.y > point2.y ? point1 : point2;
            var div = UXBuilder.Div(_lineContent, new UXBuilderDivStruct()
            {
                style = new UXStyle()
                {
                    position = Position.Absolute,
                    left = start.x,
                    top = start.y,
                    height = end.y - start.y,
                    width = 2,
                    backgroundColor = _mLineRed,
                }
            });
            div.pickingMode = PickingMode.Ignore;
        }
        private void DrawLineHorizontal(Vector2 point1, Vector2 point2)
        {
            var start = point1.x > point2.x ? point2 : point1;
            var end = point1.x > point2.x ? point1 : point2;
            var div = UXBuilder.Div(_lineContent, new UXBuilderDivStruct()
            {
                style = new UXStyle()
                {
                    position = Position.Absolute,
                    left = start.x,
                    top = start.y,
                    height = 2,
                    width = end.x - start.x,
                    backgroundColor = _mLineRed,
                }
            });
            div.pickingMode = PickingMode.Ignore;
        }

        private void DrawSlantLine(Vector2 point1, Vector2 point2)
        {
            if (float.IsNaN(point1.x) || float.IsNaN(point1.y) || float.IsNaN(point2.x) || float.IsNaN(point2.x)) return;
            var from = point1.x > point2.x ? point2 : point1;
            var to = point1.x > point2.x ? point1 : point2;
            var x = from.x - to.x;
            var y = from.y - to.y;
            var w = Mathf.Sqrt(x * x + y * y);
            var div = UXBuilder.Div(_lineContent, new UXBuilderDivStruct()
            {
                style = new UXStyle()
                {
                    position = Position.Absolute,
                    left = from.x,
                    top = from.y,
                    height = 2,
                    width = w,
                    backgroundColor = _mLineRed,
                }
            });
            var rotate = 0f;
            if (x == 0) rotate = 90f;
            else rotate = Mathf.Atan(y / x) * Mathf.Rad2Deg;
#if UNITY_2021_3_OR_NEWER
            div.style.top = from.y - y / 2;
            div.style.left = (from.x + to.x) / 2 - w / 2;
            div.style.rotate = new Rotate(rotate);
            // div.style.top = from.y + w / 2 / Mathf.Tan((90 - rotate) * Mathf.Deg2Rad);
#else
            div.transform.rotation = Quaternion.Euler(0, 0, rotate);
#endif
        }


        private void DrawButton()
        {
            //如果页面最下方需要按钮相关将在这里绘制，所以请勿删除
        }

        #endregion

        public static void CloseWindow()
        {
            if (_managementWindow != null)
            {
                _managementWindow.Close();
            }
        }

        public class PrefabVisualElement
        {
            public VisualElement VisualElement;
            public PrefabDetail Prefab;
        }

        public void Search(string s, VisualElement resultDiv)
        {
            _searchText = s;
            _searchList.Clear();
            _searchIndex = 0;
            if (string.IsNullOrEmpty(s))
            {
                // chosenPrefab = new PrefabDetail() { ID = -1 };
                _firstFlag = true;
                DrawSearchResult(resultDiv);
                _firstFlag = true;
                DrawAllPrefabs();
                return;
            }

            List<PrefabDetail> prefabDetails = HierarchyManagementEvent._prefabDetails;
            int delta = Int32.MaxValue;
            foreach (var prefabDetail in prefabDetails)
            {
                var path = Path.GetFileNameWithoutExtension(
                    AssetDatabase.GUIDToAssetPath(prefabDetail.Guid));
                var prefabName = string.IsNullOrEmpty(path) ? prefabDetail.Guid : path;
                if (prefabName.ToLower().Contains(s.ToLower()))
                {
                    _searchList.Add(prefabDetail);
                    if (Mathf.Abs(prefabDetail.LevelID - chosenPrefab.LevelID) < delta)
                    {
                        delta = Mathf.Abs(prefabDetail.LevelID - chosenPrefab.LevelID);
                        _searchIndex = _searchList.Count - 1;
                    }
                }
            }
            _searchList.Sort((detail, prefabDetail) =>
            {
                return detail.LevelID == prefabDetail.LevelID
                    ? detail.ID.CompareTo(prefabDetail.ID)
                    : detail.LevelID.CompareTo(prefabDetail.LevelID);
            });
            var chosenPath = Path.GetFileNameWithoutExtension(
                AssetDatabase.GUIDToAssetPath(chosenPrefab.Guid));
            var n = string.IsNullOrEmpty(chosenPath) ? chosenPrefab.Guid : chosenPath;
            if (!string.IsNullOrEmpty(n) && n.ToLower().Contains(s.ToLower()))
            {
                _searchIndex = _searchList.FindIndex(detail => detail.ID == chosenPrefab.ID);
            }

            if (_searchList.Count > 0 && _searchIndex >= 0 && _searchIndex < _searchList.Count)
            {
                chosenPrefab = _searchList[_searchIndex];
            }

            _firstFlag = true;
            DrawAllPrefabs();
            DrawSearchResult(resultDiv);
            _scrollFlag = true;
        }
    }
}