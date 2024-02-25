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
            if(commonData == null)
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
            VisualElement Container = new VisualElement();
            Container.style.position = Position.Absolute;
            Container.style.alignSelf = Align.Center;
            Container.style.bottom = 5;
            Container.style.top = 5;
            Container.style.width = 400;
            rightContainer.Add(Container);

            TextElement nameLabel = new TextElement();
            nameLabel.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_语言);
            nameLabel.style.position = Position.Absolute;
            nameLabel.style.left = 0;
            nameLabel.style.fontSize = 13;
            nameLabel.style.top = 5;
            nameLabel.style.color = Color.white;
            Container.Add(nameLabel);

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
            Container.Add(LanguageEnumField);


            TextElement projectNameLabel = new TextElement();
            projectNameLabel.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_自定义项目名称);
            projectNameLabel.style.position = Position.Absolute;
            projectNameLabel.style.left = 0;
            projectNameLabel.style.fontSize = 13;
            projectNameLabel.style.top = 55;
            projectNameLabel.style.color = Color.white;
            Container.Add(projectNameLabel);

            projectNameTextField = new TextField();
            projectNameTextField.style.position = Position.Absolute;
            projectNameTextField.style.width = 137;
            projectNameTextField.style.height = 25;
            projectNameTextField.style.top = 55;
            projectNameTextField.style.right = 0;

            if(commonData != null)
            {
                projectNameTextField.value = commonData.CustomUnityWindowTitle;
            }
            
            projectNameTextField.RegisterCallback<ChangeEvent<string>>((evt) =>
            {
                projectNameTextField.value = evt.newValue;
            });
            Container.Add(projectNameTextField);
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


        private void AddSwitchToggle(int x, string text, ref Toggle toggle)
        {
            toggle.style.position = Position.Absolute;
            toggle.style.left = 40;
            toggle.style.top = 25 * x + 15;
            rightContainer.Add(toggle);
            TextElement label = new TextElement();
            label.text = text;
            label.style.position = Position.Absolute;
            label.style.left = 62;
            label.style.top = 25 * x + 14;
            label.style.fontSize = 13;
            label.style.color = Color.white;
            label.style.maxWidth = 450;
            rightContainer.Add(label);
        }
        private void SwitchOnClick()
        {
            leftContainerRefresh();
            SwitchOption.isSelect();
            rightContainer.Clear();

            AddSwitchToggle(0, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_最近打开面板记录), ref switchToggles[0]);
            AddSwitchToggle(1, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_对齐吸附), ref switchToggles[1]);
            AddSwitchToggle(2, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_右键选择列表), ref switchToggles[2]);
            AddSwitchToggle(3, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_快速复制), ref switchToggles[3]);
            AddSwitchToggle(4, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_移动快捷键), ref switchToggles[4]);
            AddSwitchToggle(5, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_Prefab多开), ref switchToggles[5]);
            AddSwitchToggle(6, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_Scene中分辨率调整), ref switchToggles[6]);
            AddSwitchToggle(7, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_Prefab资源检查), ref switchToggles[7]);
            AddSwitchToggle(8, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_参考背景图片), ref switchToggles[8]);
            AddSwitchToggle(9, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_自动将Texture转为Sprite), ref switchToggles[9]);
            AddSwitchToggle(10, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_手柄引导开关), ref switchToggles[10]);

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
            if(commonData != null)
            {
                commonData.CustomUnityWindowTitle = projectNameTextField.value;
                commonData.Save();
            }

            CloseWindow();
        }
    }
}
#endif