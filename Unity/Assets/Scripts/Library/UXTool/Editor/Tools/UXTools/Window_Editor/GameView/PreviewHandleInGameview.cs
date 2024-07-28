#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.Reflection;
using System.Linq;
using UnityEditor.SceneManagement;

namespace ThunderFireUITool
{
    [UXInitialize(100)]
    public class PreviewHandleInGameView
    {
        //private static bool HaveEnteredPlayMode = false;

        public static void Init()
        {
            //Temp:预览的控件会遮挡到运行时的右上角画面,需要重新设计
            EditorApplication.playModeStateChanged += PlayModeStateChanged;
        }

        //[MenuItem("ThunderFireUXTool/preview/show handle")]
        public static void ShowPreviewHandle()
        {
            Show();
        }

        //[MenuItem("ThunderFireUXTool/preview/hide handle")]
        public static void HidePreviewHandle()
        {
            Hide();
        }

        private static List<VisualElement> handleList = new List<VisualElement>();
        private static PopupField<string> colorMenu;
        private static PopupField<string> animationMenu;

        private static Dictionary<string, ColorBlindType> colorOptions = new Dictionary<string, ColorBlindType>();
        public static List<string> animOptions = new List<string>();
        private static Animator animator;

        private object GameView;
        private MethodInfo GameViewDoToolBarGUI_Method;
        private MethodInfo GameViewSizePopup_Method;
        private int selectedSizeIndex;
        private GameViewSizeGroupType currentSizeGroupType;

        static void PlayModeStateChanged(PlayModeStateChange obj)
        {
            if (obj == PlayModeStateChange.EnteredPlayMode)
            {
                Hide();
                Show();
            }
            else if (obj == PlayModeStateChange.EnteredEditMode)
            {
                Hide();
            }
        }

        public static void Show()
        {
            InitWindowData();
            InitWindowUI();
            RefreshWindow();
        }

        public static void Hide()
        {
            foreach (var handle in handleList)
            {
                handle.RemoveFromHierarchy();
            }
        }

        private static void InitWindowData()
        {
            //Todo: init colorOptions
            colorOptions.Clear();
            foreach (ColorBlindType type in ColorBlindType.GetValues(typeof(ColorBlindType)))
            {
                colorOptions.Add(ColorBlind.GetColorLocalizationName(type), type);
            }
            //Todo: init animOptions

        }
        private static void InitWindowUI()
        {
            var gameViews = Utils.GetPlayViews();
            foreach (EditorWindow gameView in gameViews)
            {
                var m_root = new VisualElement();
                m_root.style.flexDirection = FlexDirection.RowReverse;
                m_root.style.justifyContent = Justify.FlexEnd;
                m_root.style.top = 21;
                m_root.style.right = 110;
                m_root.style.position = Position.Absolute;

                var m_ColorPanel = new VisualElement();
                m_ColorPanel.style.backgroundColor = new Color(60f / 255, 60f / 255, 60f / 255, 1);
                m_ColorPanel.style.flexDirection = FlexDirection.Row;
                m_ColorPanel.style.justifyContent = Justify.FlexEnd;
                m_ColorPanel.style.alignItems = Align.Center;

                var colorLabel = new TextElement();
                colorLabel.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_色盲模式);
                colorLabel.style.width = 87;
                colorLabel.style.fontSize = 12;
                colorLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
                colorLabel.style.color = Color.white;
                colorLabel.style.paddingRight = colorLabel.style.paddingLeft;
                m_ColorPanel.Add(colorLabel);

                colorMenu = new PopupField<string>(colorOptions.Keys.ToList(), ColorBlind.GetColorLocalizationName(ColorBlindType.Normal));
                colorMenu.style.height = 18;
                colorMenu.style.width = 100;
                colorMenu.style.fontSize = 12;
                colorMenu.style.color = new Color(228, 228, 228, 255);
                colorMenu.RegisterValueChangedCallback((evt) => { UpdateColorMenuSelection(evt.newValue); });
                colorMenu.style.marginLeft = 0;
                m_ColorPanel.Add(colorMenu);

                m_root.Add(m_ColorPanel);

                if (animOptions.Count > 0)
                {
                    var m_AnimationPanel = new VisualElement();
                    m_AnimationPanel.style.backgroundColor = new Color(60f / 255, 60f / 255, 60f / 255, 1);
                    m_AnimationPanel.style.flexDirection = FlexDirection.Row;
                    m_AnimationPanel.style.justifyContent = Justify.FlexEnd;

                    var animLabel = new TextElement();
                    animLabel.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_动画预览);
                    animLabel.style.width = 107;
                    animLabel.style.fontSize = 12;
                    animLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
                    animLabel.style.color = Color.white;
                    m_AnimationPanel.Add(animLabel);

                    animationMenu = new PopupField<string>(animOptions, animOptions[0]);
                    animationMenu.style.height = 18;
                    animationMenu.style.width = 100;
                    animationMenu.style.fontSize = 12;
                    animationMenu.style.color = new Color(228, 228, 228, 255);
                    animationMenu.contentContainer.style.color = Color.black;
                    animationMenu.RegisterValueChangedCallback((evt) => { UpdateAnimationMenuSelection(evt.newValue); });
                    animationMenu.style.marginLeft = 0;
                    m_AnimationPanel.Add(animationMenu);

                    m_root.Add(m_AnimationPanel);
                }

                gameView.rootVisualElement.Add(m_root);
                m_root.BringToFront();

                handleList.Add(m_root);
            }
        }
        private static void RefreshWindow()
        {
            UpdateColorMenuSelection(ColorBlind.GetColorLocalizationName(ColorBlindType.Normal));
            if (animOptions.Count > 0)
                UpdateAnimationMenuSelection(animOptions[0]);
        }

        private static void UpdateColorMenuSelection(string colorName)
        {
            ColorBlind.ToColorBlindMode(colorOptions[colorName]);
            if (colorMenu != null)
            {
                colorMenu.SetValueWithoutNotify(colorName);
            }
        }

        private static void UpdateAnimationMenuSelection(string animName)
        {
            Debug.Log("开始播放动画 : " + animName);
            animationMenu.SetValueWithoutNotify(animName);
        }
    }
}
#endif