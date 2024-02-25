using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    public struct DeleteWindowStruct
    {
        public string MessageTitle;
        public string MessageDelete;
        public string MessageDeleteLocal;
    }
    public class DeleteWindow : EditorWindow
    {
        private static DeleteWindow _mWindow;
        // private static string _mGuid = "";
        private static Action _actionWithFalse = null;
        private static Action _actionWithTrue = null;
        
        public static void OpenWindow(DeleteWindowStruct messages,Action actionWithFalse, Action actionWithTrue)
        {
            
            _actionWithFalse = actionWithFalse;
            _actionWithTrue = actionWithTrue;
            
            // _mGuid = guid;
            InitWindowData(messages);
            int width = 300;
            int height = 160;
            _mWindow = GetWindow<DeleteWindow>();
            _mWindow.minSize = new Vector2(width, height);
            _mWindow.titleContent.text = messages.MessageTitle;
            _mWindow.position = new Rect((Screen.currentResolution.width - width) / 2,
                (Screen.currentResolution.height - height) / 2, width, height);
        }
        
        private static string _deleteText;
        private static string _deleteLocalText;
        private static string _okText;
        private static string _cancelText;
        private static void InitWindowData(DeleteWindowStruct messages)
        {
            _deleteText = messages.MessageDelete;
            _deleteLocalText = messages.MessageDeleteLocal;
            _okText = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定);
            _cancelText = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消);
        }

        private void OnEnable()
        {
            VisualElement root = rootVisualElement;

            var div = UXBuilder.Div(root, new UXBuilderDivStruct()
            {
                style = new UXStyle()
                {
                    width = 300, height = 160, alignItems = Align.Center,
                    paddingBottom = 20, paddingLeft = 10, paddingRight = 10, paddingTop = 20,
                }
            });
            UXBuilder.Text(div, new UXBuilderTextStruct()
            {
                text = _deleteText,
                style = new UXStyle(){ width = Length.Percent(90), fontSize = 14, marginLeft  = 10}
            });
            var row = UXBuilder.Row(div, new UXBuilderRowStruct()
            {
                align = Align.Center,
                style = new UXStyle()
                {
                    marginTop = 35, marginLeft = 30,
                }
            });
            var checkBox = UXBuilder.CheckBox(row, new UXBuilderCheckBoxStruct()
            {
                style = new UXStyle(){ marginTop = 4 }
            });
            UXBuilder.Text(row, new UXBuilderTextStruct()
            {
                text = _deleteLocalText,
                style = new UXStyle(){ width = Length.Percent(60), fontSize = 14, marginLeft = 5 }
            });
            row = UXBuilder.Row(div, new UXBuilderRowStruct()
            {
                justify = Justify.Center,
                align = Align.Center,
                style = new UXStyle()
                {
                    marginTop = 15,
                }
            });
            UXBuilder.Button(row, new UXBuilderButtonStruct()
            {
                type = ButtonType.Primary,
                text = _okText,
                OnClick = () =>
                {
                    if (checkBox.value)
                    {
                        _actionWithTrue();
                    }
                    else
                    {
                        _actionWithFalse();
                    }
                    CloseWindow();
                },
                style = new UXStyle(){ width = Length.Percent(30), height = 25, fontSize = 14 }
            });
            UXBuilder.Button(row, new UXBuilderButtonStruct()
            {
                text = _cancelText,
                OnClick = CloseWindow,
                style = new UXStyle(){ width = Length.Percent(30), height = 25, fontSize = 14, marginLeft = 20}
            });
        }

        private static void CloseWindow()
        {
            if (_mWindow != null)
            {
                _actionWithFalse = null;
                _actionWithTrue = null;
                _mWindow.Close();
            }
        }
    }
}