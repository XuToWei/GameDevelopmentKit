#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

using System.IO;
using System.Linq;
using System;

namespace ThunderFireUITool
{
    public class ConfigurationOption : VisualElement
    {
        Action clickAction;
        float height = 40;
        public TextElement nameLabel;
        private bool m_selected = false;

        public ConfigurationOption(string text = "", Action action = null)
        {
            clickAction = action;

            style.position = Position.Absolute;
            style.left = 0;
            style.right = 0;
            style.height = height;

            nameLabel = new TextElement();
            nameLabel.text = text;
            nameLabel.style.position = Position.Absolute;
            nameLabel.style.bottom = 10;
            nameLabel.style.alignSelf = Align.Center;
            nameLabel.style.fontSize = 15;
            nameLabel.style.color = Color.white;
            this.Add(nameLabel);

            RegisterCallback<MouseDownEvent>(OnClick);
            RegisterCallback<MouseEnterEvent>(OnHoverStateChang);
            RegisterCallback<MouseLeaveEvent>(OnHoverStateChang);
        }

        private void OnHoverStateChang(EventBase e)
        {
            if (!m_selected)
            {
                if (e.eventTypeId == MouseEnterEvent.TypeId())
                {

                    style.backgroundColor = new Color(0.6f, 0.6f, 0.6f, 1);

                }
                else if (e.eventTypeId == MouseLeaveEvent.TypeId())
                {
                    if (parent != null)
                    {
                        style.backgroundColor = parent.style.backgroundColor;
                    }

                }
            }
        }

        public void isSelect()
        {
            m_selected = true;
            style.backgroundColor = new Color(0.6f, 0.6f, 0.6f, 1);
        }

        private void OnClick(MouseDownEvent e)
        {
            if (e.button == 0)
            {
                if (!m_selected)
                {
                    clickAction.Invoke();
                }
            }
        }
    }

    public class ConfigurationWindow : EditorWindow
    {
        private static ConfigurationWindow c_window;

        [MenuItem(ThunderFireUIToolConfig.Menu_Setting, false, -148)]
        public static void OpenWindow()
        {
            int width = 650;
            int height = 430;
            c_window = GetWindow<ConfigurationWindow>();
            c_window.minSize = new Vector2(width, height);
            c_window.maxSize = new Vector2(width, height);
            c_window.position = new Rect((Screen.currentResolution.width - width) / 2, (Screen.currentResolution.height - height) / 2, width, height);
            c_window.titleContent.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_设置);
        }

        public static void CloseWindow()
        {
            if (c_window != null)
            {
                c_window.Close();
            }
        }

        [UnityEditor.Callbacks.DidReloadScripts(0)]
        private static void OnScriptReload()
        {
            if (HasOpenInstances<ConfigurationWindow>())
                c_window = GetWindow<ConfigurationWindow>();
        }

        private void OnEnable()
        {
            InitWindowData();
            InitWindowUI();
        }

        private VisualElement Root;
        private VisualElement rightContainer;
        private VisualElement leftContainer;
        private PopupField<string> LanguageEnumField;
        private Toggle[] switchToggles;
        private TextField projectNameTextField;
        private IntegerField maxFilesField;
        private IntegerField maxPrefabsField;
        private TextElement errorLabel;
        private TextElement errorPrefabLabel;

        private ConfigurationOption GeneralOption;
        private ConfigurationOption StorageOption;
        private ConfigurationOption SwitchOption;

        private EditorLocalName LanguageType;
        private WidgetInstantiateMode PrefabDragMode;
        // private string prefabPath;
        // private string componentPath;

        private UXToolCommonData commonData;

        private void InitWindowData()
        {
            EditorLocalizationSettings localizationSetting = JsonAssetManager.GetAssets<EditorLocalizationSettings>();

            LanguageType = localizationSetting.LocalType;

            int max = Enum.GetValues(typeof(SwitchSetting.SwitchType)).Cast<int>().Max();
            switchToggles = new Toggle[max + 1];
            for (int i = 0; i < switchToggles.Length; i++)
            {
                switchToggles[i] = new Toggle();
                switchToggles[i].value = SwitchSetting.CheckValid(i);
            }

            commonData = AssetDatabase.LoadAssetAtPath<UXToolCommonData>(ThunderFireUIToolConfig.UXToolCommonDataPath);
            if (commonData == null)
            {
                commonData = UXToolCommonData.Create();
            }
        }

        private void InitWindowUI()
        {
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ThunderFireUIToolConfig.UIBuilderPath + "SettingWindow.uxml");
            Root = visualTree.CloneTree();
            rootVisualElement.Add(Root);
            Root.style.alignSelf = Align.Center;

            leftContainer = Root.Q<VisualElement>("LeftContainer");
            rightContainer = Root.Q<VisualElement>("RightContainer");

