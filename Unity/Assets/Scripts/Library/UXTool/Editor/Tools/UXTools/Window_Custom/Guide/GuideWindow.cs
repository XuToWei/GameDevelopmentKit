using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ThunderFireUITool;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class GuideWindow : EditorWindow
{
    private static GuideWindow _mWindow;

    public static List<Toggle> toggles;

    private VisualElement rowBottom;

    private bool agree = false;

    public static string deleteUrl = ThunderFireUIToolConfig.ScriptsRootPath;

    public static int pageNum = 0;
    public static bool isAgreement = false;

#if UXTOOLS_DEV
    [MenuItem(ThunderFireUIToolConfig.Menu_WelcomePage, false, -149)]
#endif
    public static void OpenWindow()
    {
        int width = 1200;
        int height = 770;
        _mWindow = CreateInstance(typeof(GuideWindow)) as GuideWindow;
        if (_mWindow == null) return;
        _mWindow.minSize = new Vector2(width, height);
        _mWindow.maxSize = new Vector2(width, height);
        _mWindow.titleContent.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_欢迎页);
        _mWindow.position = new Rect((Screen.currentResolution.width - width) / 2,
            (Screen.currentResolution.height - height) / 2, width, height);
        // _mWindow.Show();
        _mWindow.ShowModalUtility();
    }

    private void OnEnable()
    {
        toggles = new Toggle[Enum.GetValues(typeof(SwitchSetting.SwitchType)).Cast<int>().Max() + 1].ToList();
        for (int i = 0; i < toggles.Count; i++)
        {
            toggles[i] = new Toggle();
            toggles[i].value = SwitchSetting.CheckValid(i);
        }
        var root = rootVisualElement;
        root.style.paddingBottom = 40;
        root.style.paddingLeft = 90;
        root.style.paddingRight = 90;
        root.style.paddingTop = 60;
        if (isAgreement) DrawAgreement();
        else DrawUI();
    }

    private void DrawUI()
    {
        var root = rootVisualElement;
        root.Clear();
        var div = UXBuilder.Div(root, new UXBuilderDivStruct());
        var rowTop = UXBuilder.Row(div, new UXBuilderRowStruct()
        {
            align = Align.Center,
            justify = Justify.Center,
            style = new UXStyle() { height = 86 }
        });

        var rowPage = UXBuilder.Row(div, new UXBuilderRowStruct()
        {
            align = Align.Center,
            justify = Justify.Center,
            style = new UXStyle() { height = 500 }
        });

        var image = new Image() { style = { width = 276, height = 86 } };
        image.image =
            AssetDatabase.LoadAssetAtPath<Texture>($"{ThunderFireUIToolConfig.IconPath}ToolLogo.png");
        image.scaleMode = ScaleMode.ScaleToFit;
        rowTop.Add(image);

        var line = UXBuilder.Divider(div, new UXBuilderDividerStruct()
        {
            style = new UXStyle() { width = Length.Percent(100), marginBottom = 10, marginTop = 10 },
        });

        rowBottom = UXBuilder.Row(div, new UXBuilderRowStruct()
        {
            align = Align.Center,
            justify = Justify.Center,
            style = new UXStyle() { height = 63 }
        });

        DrawPage(rowPage);
    }

    public void DrawPage(VisualElement parent)
    {
        parent.Clear();
        var page = pageNum == 0 ? (VisualElement)new GuideFirstPage(parent) : (VisualElement)new GuideLastPage(parent);
        parent.Add(page);
        DrawBottom(rowBottom);
    }

    public void DrawBottom(VisualElement parent)
    {
        parent.Clear();
        var rowAgreement = UXBuilder.Row(parent, new UXBuilderRowStruct()
        {
            align = Align.Center,
            justify = Justify.FlexStart,
            style = new UXStyle() { width = Length.Percent(50), height = Length.Percent(100) }
        });
        var toggleAgreement = UXBuilder.CheckBox(rowAgreement, new UXBuilderCheckBoxStruct()
        {
            onChange = b => agree = b,
            style = new UXStyle() { marginTop = 1, marginBottom = Length.Percent(0) },
        });
        toggleAgreement.value = agree;
        var toggle = toggleAgreement.Q<VisualElement>(null, "unity-toggle__checkmark");
        toggle.style.width = 16;
        toggle.style.height = 16;
        var textAgreement = UXBuilder.Text(rowAgreement, new UXBuilderTextStruct()
        {
            text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_请阅读并同意),
            style = new UXStyle()
            {
                fontSize = 16,
                marginLeft = 5,
                paddingRight = 1
            }
        });
        var buttonAgreement = UXBuilder.Button(rowAgreement, new UXBuilderButtonStruct()
        {
            type = ButtonType.Text,
            text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_使用协议题目),
            OnClick = () =>
            {
                isAgreement = true;
                DrawAgreement();
            },
            style = new UXStyle() { fontSize = 16, marginLeft = 1, marginRight = 1, paddingLeft = 1, paddingRight = 1, }
        });

        var rowButtons = UXBuilder.Row(parent, new UXBuilderRowStruct()
        {
            align = Align.Center,
            justify = Justify.FlexEnd,
            style = new UXStyle() { width = Length.Percent(50), height = Length.Percent(100) }
        });

        var buttonUrl = UXBuilder.Button(rowButtons, new UXBuilderButtonStruct()
        {
            text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_用户手册),
            style = new UXStyle()
            {
                fontSize = 16,
                paddingBottom = 6,
                paddingTop = 6,
                paddingLeft = 25,
                paddingRight = 25,
                marginRight = 12
            },
            OnClick = () =>
            {
                Application.OpenURL("https://uxtool.netease.com/help");
            }
        });
        var buttonClose = UXBuilder.Button(rowButtons, new UXBuilderButtonStruct()
        {
            text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_开始使用),
            type = ButtonType.Primary,
            style = new UXStyle()
            {
                fontSize = 16,
                paddingBottom = 6,
                paddingTop = 6,
                paddingLeft = 25,
                paddingRight = 25
            },
            OnClick = () =>
            {
                GuideWindow.GetInstance().CloseWindow();
            }
        });
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ThunderFireUIToolConfig.UIBuilderPath + "USS/GuideButton.uss");
        buttonClose.styleSheets.Add(styleSheet);
        buttonClose.AddToClassList("ux-button-guide");
    }

    public static GuideWindow GetInstance()
    {
        if (_mWindow == null) _mWindow = GetWindow<GuideWindow>();
        return _mWindow;
    }

    public void CloseWindow()
    {
        _mWindow.Close();
    }

    private void OnDestroy()
    {
        SwitchSetting.ChangeSwitch(GuideWindow.toggles.ToArray());
        if (agree)
        {
            pageNum = 0;
            isAgreement = false;
            UXToolAnalysis.SendUXToolLog(UXToolAnalysisLog.UXToolAccept);
            return;
        }

        var option = EditorUtility.DisplayDialogComplex(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_提示),
            EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_使用协议提示),
            EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_同意),
            EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消),
            EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_不同意));

        switch (option)
        {
            // Agree.
            case 0:
                UXToolAnalysis.SendUXToolLog(UXToolAnalysisLog.UXToolAccept);
                pageNum = 0;
                isAgreement = false;
                agree = true;
                break;
            // Cancel.
            case 1:
                EditorApplication.delayCall += () => OpenWindow();
                break;
            // Don't Save.
            case 2:
                pageNum = 0;
                isAgreement = false;
                DeleteFile();
                EditorPrefs.SetBool("UXToolGuide", true);

                break;
            default:
                OpenWindow();
                break;
        }
    }


    private void DrawAgreement()
    {
        var root = rootVisualElement;
        root.Clear();
        var div = UXBuilder.Div(root, new UXBuilderDivStruct());
        var rowTop = UXBuilder.Row(div, new UXBuilderRowStruct()
        {
            align = Align.Center,
            justify = Justify.Center,
            style = new UXStyle() { height = 86 }
        });
        UXBuilder.Text(rowTop, new UXBuilderTextStruct()
        {
            text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_使用协议题目),
            style = { fontSize = 26 },
        });
        var scrollView = UXBuilder.ScrollView(div, new UXBuilderScrollViewStruct()
        {
            style =
            {
                width = Length.Percent(80), height = 480, alignSelf = Align.Center, marginBottom = 20,
                backgroundColor = new Color(0, 0, 0, 70f / 255f), paddingTop = 20, paddingBottom = 20
            },
        });
        scrollView.style.position = Position.Relative;
        UXBuilder.Text(scrollView, new UXBuilderTextStruct()
        {
            text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_使用协议内容),
            style =
            {
                width = Length.Percent(100), fontSize = 18, paddingLeft = 40, paddingRight = 40,
            },
        });

        var line = UXBuilder.Divider(div, new UXBuilderDividerStruct()
        {
            style = new UXStyle() { width = Length.Percent(100), marginBottom = 10, marginTop = 10 },
        });

        var agreementBottom = UXBuilder.Row(div, new UXBuilderRowStruct()
        {
            align = Align.Center,
            justify = Justify.FlexStart,
            style = new UXStyle() { height = 63 }
        });
        var textAgreement = UXBuilder.Button(agreementBottom, new UXBuilderButtonStruct()
        {
            text = "<< " + EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_返回),
            type = ButtonType.Text,
            style = new UXStyle()
            {
                fontSize = 16,
                unityFontStyleAndWeight = FontStyle.Italic
            },
            OnClick = () =>
            {
                isAgreement = false;
                DrawUI();
            },
        });
    }

    private static void DeleteFile()
    {
        if (Directory.Exists(deleteUrl))
        {
            try
            {
                // var dir = new DirectoryInfo(deleteUrl);
                // dir.Attributes &= ~FileAttributes.ReadOnly;
                // dir.Delete(true);
                Directory.Delete(deleteUrl, true);
                // Debug.Log("未同意《UX Tool使用协议》，已删除包体”");
            }
            catch (Exception ex)
            {
                Debug.LogError("删除文件夹时,出现错误:" + ex.Message);
            }
        }
    }
}
