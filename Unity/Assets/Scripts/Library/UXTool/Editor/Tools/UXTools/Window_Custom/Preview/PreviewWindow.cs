#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.Reflection;

namespace ThunderFireUITool
{
    /// <summary>
    /// 预览时弹出的控制预览参数的窗口
    /// </summary>
    public class PreviewWindow : EditorWindow
    {
        //[MenuItem("ThunderFireUXTool/预览工具 (preview)/show window", false, 52)]
        public static void OpenWindow()
        {
            int width = 300;
            int height = 60;
            var window = GetWindow<PreviewWindow>();
            window.minSize = new Vector2(width, height);
            window.maxSize = new Vector2(width, height);
            window.position = new Rect((Screen.currentResolution.width - width) / 2, (Screen.currentResolution.height - height) / 2, width, height);
            window.titleContent.text = "Preview";
        }

        public static void CloseWindow()
        {
            var window = GetWindow<PreviewWindow>();
            window.Close();
        }

        string[] defaultSizeOptions = new string[] { "1920 * 1080", "2520 * 1080", "1080 * 1920" };
        string[] defaultLangOptions = new string[] { "中文", "English" };
        string[] defaultColorOptions = new string[] { "Red", "Green", "Blue", "Normal" };

#if !UNITY_2019_1_OR_NEWER
        //使用IMG  
        //OnPreviewGUI不能绘制UI的preview
        //void OnGUI()
        //{
        //    Debug.Log("11111");
        //    gameObject = (GameObject)EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true);
        //    if (gameObject != null)
        //    {
        //        if (gameObjectEditor == null)
        //        {
        //            gameObjectEditor = Editor.CreateEditor(gameObject);
        //        }
        //        gameObjectEditor.OnPreviewGUI(GUILayoutUtility.GetRect(500, 500), EditorStyles.whiteLabel);
        //    }
        //}

        void OnGUI()
        {
            //if(EditorGUILayout.DropdownButton(new GUIContent(), FocusType.Keyboard))
            //{
            //    
            //}

            index = EditorGUILayout.Popup("分辨率:", index, options, GUILayout.Width(250));
            index = EditorGUILayout.Popup("语言:", index, options, GUILayout.Width(400));
            Debug.Log(index);
        }
#else
        //使用UIElements
        private ToolbarMenu resolutionMenu;
        private ToolbarMenu langMenu;
        private ToolbarMenu colorMenu;

        private object GameView;

        private MethodInfo GameViewDoToolBarGUI_Method;
        private MethodInfo GameViewSizePopup_Method;

        private int selectedSizeIndex;
        private GameViewSizeGroupType currentSizeGroupType;

        private void OnEnable()
        {
            InitWindowData();
            InitWindowUI();
            RefreshWindow();
        }

        private void InitWindowData()
        {
            //GameView = Utils.GetGameView();
            GameView = Utils.GetMainPlayModeView();

            GameViewDoToolBarGUI_Method = Utils.GetEditorMethod(Type.GetType("UnityEditor.GameView,UnityEditor"), "DoToolbarGUI");
            GameViewSizePopup_Method = Utils.GetEditorMethod(Type.GetType("UnityEditor.EditorGUILayout,UnityEditor"), "GameViewSizePopup");

            selectedSizeIndex = (int)typeof(Editor).Assembly.GetType("UnityEditor.GameView")
                .GetProperty("selectedSizeIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(GameView);

            currentSizeGroupType = (GameViewSizeGroupType)typeof(Editor).Assembly.GetType("UnityEditor.GameView")
                .GetProperty("currentSizeGroupType", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                .GetValue(GameView);
        }
        private void InitWindowUI()
        {
            var root = this.rootVisualElement;

            var toolbar = new Toolbar();
            toolbar.style.height = 20;
            root.Add(toolbar);

            IMGUIContainer imgContainer = new IMGUIContainer(() =>
            {
                GUILayoutOption[] ops = new GUILayoutOption[] { GUILayout.Width(160f) };
                GameViewSizePopup_Method.Invoke(null, new object[] { currentSizeGroupType, selectedSizeIndex, GameView, EditorStyles.toolbarPopup, ops });
            });
            toolbar.Add(imgContainer);

            //resolutionMenu = new ToolbarMenu { text = "resolution" };
            //for(int i = 0; i< defaultSizeOptions.Length; i++)
            //{
            //    int tmp = i;
            //    resolutionMenu.menu.AppendAction(defaultSizeOptions[i], a => { Debug.Log(defaultSizeOptions[tmp]); }, a => DropdownMenuAction.Status.Normal);    
            //}
            //toolbar.Add(resolutionMenu);



            langMenu = new ToolbarMenu { text = "language" };
            for (int i = 0; i < defaultLangOptions.Length; i++)
            {
                int tmp = i;
                langMenu.menu.AppendAction(defaultLangOptions[i], a => { Debug.Log(defaultLangOptions[tmp]); }, a => DropdownMenuAction.Status.Normal);
            }
            toolbar.Add(langMenu);

            colorMenu = new ToolbarMenu { text = "color" };
            foreach (ColorBlindType type in ColorBlindType.GetValues(typeof(ColorBlindType)))
            {
                ColorBlindType tmp = type;
                colorMenu.menu.AppendAction(ColorBlind.GetColorLocalizationName(tmp), a => { ColorBlind.ToColorBlindMode(tmp); }, a => DropdownMenuAction.Status.Normal);
            }
            toolbar.Add(colorMenu);


            Button closeBtn = new Button();
            closeBtn.style.height = 30;
            closeBtn.style.width = 100;
            closeBtn.style.top = 5;
            closeBtn.style.alignSelf = Align.Center;

            closeBtn.clicked += PreviewLogic.ExitPreview;
            closeBtn.text = "Stop";
            root.Add(closeBtn);
        }

        private void RefreshWindow()
        {
            UpdateToolBarMenuSelection();
        }
        void UpdateToolBarMenuSelection()
        {
            //langMenu.text = defaultLangOptions[0];
        }
#endif
    }
}
#endif