            Label nameLabel = Root.Q<Label>("Title");
            nameLabel.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_设置);

            Button confirmBtn = Root.Q<Button>("ConfirmBtn");
            confirmBtn.clicked += ConfirmOnClick;
            confirmBtn.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定);

            Button cancelBtn = Root.Q<Button>("CancelBtn");
            cancelBtn.clicked += CloseWindow;
            cancelBtn.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消);

            leftContainerRefresh();
            GeneralOnClick();
        }

        private void GeneralOnClick()
        {
            leftContainerRefresh();
            GeneralOption.isSelect();
            rightContainer.Clear();

            VisualElement container = UXBuilder.Div(rightContainer, new UXBuilderDivStruct()
            {
                style = new UXStyle()
                {
                    position = Position.Absolute,
                    alignSelf = Align.Center,
                    bottom = 5,
                    top = 5,
                    width = 400,
                }
            });

            AddGeneralSetting(container);

            AddFilesCountLimit(container);

        }

        private void AddGeneralSetting(VisualElement container)
        {
            TextElement generalTitleLabel = UXBuilder.Text(container, new UXBuilderTextStruct()
            {
                text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_通用),
                style = new UXStyle()
                {
                    position = Position.Absolute,
                    left = -40,
                    fontSize = 16,
                    top = 0,
                    color = Color.white,
                }
            });

            TextElement nameLabel = UXBuilder.Text(container, new UXBuilderTextStruct()
            {
                text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_语言),
                style = new UXStyle()
                {
                    position = Position.Absolute,
                    left = 0,
                    fontSize = 13,
                    top = 40,
                    color = Color.white,
                }
            });

            var list = new List<string>();
            list.Add("简体中文");
            list.Add("繁體中文");
            list.Add("English");
            list.Add("日本語");
            list.Add("한국어");
            LanguageEnumField = new PopupField<string>(list, ((int)LanguageType));
            LanguageEnumField.style.position = Position.Absolute;
            LanguageEnumField.style.width = 137;
            LanguageEnumField.style.height = 25;
            LanguageEnumField.style.right = 0;
            LanguageEnumField.style.top = 40;
            container.Add(LanguageEnumField);

            TextElement projectNameLabel = UXBuilder.Text(container, new UXBuilderTextStruct()
            {
                text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_自定义项目名称),
                style = new UXStyle()
                {
                    position = Position.Absolute,
                    left = 0,
                    fontSize = 13,
                    top = 70,
                    color = Color.white,
                }
            });

            projectNameTextField = new TextField();
            projectNameTextField.style.position = Position.Absolute;
            projectNameTextField.style.width = 137;
            projectNameTextField.style.height = 25;
            projectNameTextField.style.top = 70;
            projectNameTextField.style.right = 0;

            if (commonData != null)
            {
                projectNameTextField.value = commonData.CustomUnityWindowTitle;
            }

            projectNameTextField.RegisterCallback<ChangeEvent<string>>((evt) =>
            {
                projectNameTextField.value = evt.newValue;
            });
            container.Add(projectNameTextField);
        }


        private void AddFilesCountLimit(VisualElement container)
        {
            TextElement limitTitleLabel = UXBuilder.Text(container, new UXBuilderTextStruct()
            {
                text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_面板设置),
                style = new UXStyle()
                {
                    position = Position.Absolute,
                    left = -40,
                    fontSize = 16,
                    top = 115,
                    color = Color.white,
                }
            });

            TextElement maxPrefabsLabel = UXBuilder.Text(container, new UXBuilderTextStruct()
            {
                text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_最近打开模板数上限),
                style = new UXStyle()
                {
                    position = Position.Absolute,
                    left = 0,
                    fontSize = 13,
                    top = 155,
                    color = Color.white,
                    maxWidth = 270,
                }
            });

            maxPrefabsField = new IntegerField();
            maxPrefabsField.style.position = Position.Absolute;
            maxPrefabsField.style.width = 137;
            maxPrefabsField.style.height = 25;
            maxPrefabsField.style.top = 155;
            maxPrefabsField.style.right = 0;
            if (commonData != null)
            {
                maxPrefabsField.value = commonData.MaxRecentOpenedPrefabs;
            }

            container.Add(maxPrefabsField);
            errorPrefabLabel = UXBuilder.Text(container, new UXBuilderTextStruct()
            {
                text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_显示上限必须大于0),
                style = new UXStyle()
                {
                    position = Position.Absolute,
                    color = Color.red,
                    maxWidth = 137,
                    display = DisplayStyle.None,
                    fontSize = 13,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    top = 180,
                    right = 1,

                }
            });


            TextElement maxFilesLabel = UXBuilder.Text(container, new UXBuilderTextStruct()
            {
                text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_最近选中文件数上限),
                style = new UXStyle()
                {
                    position = Position.Absolute,
                    left = 0,
                    fontSize = 13,
                    top = 210,
                    color = Color.white,
                    maxWidth = 250,
                }
            });

            maxFilesField = new IntegerField();
            maxFilesField.style.position = Position.Absolute;
            maxFilesField.style.width = 137;
            maxFilesField.style.height = 25;
            maxFilesField.style.top = 210;
            maxFilesField.style.right = 0;
            if (commonData != null)
            {
                maxFilesField.value = commonData.MaxRecentSelectedFiles;
            }

            container.Add(maxFilesField);

            errorLabel = UXBuilder.Text(container, new UXBuilderTextStruct()
            {
                text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_显示上限必须大于0),
                style = new UXStyle()
                {
                    position = Position.Absolute,
                    color = Color.red,
                    maxWidth = 137,
                    display = DisplayStyle.None,
                    fontSize = 13,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    top = 235,
                    right = 1,

                }
            });
        }

        private void leftContainerRefresh()
        {
            leftContainer.Clear();

            GeneralOption = new ConfigurationOption(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_通用), GeneralOnClick);
            GeneralOption.style.top = 0;
            leftContainer.Add(GeneralOption);

            //StorageOption = new ConfigurationOption(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_组件), StorageOnClick);
            //StorageOption.style.top = 40;
            //leftContainer.Add(StorageOption);

            SwitchOption = new ConfigurationOption(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_功能开关), SwitchOnClick);
            SwitchOption.style.top = 40;
            leftContainer.Add(SwitchOption);
        }


        Dictionary<string, List<(string, int)>> GetSwitchCategories()
        {
            // 这里的字典key为分类名称，value为该分类下的所有开关的本地化以及SwitchSetting对应的SwitchType序号
            return new Dictionary<string, List<(string, int)>>()
            {
                { "基础common", new List<(string , int)> {
                    (EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_对齐吸附), 1),
                    (EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_右键选择列表), 2),
                    (EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_快速复制), 3),
                    (EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_移动快捷键), 4)
                }},
                { "操作记录", new List<(string , int)> {
                    (EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_最近打开面板记录), 0),
                    (EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_最近选中面板记录), 11)
                }},
                { "其他", new List<(string , int)> {
                    (EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_Prefab多开), 5),
                    (EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_Scene中分辨率调整), 6),
                    (EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_Prefab资源检查), 7),
                    (EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_参考背景图片), 8),
                    (EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_自动将Texture转为Sprite), 9),
                    (EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_手柄引导开关), 10)
                }}
            };
        }

        private void AddSwitchToggle(VisualElement container, string text, ref Toggle toggle)
        {
            VisualElement row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.marginTop = 5;
            row.style.marginBottom = 5;
            row.style.marginLeft = 40;

            toggle.style.marginRight = 10;
            row.Add(toggle);

            Label label = new Label();
            label.text = text;

            label.style.fontSize = 13;
            label.style.color = Color.white;

            row.Add(label);
            container.Add(row);
        }
        private void SwitchOnClick()
        {
            leftContainerRefresh();
            SwitchOption.isSelect();
            rightContainer.Clear();
            ScrollView scrollView = new ScrollView();
            scrollView.style.flexGrow = 1;
            rightContainer.Add(scrollView);

            var categories = GetSwitchCategories();

            foreach (var category in categories)
            {
                // 添加分类标题
                Label header = new Label(category.Key);
                header.style.fontSize = 16;
                header.style.color = Color.white;
                header.style.marginLeft = 20;
                scrollView.Add(header);

                // 添加该分类下的所有开关
                for (int i = 0; i < category.Value.Count; i++)
                {
                    var (localization, toggleIndex) = category.Value[i];
                    AddSwitchToggle(scrollView, localization, ref switchToggles[toggleIndex]);
                }
            }
        }

        private void ConfirmOnClick()
        {
            var LocalizationSettings = JsonAssetManager.GetAssets<EditorLocalizationSettings>();
            LocalizationSettings.ChangeLocalValue((EditorLocalName)LanguageEnumField.index);

            Selection.activeGameObject = null;

            SwitchSetting.ChangeSwitch(switchToggles);
            if (SceneViewToolBar.HaveToolbar)
            {
                SceneViewToolBar.CloseEditor();
                SceneViewToolBar.OpenEditor();
            }
            if (commonData != null)
            {
                commonData.CustomUnityWindowTitle = projectNameTextField.value;
                if (maxPrefabsField.value >= 1)
                {
                    commonData.MaxRecentOpenedPrefabs = maxPrefabsField.value;
                    errorPrefabLabel.style.display = DisplayStyle.None;
                }
                else
                {
                    errorPrefabLabel.style.display = DisplayStyle.Flex;
                    return;
                }
                if (maxFilesField.value >= 1)
                {
                    commonData.MaxRecentSelectedFiles = maxFilesField.value;
                    errorLabel.style.display = DisplayStyle.None;
                }
                else
                {
                    errorLabel.style.display = DisplayStyle.Flex;
                    return;
                }

                commonData.Save();
            }

            CloseWindow();
        }
    }
}
#endif