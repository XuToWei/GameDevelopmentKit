using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    [UXInitialize(100)]
    public class GameviewPanel
    {
        private static Button button;

        static GameviewPanel()
        {
            PreviewHandleInGameView.Init();
            EditorApplication.playModeStateChanged += (PlayModeStateChange obj) =>
            {
                if(button == null)
                {
                    button = new Button();
                    button.style.position = Position.Absolute;
                    button.style.top = 20;
                    button.style.right = PreviewHandleInGameView.animOptions.Count > 0 ? 510 : 300;
                    button.style.width = 15;
                    button.style.height = 20;
                    button.style.paddingLeft = button.style.paddingRight = button.style.marginRight = 0;
                    button.style.justifyContent = Justify.Center;
                    button.style.alignItems = Align.Center;
                    VisualElement visual = new VisualElement();
                    visual.style.width = 6;
                    visual.style.height = 10;
                    visual.style.backgroundImage = (Texture2D)ToolUtils.GetIcon("UI_Play");
                    button.Add(visual);
                    button.clickable.activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
                    button.clicked += () => {
                        if(button.style.right != 0)
                        {
                            LanguageController.HidePanel();
                            PreviewHandleInGameView.Hide();
                            button.style.right = 0;
                            visual.style.backgroundImage = (Texture2D)ToolUtils.GetIcon("UI_PlayReverse");
                        }
                        else
                        {
                            LanguageController.ShowPanel();
                            PreviewHandleInGameView.Show();
                            button.style.right = PreviewHandleInGameView.animOptions.Count > 0 ? 510 : 300;
                            visual.style.backgroundImage = (Texture2D)ToolUtils.GetIcon("UI_Play");
                        }
                    };
                }

                if (obj == PlayModeStateChange.EnteredPlayMode)
                {
                    var gameViews = Utils.GetPlayViews();
                    foreach (EditorWindow gameView in gameViews)
                    {
                        gameView.rootVisualElement.Add(button);
                    }
                }
                else if (obj == PlayModeStateChange.ExitingPlayMode)
                {
                    var gameViews = Utils.GetPlayViews();
                    foreach (EditorWindow gameView in gameViews)
                    {
                        if(gameView.rootVisualElement.Contains(button))
                        {
                            gameView.rootVisualElement.Remove(button);
                        }
                    }
                }
            };
        }
    }
}