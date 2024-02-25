using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using ThunderFireUITool;

public class LocalizationSettingWindow : EditorWindow
{
    private static LocalizationSettingWindow c_window;
    private static TextElement localizationFolder;
    private static TextElement tablePath;

    [MenuItem(ThunderFireUIToolConfig.Menu_Localization + "/设置 (Setting)", false, 54)]
    public static void OpenWindow()
    {
        int width = 650;
        int height = 350;
        c_window = GetWindow<LocalizationSettingWindow>();
        c_window.minSize = new Vector2(width, height);
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
    private Toggle[] toggles;
    private ConfigurationOption LocalizeOption;
    private ConfigurationOption PathOption;

    private void InitWindowData()
    {
        toggles = new Toggle[LocalizationLanguage.Length];
        for (int i = 0; i < toggles.Length; i++)
        {
            toggles[i] = new Toggle();
            toggles[i].SetEnabled(false);
        }
        foreach (int i in EditorLocalizationTool.ReadyLanguageTypes)
        {
            toggles[i].value = true;
        }
    }

    private void InitWindowUI()
    {
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ThunderFireUIToolConfig.UIBuilderPath + "SettingWindow.uxml");
        Root = visualTree.CloneTree();
        rootVisualElement.Add(Root);

        leftContainer = Root.Q<VisualElement>("LeftContainer");
        rightContainer = Root.Q<VisualElement>("RightContainer");

        Label nameLabel = Root.Q<Label>("Title");
        nameLabel.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_本地化设置);

        Button confirmBtn = Root.Q<Button>("ConfirmBtn");
        confirmBtn.clicked += ConfirmOnClick;
        confirmBtn.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定);

        Button cancelBtn = Root.Q<Button>("CancelBtn");
        cancelBtn.clicked += CloseWindow;
        cancelBtn.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消);

        leftContainerRefresh();
        LanguageOnClick();
    }

    private void AddLanguage(string text, int row, int col, ref Toggle toggle)
    {
        toggle.style.position = Position.Absolute;
        toggle.style.left = 50 + 225 * col;
        toggle.style.top = 20 + 30 * row;
        rightContainer.Add(toggle);
        TextElement label = new TextElement();
        label.text = text;
        label.style.position = Position.Absolute;
        label.style.left = 72 + 225 * col;
        label.style.top = 19 + 30 * row;
        label.style.fontSize = 13;
        label.style.color = Color.white;
        rightContainer.Add(label);
    }
    private void LanguageOnClick()
    {
        leftContainerRefresh();
        LocalizeOption.isSelect();
        rightContainer.Clear();
        for (int i = 0; i < LocalizationLanguage.Length; i++)
        {
            AddLanguage(LocalizationLanguage.GetLanguage(i), i / 2, i % 2, ref toggles[i]);
        }
    }

    private void PathOnClick()
    {
        leftContainerRefresh();
        PathOption.isSelect();
        rightContainer.Clear();

        TextElement label = new TextElement();
        label.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_本地化文件夹) + ":";
        label.style.position = Position.Absolute;
        label.style.left = 50;
        label.style.top = 20;
        label.style.fontSize = 13;
        label.style.color = Color.white;
        rightContainer.Add(label);

        localizationFolder = new TextElement();
        localizationFolder.text = UXGUIConfig.LocalizationFolder;
        localizationFolder.style.position = Position.Absolute;
        localizationFolder.style.left = 50;
        localizationFolder.style.top = 40;
        localizationFolder.style.maxWidth = 400;
        localizationFolder.style.fontSize = 13;
        localizationFolder.style.color = Color.white;
        rightContainer.Add(localizationFolder);

        Image mrIcon = new Image();
        mrIcon.style.height = 10;
        mrIcon.image = ToolUtils.GetIcon("More");
        Button MoreBtn = EditorUIUtils.CreateUIEButton("", mrIcon, OpenLocalizationFolder, 30, 20);
        MoreBtn.style.top = 20;
        MoreBtn.style.right = 50;
        MoreBtn.tooltip = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_打开路径);
        rightContainer.Add(MoreBtn);

        label = new TextElement();
        label.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_文本表格路径) + ":";
        label.style.position = Position.Absolute;
        label.style.left = 50;
        label.style.top = 80;
        label.style.fontSize = 13;
        label.style.color = Color.white;
        rightContainer.Add(label);

        tablePath = new TextElement();
        tablePath.text = ThunderFireUIToolConfig.TextTablePath;
        tablePath.style.position = Position.Absolute;
        tablePath.style.left = 50;
        tablePath.style.top = 100;
        tablePath.style.maxWidth = 400;
        tablePath.style.fontSize = 13;
        tablePath.style.color = Color.white;
        rightContainer.Add(tablePath);

        mrIcon = new Image();
        mrIcon.style.height = 10;
        mrIcon.image = ToolUtils.GetIcon("More");
        MoreBtn = EditorUIUtils.CreateUIEButton("", mrIcon, OpenTextTableFile, 30, 20);
        MoreBtn.style.top = 80;
        MoreBtn.style.right = 50;
        MoreBtn.tooltip = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_打开路径);
        rightContainer.Add(MoreBtn);
    }

    private void OpenLocalizationFolder()
    {
        Application.OpenURL(Path.GetFullPath(UXGUIConfig.LocalizationFolder));
    }

    private void OpenTextTableFile()
    {
        Application.OpenURL(Path.GetFullPath(ThunderFireUIToolConfig.TextTablePath));
    }

    private void leftContainerRefresh()
    {
        leftContainer.Clear();

        LocalizeOption = new ConfigurationOption(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_语言), LanguageOnClick);
        LocalizeOption.style.top = 0;
        leftContainer.Add(LocalizeOption);

        PathOption = new ConfigurationOption(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_路径), PathOnClick);
        PathOption.style.top = 40;
        leftContainer.Add(PathOption);
    }

    private void ConfirmOnClick()
    {
        CloseWindow();
    }
}