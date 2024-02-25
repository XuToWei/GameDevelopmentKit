using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    public class PrefabDetailWindow : EditorWindow
    {
        private static PrefabDetailWindow _mWindow;

        private static List<string> _mOptions = new List<string>();
        private static PrefabDetail _mPrefabSubmit = new PrefabDetail();
        private static List<TagDetail> _mTags = new List<TagDetail>();
        private static string _mPath = "";
        private static ManagementLevel _mLevel = new ManagementLevel();
        private static string _mGuid = "";
        private static bool _isNew = true;

        static PrefabDetailWindow()
        {
            EditorApplication.playModeStateChanged += (obj) =>
            {
                if (HasOpenInstances<PrefabDetailWindow>())
                    _mWindow = GetWindow<PrefabDetailWindow>();
                if (EditorApplication.isPaused)
                {
                    return;
                }
                if (!(EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode))
                {
                    return;
                }
                if (_mWindow != null)
                    _mWindow.CloseWindow();
            };
        }

        public static void OpenWindow(PrefabDetail prefabDetail)
        {
            int width = 400;
            int height = 270;
            List<ManagementLevel> _managementLevels = HierarchyManagementEvent._managementLevels;
            List<PrefabDetail> _prefabDetails = new List<PrefabDetail>();
            foreach (var item in HierarchyManagementEvent._prefabDetails)
            {
                _prefabDetails.Add(PrefabDetail.DeepCopyByXml(item));
            }
            // _prefabDetails = HierarchyManagementWindow.GetPrefabDetails();
            _mPrefabSubmit = prefabDetail;
            // _mPrefabDetail = PrefabDetail.DeepCopyByXml(prefabDetail);
            _mLevel = _managementLevels.Where(t => t.ID == prefabDetail.LevelID).FirstOrDefault();
            _mGuid = prefabDetail.Guid;
            _mTags = new List<TagDetail>(prefabDetail.Tags);
            _mPath = AssetDatabase.GUIDToAssetPath(prefabDetail.Guid);
            _isNew = false;
            _mWindow = GetWindow<PrefabDetailWindow>();
            _mWindow.minSize = new Vector2(width, height);
            _mWindow.position = new Rect((Screen.currentResolution.width - width) / 2,
                (Screen.currentResolution.height - height) / 2, width, height);
            _mWindow.titleContent.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_修改节点);
        }
        public static void OpenWindow(ManagementLevel level)
        {
            int width = 400;
            int height = 270;

            _mPrefabSubmit = new PrefabDetail();
            _mLevel = level;
            _mGuid = "";
            _mPath = "";
            _mTags = new List<TagDetail>();
            _isNew = true;
            _mWindow = GetWindow<PrefabDetailWindow>();
            _mWindow.minSize = new Vector2(width, height);
            _mWindow.position = new Rect((Screen.currentResolution.width - width) / 2,
                (Screen.currentResolution.height - height) / 2, width, height);
            _mWindow.titleContent.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_新增节点);
        }

        private void OnDisable()
        {
            _mPrefabSubmit = new PrefabDetail();
            _mTags = new List<TagDetail>();
            _isNew = true;
            HierarchyManagementWindow.GetInstance()._focusFlag = true;
        }

        [UnityEditor.Callbacks.DidReloadScripts(0)]
        private static void OnScriptReload()
        {
            if (HasOpenInstances<PrefabDetailWindow>())
                _mWindow = GetWindow<PrefabDetailWindow>();
        }

        public static PrefabDetailWindow GetInstance()
        {
            return _mWindow;
        }

        private void OnEnable()
        {
            if (HierarchyManagementWindow.GetInstance() != null)
                HierarchyManagementWindow.GetInstance()._focusFlag = false;
            InitWindowData();
            DrawWindowUI();
        }

        private void InitWindowData()
        {

            int _range = HierarchyManagementEvent._range;
            List<ManagementChannel> _managementChannels = HierarchyManagementEvent._managementChannels;
            List<ManagementLevel> _managementLevels = HierarchyManagementEvent._managementLevels;
            _mOptions.Clear();
            foreach (var level in _managementLevels)
            {
                _mOptions.Add(level.ChannelID * _range +
                              _managementChannels.Where(t => t.ID == level.ChannelID).FirstOrDefault().LevelIDList.IndexOf(level.ID) + "");
            }
        }

        #region UI

        private UXBuilderInput _tagInput;
        private void DrawWindowUI()
        {
            var root = rootVisualElement;
            var content = UXBuilder.Div(root, new UXBuilderDivStruct()
            {
                style = new UXStyle()
                {
                    paddingTop = 20,
                    paddingBottom = 20,
                    paddingLeft = 20,
                    paddingRight = 20,
                    justifyContent = Justify.Center
                }
            });
            UXBuilderRow row;
            UXBuilderCol col;

            //文件选择
            #region FileSelect
            row = UXBuilder.Row(content, new UXBuilderRowStruct() { style = new UXStyle() { marginBottom = 10 } });
            col = UXBuilder.Col(row, new UXBuilderColStruct()
            {
                span = 4,
                style = new UXStyle() { marginRight = 20 }
            });
            UXBuilder.Text(col, new UXBuilderTextStruct()
            {
                text = "Prefab",
                style = new UXStyle() { unityTextAlign = TextAnchor.UpperRight }
            });
            col = UXBuilder.Col(row, new UXBuilderColStruct()
            {
                span = 16
            });
            var fileSelect = UXBuilder.Upload(col, new UXBuilderPathUploadStruct()
            {
                style = new UXStyle() { width = Length.Percent(100) },
                type = UXUploadType.File,
                inputStyle = new UXStyle() { width = Length.Percent(85) },
                openPath = _mPath,
                onChange = s =>
                {
                    _mGuid = AssetDatabase.AssetPathToGUID(s);
                }
            });
            #endregion

            //层级选择
            #region LevelSelect
            row = UXBuilder.Row(content, new UXBuilderRowStruct() { style = new UXStyle() { marginBottom = 10 } });
            col = UXBuilder.Col(row, new UXBuilderColStruct()
            {
                span = 4,
                style = new UXStyle() { marginRight = 20 }
            });
            UXBuilder.Text(col, new UXBuilderTextStruct()
            {
                text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_层级),
                style = new UXStyle() { unityTextAlign = TextAnchor.UpperRight }
            });
            col = UXBuilder.Col(row, new UXBuilderColStruct()
            {
                span = 16
            });
            UXBuilder.Select(col, new UXBuilderSelectStruct()
            {
                style = new UXStyle() { width = Length.Percent(85) },
                options = _mOptions,
                defaultValue = _mLevel != null
                    ? _mLevel.ChannelID * HierarchyManagementEvent._range +
                      HierarchyManagementEvent._managementChannels.Where(t => t.ID == _mLevel.ChannelID).FirstOrDefault().LevelIDList.IndexOf(_mLevel.ID) + ""
                    : "",
                onChange = s =>
                {
                    foreach (var level in HierarchyManagementEvent._managementLevels)
                    {
                        if (level.ChannelID * HierarchyManagementEvent._range +
                            HierarchyManagementEvent._managementChannels.Where(t => t.ID == level.ChannelID).FirstOrDefault().LevelIDList.IndexOf(level.ID) + "" == s)
                        {
                            _mLevel = level;
                        }
                    }
                }
            });
            #endregion

            //标签展示
            #region TagsShow

            row = UXBuilder.Row(content,
                new UXBuilderRowStruct()
                { style = new UXStyle() { marginBottom = 5, height = 60 }, align = Align.FlexStart });
            col = UXBuilder.Col(row, new UXBuilderColStruct()
            {
                span = 4,
                style = new UXStyle() { marginRight = 20 }
            });
            UXBuilder.Text(col, new UXBuilderTextStruct()
            {
                text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_标签),
                style = new UXStyle() { unityTextAlign = TextAnchor.UpperRight }
            });
            col = UXBuilder.Col(row, new UXBuilderColStruct()
            {
                span = 16,
                style = new UXStyle() { height = Length.Percent(100), flexDirection = FlexDirection.Row }
            });
            var tagContent = UXBuilder.Div(col, new UXBuilderDivStruct()
            {
                style = new UXStyle()
                {
                    width = Length.Percent(85),
                    borderTopColor = Color.gray,
                    borderBottomColor = Color.gray,
                    borderRightColor = Color.gray,
                    borderLeftColor = Color.gray,
                    borderBottomWidth = 1,
                    borderTopWidth = 1,
                    borderRightWidth = 1,
                    borderLeftWidth = 1,
                    marginLeft = 3,
                },
            });
            DrawTags(tagContent);

            var recentTagsRow = UXBuilder.Row(content,
                new UXBuilderRowStruct() { style = new UXStyle() { marginBottom = 5 } });
            DrawRecentTags(recentTagsRow, tagContent);

            row = UXBuilder.Row(content, new UXBuilderRowStruct() { style = new UXStyle() { marginBottom = 5 } });
            UXBuilder.Col(row, new UXBuilderColStruct()
            {
                span = 4,
                style = new UXStyle() { marginRight = 20 }
            });
            col = UXBuilder.Col(row, new UXBuilderColStruct()
            {
                span = 16,
                style = new UXStyle() { flexDirection = FlexDirection.Row }
            });
            _tagInput = UXBuilder.Input(col, new UXBuilderInputStruct()
            {
                style = new UXStyle() { width = Length.Percent(85), marginLeft = 0 },
            });
            UXBuilder.Button(col, new UXBuilderButtonStruct()
            {
                text = "+",
                style = new UXStyle() { width = 30 },
                OnClick = () =>
                {
                    var tag = AddTag(tagContent, _tagInput.text);
                    _tagInput.value = "";
                    if (tag == null) return;

                    HierarchyManagementWindow.recentTags.Insert(0, tag.Name);
                    while (HierarchyManagementWindow.recentTags.Count > 2)
                    {
                        HierarchyManagementWindow.recentTags.RemoveAt(2);
                    }
                    DrawRecentTags(recentTagsRow, tagContent);
                }
            });
            #endregion

            //按钮
            #region Button

            row = UXBuilder.Row(content, new UXBuilderRowStruct()
            {
                justify = Justify.Center,
                align = Align.Center,
                style = new UXStyle()
                {
                    marginTop = 10,
                }
            });
            UXBuilder.Button(row, new UXBuilderButtonStruct()
            {
                type = ButtonType.Primary,
                text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定),
                OnClick = Submit,
                style = new UXStyle() { width = 75, height = 25, fontSize = 14 }
            });
            UXBuilder.Button(row, new UXBuilderButtonStruct()
            {
                text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消),
                OnClick = CloseWindow,
                style = new UXStyle() { width = 75, height = 25, fontSize = 14, marginLeft = 20 }
            });

            #endregion
        }

        private void DrawTags(VisualElement visualElement)
        {
            visualElement.Clear();
            var scrollView = UXBuilder.ScrollView(visualElement, new UXBuilderScrollViewStruct());
            var row = UXBuilder.Row(scrollView, new UXBuilderRowStruct());
            row.style.alignItems = Align.FlexStart;
            foreach (var tag in _mTags)
            {
                var div = UXBuilder.Div(row, new UXBuilderDivStruct()
                {
                    style = new UXStyle()
                    {
                        width = 90,
                        height = 20,
                        backgroundColor = Color.gray,
                        marginBottom = 2,
                        marginLeft = 2,
                        marginRight = 2,
                        marginTop = 2,
                        alignItems = Align.Center,
                        justifyContent = Justify.Center,
                        flexDirection = FlexDirection.Row,
                    }
                });
                var name = tag.Name;
                var text = UXBuilder.Text(div, new UXBuilderTextStruct()
                {
                    text = name,
                    style = new UXStyle()
                    {
                        overflow = Overflow.Hidden,
                        whiteSpace = WhiteSpace.NoWrap,
                        unityTextAlign = TextAnchor.MiddleCenter,
                        height = Length.Percent(100),
                        width = 55,
                        backgroundColor = Color.gray,
                        marginBottom = 2,
                        marginLeft = 2,
                        marginRight = 2,
                        marginTop = 2,
                    }
                });
                text.tooltip = name;
                UIElementUtils.TextOverflowWithEllipsis(text);
                var deleteButton = UXBuilder.Text(div, new UXBuilderTextStruct()
                {
                    text = "X",
                    style = new UXStyle()
                    {
                        width = 15,
                        height = Length.Percent(100),
                        unityTextAlign = TextAnchor.MiddleCenter,
                        fontSize = 10,
                    },
                });
                deleteButton.RegisterCallback<MouseDownEvent>(evt =>
                {
                    DeleteTag(tag, visualElement);
                });
                text.tooltip = tag.Name;
            }
        }

        private void DrawRecentTags(VisualElement visualElement, VisualElement tagContent)
        {
            visualElement.Clear();
            if (HierarchyManagementWindow.recentTags.Count == 0) return;

            var col = UXBuilder.Col(visualElement, new UXBuilderColStruct()
            {
                span = 4,
                style = new UXStyle() { marginRight = 20 }
            });
            col = UXBuilder.Col(visualElement, new UXBuilderColStruct()
            {
                span = 16,
                style = new UXStyle() { flexDirection = FlexDirection.Row }
            });
            var recentTagsContent = UXBuilder.Div(col, new UXBuilderDivStruct()
            {
                style = new UXStyle()
                {
                    width = Length.Percent(85),
                    marginLeft = 3,
                },
            });
            var row = UXBuilder.Row(recentTagsContent, new UXBuilderRowStruct());
            row.style.alignItems = Align.FlexStart;
            foreach (var tagName in HierarchyManagementWindow.recentTags)
            {
                var div = UXBuilder.Div(row, new UXBuilderDivStruct()
                {
                    style = new UXStyle()
                    {
                        width = 90,
                        height = 20,
                        backgroundColor = Color.gray,
                        marginBottom = 2,
                        marginLeft = 2,
                        marginRight = 2,
                        marginTop = 2,
                        alignItems = Align.Center,
                        justifyContent = Justify.Center,
                        flexDirection = FlexDirection.Row,
                    }
                });
                var name = tagName;
                var text = UXBuilder.Text(div, new UXBuilderTextStruct()
                {
                    text = name,
                    style = new UXStyle()
                    {
                        overflow = Overflow.Hidden,
                        whiteSpace = WhiteSpace.NoWrap,
                        unityTextAlign = TextAnchor.MiddleCenter,
                        height = Length.Percent(100),
                        width = 55,
                        backgroundColor = Color.gray,
                        marginBottom = 2,
                        marginLeft = 2,
                        marginRight = 2,
                        marginTop = 2,
                    }
                });
                text.tooltip = tagName;
                UIElementUtils.TextOverflowWithEllipsis(text);
                div.RegisterCallback<MouseDownEvent>(evt =>
                {
                    if (evt.button != 0) return;
                    AddTag(tagContent, tagName);
                    if (HierarchyManagementWindow.recentTags.IndexOf(tagName) == 0) return;
                    HierarchyManagementWindow.recentTags.Insert(0, tagName);
                    while (HierarchyManagementWindow.recentTags.Count > 2)
                    {
                        HierarchyManagementWindow.recentTags.RemoveAt(2);
                    }
                    DrawRecentTags(visualElement, tagContent);
                });
            }
        }

        #endregion

        #region Event
        private void Submit()
        {
            if (string.IsNullOrEmpty(_mGuid))
            {
                EditorUtility.DisplayDialog("messageBox", EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_请选择一个Prefab),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消));
                return;
            }
            var path = AssetDatabase.GUIDToAssetPath(_mGuid);
            if (HierarchyManagementEvent._prefabDetails.Find(s => s.Guid == _mGuid) != null)
            {
                if (_isNew || _mPrefabSubmit.Guid != _mGuid)
                {
                    EditorUtility.DisplayDialog("messageBox", EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_该Prefab已经被定义层级),
                        EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定),
                        EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消));
                    return;
                }
            }
            if (path.Length < 8 || path.Substring(path.Length - 7, 7) != ".prefab")
            {
                EditorUtility.DisplayDialog("messageBox", EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_请选择一个Prefab),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消));
                return;
            }
            if (!string.IsNullOrEmpty(_tagInput.value))
            {
                if (!EditorUtility.DisplayDialog("messageBox", EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_输入的标签没有加入标签列表Tip),
                        EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定),
                        EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消)))
                    return;
            }
            var list = HierarchyManagementEvent._prefabDetails;
            if (_isNew)
            {
                _mPrefabSubmit.ID = list.Count;
                _mPrefabSubmit.Guid = _mGuid;
                _mPrefabSubmit.Name = Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(_mGuid));
                _mPrefabSubmit.LevelID = _mLevel.ID;
                _mPrefabSubmit.ChannelID = _mLevel.ChannelID;

                foreach (var item in _mTags)
                {
                    item.Num = DealWithTag(item);
                }
                _mPrefabSubmit.Tags = _mTags;
                list.Add(_mPrefabSubmit);
                _mLevel.PrefabDetailIDList.Insert(0, _mPrefabSubmit.ID);

                HierarchyManagementWindow.GetInstance()._scrollFlag = true;
            }
            else
            {
                PrefabDetail prefabDetail = HierarchyManagementEvent._prefabDetails.Where(t => t.ID == _mPrefabSubmit.ID).First();
                int index = HierarchyManagementEvent._prefabDetails.IndexOf(prefabDetail);
                if (index == -1) return;
                HierarchyManagementEvent._prefabDetails[index] = _mPrefabSubmit;
                if (_mPrefabSubmit.LevelID != _mLevel.ID)
                {
                    HierarchyManagementEvent._managementLevels.Where(t => t.ID == _mPrefabSubmit.LevelID).FirstOrDefault().PrefabDetailIDList.Remove(_mPrefabSubmit.ID);
                    _mLevel.PrefabDetailIDList.Insert(0, _mPrefabSubmit.ID);
                    _mPrefabSubmit.Guid = _mGuid;
                    _mPrefabSubmit.Name = Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(_mGuid));
                    _mPrefabSubmit.LevelID = _mLevel.ID;
                    _mPrefabSubmit.ChannelID = _mLevel.ChannelID;
                    _mPrefabSubmit.Tags = _mTags;
                }
                else
                {
                    _mPrefabSubmit.Guid = _mGuid;
                    _mPrefabSubmit.Name = Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(_mGuid));
                    _mPrefabSubmit.LevelID = _mLevel.ID;
                    _mPrefabSubmit.ChannelID = _mLevel.ChannelID;
                    _mPrefabSubmit.Tags = _mTags;
                }
            }

            var maxPrefabNum = 0;
            foreach (var item in HierarchyManagementEvent._managementLevels)
            {
                if (item.PrefabDetailIDList.Count > maxPrefabNum)
                    maxPrefabNum = item.PrefabDetailIDList.Count;
            }
            //更新层级数据
            HierarchyManagementEvent.maxPrefabNum = maxPrefabNum;

            //更新窗口中数据
            HierarchyManagementWindow.GetInstance().chosenPrefab = _mPrefabSubmit;
            HierarchyManagementWindow.GetInstance().Search(HierarchyManagementWindow._searchText,
                HierarchyManagementWindow.GetInstance().resultDiv);
            CloseWindow();
        }

        private int DealWithTag(TagDetail tag)
        {
            var num = 0;
            var prefab = HierarchyManagementEvent._prefabDetails.FindLast(s =>
                s.Tags.Contains(tag) && s.Guid != _mPrefabSubmit.Guid && s.LevelID <= _mLevel.ID);
            foreach (var s in HierarchyManagementEvent._prefabDetails)
            {
                if (s.Tags.Contains(tag) && s.Guid != _mPrefabSubmit.Guid && s.LevelID <= _mLevel.ID)
                {
                    if (s.Tags.Find(td => td.Equals(tag)).Num > prefab.Tags.Find(td => td.Equals(tag)).Num)
                    {
                        prefab = s;
                    }
                }
            }
            // var prefab = _prefabDetails.FindLast(s =>
            //         s.Tags.Contains(tag) && s.ID != _mPrefabSubmit.ID && s.LevelID <= _mLevel.ID);
            if (prefab != null)
            {
                if (prefab.LevelID < _mLevel.ID)
                {
                    num = prefab.Tags.Find(s => s.Equals(tag)).Num + 1;
                    foreach (var item in HierarchyManagementEvent._prefabDetails.FindAll(s =>
                                 s.Tags.Contains(tag) && s.LevelID > _mLevel.ID))
                    {
                        item.Tags.Find(s => s.Equals(tag)).Num += 1;
                    }
                }
                else num = prefab.Tags.Find(s => s.Equals(tag)).Num;
            }
            else
            {
                foreach (var item in HierarchyManagementEvent._prefabDetails.FindAll(s =>
                             s.Tags.Contains(tag) && s.LevelID > _mLevel.ID))
                {
                    item.Tags.Find(s => s.Equals(tag)).Num += 1;
                }
            }

            return num;
        }

        private TagDetail AddTag(VisualElement tagContent, string str)
        {
            if (string.IsNullOrEmpty(str.Trim()))
            {
                EditorUtility.DisplayDialog("messageBox", EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_添加的标签不能为空),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消));
                return null;
            }
            if (_mTags.Contains(new TagDetail() { Name = str }))
            {
                EditorUtility.DisplayDialog("messageBox", EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_已包含此标签),
                  EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定),
                  EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消));
                return null;
            }

            var tag = new TagDetail() { Name = str };
            int num = 0;

            if (!_isNew)
            {
                num = DealWithTag(tag);
            }

            TagColor tagColor = HierarchyManagementEvent._mTagColors.Find(s => s.Name == tag.Name);
            if (tagColor == null)
            {
                HierarchyManagementEvent._mTagColors.Add(new TagColor()
                {
                    Name = tag.Name,
                    Color = Color.black,
                });
                EditorUtility.DisplayDialog("messageBox",
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_新标签的默认展示Tip),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消));
            }

            tag.Num = num;
            _mTags.Add(tag);
            DrawTags(tagContent);

            return tag;
        }

        private void DeleteTag(TagDetail tag, VisualElement visualElement)
        {
            //判断是否要进行tag的减
            var flag = HierarchyManagementEvent._prefabDetails.FindAll(s => s.Tags.Contains(tag) && s.ID != _mPrefabSubmit.ID)
                .Any(item => item.Tags.Find(s => s.Equals(tag)).Num == tag.Num);
            if (!flag)
            {
                foreach (var item in HierarchyManagementEvent._prefabDetails.FindAll(s =>
                             s.Tags.Contains(tag) && s.ID != _mPrefabSubmit.ID))
                {
                    var t = item.Tags.Find(s => s.Equals(tag));
                    if (t.Num > tag.Num)
                        t.Num -= 1;
                }
            }

            _mTags.Remove(tag);
            DrawTags(visualElement);
        }

        #endregion

        private void CloseWindow()
        {
            if (_mWindow != null)
            {
                _mWindow.Close();
            }
        }
    }
}