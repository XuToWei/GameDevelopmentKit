#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace ThunderFireUITool
{
    public class AboutWindow : EditorWindow
    {
        private static AboutWindow m_window;
        public static string AboutText;
        public static string CloseText;
        private static VisualElement AboutUXML;

        [MenuItem(ThunderFireUIToolConfig.Menu_About, false, -150)]
        public static void OpenWindow()
        {
            int width = 480;
            int height = 274;
            m_window = GetWindow<AboutWindow>();
            //m_window = CreateInstance(typeof(AboutWindow)) as AboutWindow;
            if (m_window == null) return;
            m_window.minSize = new Vector2(width, height);
            m_window.maxSize = new Vector2(width, height);
            m_window.titleContent.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_关于);
            m_window.position = new Rect((Screen.currentResolution.width - width) / 2,
                (Screen.currentResolution.height - height) / 2, width, height);
            //m_window.ShowModalUtility();
        }


        public static void InitWindowData()
        {
            AboutText = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_用户手册);
            CloseText = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_关闭);
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ThunderFireUIToolConfig.UIBuilderPath + "AboutPanel.uxml");
            AboutUXML = visualTree.CloneTree();
        }


        private void OnEnable()
        {
            DrawUI();
        }

        private void DrawUI()
        {
            var root = rootVisualElement;
            root.style.paddingBottom = 25;
            root.style.paddingLeft = 0;
            root.style.paddingRight = 0;
            root.style.paddingTop = 25;

            var div = UXBuilder.Div(root, new UXBuilderDivStruct());
            var rowTop = UXBuilder.Row(div, new UXBuilderRowStruct()
            {
                align = Align.Center,
                justify = Justify.Center,
                style = new UXStyle() { height = 86 }
            });

            var image = new Image() { style = { width = 203, height = 65 } };
            image.image =
                AssetDatabase.LoadAssetAtPath<Texture>(ThunderFireUIToolConfig.IconPath + "ToolLogo.png");
            image.scaleMode = ScaleMode.ScaleToFit;
            rowTop.Add(image);

            var rowVersion = UXBuilder.Row(div, new UXBuilderRowStruct()
            {
                align = Align.Center,
                justify = Justify.Center,
                style = new UXStyle() { marginTop = 15 }
            });

            UXBuilder.Text(rowVersion, new UXBuilderTextStruct()
            {
                style = new UXStyle()
                {
                    unityTextAlign = TextAnchor.MiddleCenter,
                    fontSize = 14,
                },
                text = "V 0.9.4"
            });


            var rowUserID = UXBuilder.Row(div, new UXBuilderRowStruct()
            {
                align = Align.Center,
                justify = Justify.Center,
                style = new UXStyle() { marginTop = 35 }
            });

            var userIDLabel = UXBuilder.Text(rowUserID, new UXBuilderTextStruct()
            {
                style = new UXStyle()
                {
                    unityTextAlign = TextAnchor.MiddleCenter,
                    fontSize = 14,
                },
                text = "ID:"
            });

            var userIDContentLabel = UXBuilder.Text(rowUserID, new UXBuilderTextStruct()
            {
                style = new UXStyle()
                {
                    unityTextAlign = TextAnchor.MiddleCenter,
                    fontSize = 14,
                },
                text = UXToolAnalysis.GetUserID()
            });

            var buttonCopy = UXBuilder.Button(rowUserID, new UXBuilderButtonStruct()
            {
                text = "",
                style = new UXStyle()
                {
                    width = 16,
                    height = 16,
                    backgroundImage = (StyleBackground)ToolUtils.GetIcon("ToolBar/copy"),
                    color = new Color(0, 0, 0, 0)
                },
                OnClick = () =>
                {
                    string str = userIDContentLabel.text;

                    TextEditor textEditor = new TextEditor();
                    textEditor.text = str;
                    textEditor.SelectAll();
                    textEditor.Copy();
                    ShowNotification(new GUIContent(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_ID已复制)), 0.5f);
                }
            }); ;


            var rowButtons = UXBuilder.Row(div, new UXBuilderRowStruct()
            {
                align = Align.Center,
                justify = Justify.Center,
                style = new UXStyle() { width = Length.Percent(100), marginTop = 45 }
            });

            var buttonUrl = UXBuilder.Button(rowButtons, new UXBuilderButtonStruct()
            {
                text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_用户手册),
                type = ButtonType.Primary,
                style = new UXStyle()
                {
                    fontSize = 14,
                    paddingBottom = 6,
                    paddingTop = 6,
                    paddingLeft = 25,
                    paddingRight = 25,
                    marginRight = 12
                },
                OnClick = () => { Application.OpenURL("https://uxtool.netease.com/help"); }
            });
            var buttonClose = UXBuilder.Button(rowButtons, new UXBuilderButtonStruct()
            {
                text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_关闭),
                style = new UXStyle()
                {
                    fontSize = 14,
                    paddingBottom = 6,
                    paddingTop = 6,
                    paddingLeft = 25,
                    paddingRight = 25
                },
                OnClick = CloseWindow
            });
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ThunderFireUIToolConfig.UIBuilderPath + "USS/GuideButton.uss");
            buttonUrl.styleSheets.Add(styleSheet);
            buttonUrl.AddToClassList("ux-button-guide");

            var rowRight = UXBuilder.Row(div, new UXBuilderRowStruct()
            {
                align = Align.Center,
                justify = Justify.Center,
                style = new UXStyle() { marginTop = 10 }
            });

            UXBuilder.Text(rowRight, new UXBuilderTextStruct()
            {
                style = new UXStyle()
                {
                    unityTextAlign = TextAnchor.MiddleCenter,
                    fontSize = 12,
                    color = new Color(171f / 255f, 171f / 255f, 171f / 255f)
                },
                text = "2022 ThunderFire-All rights reserved."
            });
        }

        private void CloseWindow()
        {
            if (m_window != null)
            {
                m_window.Close();
            }

        }

        private void Submit()
        {
            Application.OpenURL("https://uxtool.netease.com/help");
        }

    }

}
#endif