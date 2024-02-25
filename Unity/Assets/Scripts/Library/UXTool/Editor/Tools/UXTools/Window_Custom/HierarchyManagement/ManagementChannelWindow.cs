using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    public class ManagementChannelWindow : EditorWindow
    {
        private static ManagementChannelWindow _mWindow;
        private static ManagementChannel _channel;
        private static string _inputText = "";

        private static Label changedVE;

        static ManagementChannelWindow()
        {
            EditorApplication.playModeStateChanged += (obj) =>
            {
                if (HasOpenInstances<ManagementChannelWindow>())
                    _mWindow = GetWindow<ManagementChannelWindow>();
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

        [UnityEditor.Callbacks.DidReloadScripts(0)]
        private static void OnScriptReload()
        {
            if (HasOpenInstances<ManagementChannelWindow>())
                _mWindow = GetWindow<ManagementChannelWindow>();
        }

        public static ManagementChannelWindow GetInstance()
        {
            return _mWindow;
        }

        private void CloseWindow()
        {
            if (_mWindow != null)
            {
                _mWindow.Close();
            }
        }

        public static void OpenWindow(ManagementChannel channel, Label label)
        {
            int width = 300;
            int height = 100;
            changedVE = label;
            InitWindowData();
            _channel = channel;
            _inputText = channel.Name;
            _mWindow = GetWindow<ManagementChannelWindow>();
            _mWindow.minSize = new Vector2(width, height);
            // _mWindow.position = new Rect((Screen.currentResolution.width - width) / 2,
            //     (Screen.currentResolution.height - height) / 2, width, height);
            _mWindow.titleContent.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_修改Channel名称);
        }

        private static string _okText;
        private static string _cancelText;
        private static void InitWindowData()
        {
            _okText = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定);
            _cancelText = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消);
        }

        private void OnEnable()
        {
            HierarchyManagementWindow.GetInstance()._focusFlag = false;
            VisualElement root = rootVisualElement;

            var div = UXBuilder.Div(root, new UXBuilderDivStruct()
            {
                style = new UXStyle()
                {
                    width = 300,
                    height = 100,
                    alignItems = Align.Center,
                    paddingBottom = 10,
                    paddingLeft = 10,
                    paddingRight = 10,
                    paddingTop = 20,
                }
            });
            var row = UXBuilder.Row(div, new UXBuilderRowStruct()
            {
                align = Align.Center,
                style = new UXStyle() { marginLeft = 10 }
            });

            UXBuilder.Text(row, new UXBuilderTextStruct()
            {
                text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_Channel名称),
                style = new UXStyle()
                { marginRight = 5, unityTextAlign = TextAnchor.MiddleRight, width = Length.Percent(35) }
            });
#if UNITY_2020_3_OR_NEWER
            var input = UXBuilder.Input(row, new UXBuilderInputStruct()
            {
                style = new UXStyle()
                { width = Length.Percent(60) },
                onChange = s =>
                {
                    _inputText = s;
                }
            });
            input.value = _channel.Name;
#else
            var inputDiv = UXBuilder.Div(row, new UXBuilderDivStruct()
            {
                style = new UXStyle() { width = Length.Percent(60), backgroundColor = Color.white},
            });
            _inputText = _channel.Name;
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.black;
            style.fontSize = 14;
            IMGUIContainer input = new IMGUIContainer();
            input.style.overflow = Overflow.Hidden;
            inputDiv.Add(input);
            input.onGUIHandler += () => { _inputText = EditorGUILayout.TextField(_inputText, style); };
#endif
            row = UXBuilder.Row(div, new UXBuilderRowStruct()
            {
                justify = Justify.Center,
                align = Align.Center,
                style = new UXStyle()
                {
                    marginTop = 25,
                }
            });
            UXBuilder.Button(row, new UXBuilderButtonStruct()
            {
                type = ButtonType.Primary,
                text = _okText,
                OnClick = Submit,
                style = new UXStyle() { width = Length.Percent(30), height = 25, fontSize = 14 }
            });
            UXBuilder.Button(row, new UXBuilderButtonStruct()
            {
                text = _cancelText,
                OnClick = CloseWindow,
                style = new UXStyle() { width = Length.Percent(30), height = 25, fontSize = 14, marginLeft = 20 }
            });
        }

        private void Submit()
        {
            if (string.IsNullOrEmpty(_inputText.Trim()))
            {
                _channel.Name = "Channel " + _channel.ID;
            }
            else _channel.Name = _inputText;

            HierarchyManagementWindow.GetInstance()._refreshFlag = true;
            changedVE.text = _channel.Name;
            changedVE.tooltip = _channel.Name;
            UIElementUtils.CutTextElement(changedVE);
            // HierarchyManagementWindow.GetInstance().RefreshPaint();
            CloseWindow();
        }

        private void OnDisable()
        {
            HierarchyManagementWindow.GetInstance()._focusFlag = true;

        }
    }
}