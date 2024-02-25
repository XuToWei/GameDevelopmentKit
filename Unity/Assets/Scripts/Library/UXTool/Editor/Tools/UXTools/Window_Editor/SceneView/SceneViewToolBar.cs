#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Cursor = UnityEngine.Cursor;

namespace ThunderFireUITool
{

    [UXInitialize]
    public class SceneViewToolBar : EditorWindow
    {
        //toolbar根节点,黑色条
        public static VisualElement toolbarBg;
        //toolbar面板
        private static VisualElement toolBarPanel;

        private static VisualElement createTextBtn;
        private static VisualElement createImageBtn;

        private static VisualElement HideBarBtn;
        private static VisualElement ShowBarBtn;

        private static bool OverToolBarBg;
        private static bool OverToolBar;
        public static bool HaveToolbar;

        private static EditorLogic editorLogic;
        static SceneViewToolBar()
        {
            //检查和恢复recompile之后的toolbar状态
            TryOpenToolbar();
            EditorApplication.update += InitFunction;
        }

        public static void InitFunction()
        {
            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView == null) return;
            editorLogic = new EditorLogic();
            editorLogic.Init();
            PrefabTabs.InitPrefabTabs();
            ResolutionController.InitResolutionController();
            if (ResolutionController.loaded)
            {
                sceneView.rootVisualElement.Add(ResolutionController.Root);
            }
            EditorApplication.update -= InitFunction;
        }

        public static void CloseFunction()
        {
            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView == null) return;

            if (sceneView.rootVisualElement.Contains(PrefabTabs.prefabTabsPanel))
            {
                sceneView.rootVisualElement.Remove(PrefabTabs.prefabTabsPanel);
            }
            if (ResolutionController.loaded)
            {
                if (sceneView.rootVisualElement.Contains(ResolutionController.Root))
                {
                    sceneView.rootVisualElement.Remove(ResolutionController.Root);
                }
            }
            editorLogic.Close();
        }

        public static void CloseEditor()
        {
            if (HaveToolbar && toolbarBg != null)
            {
                SceneView sceneView = SceneView.lastActiveSceneView;
                if (sceneView == null) return;

                if (sceneView.rootVisualElement.Contains(toolbarBg))
                {
                    sceneView.rootVisualElement.Remove(toolbarBg);
                }
                HaveToolbar = false;
            }
        }

        public static void OpenEditor()
        {
            //在播放预制体的时候打开编辑器，不修改判断播放预制体的值(临时处理手法)
            if (EditorApplication.isPlaying != true)
            {
                PlayerPrefs.SetString("previewStage", "false");
            }
            if (!HaveToolbar)
            {
                InitToolBar();
                HaveToolbar = true;
            }

            UXToolAnalysis.SendUXToolLog(UXToolAnalysisLog.ToolBar);
            UXSceneViewCursor.Instance.Init();
            SceneView.lastActiveSceneView.in2DMode = true;
        }

        [MenuItem(ThunderFireUIToolConfig.Menu_ToolBar, false, 101)]
        public static void SwitchEditor()
        {
            bool flag = Menu.GetChecked(ThunderFireUIToolConfig.Menu_ToolBar);
            Menu.SetChecked(ThunderFireUIToolConfig.Menu_ToolBar, !flag);
            EditorPrefs.SetBool("EditorOpen", !flag);

            if (!flag)
            {
                //点击之后变为开启状态
                OpenEditor();
            }
            else
            {
                //点击之后变为关闭状态
                CloseEditor();
            }
        }

        [MenuItem(ThunderFireUIToolConfig.Menu_ToolBar, true)]
        public static bool CheckToolBarState()
        {
            Menu.SetChecked(ThunderFireUIToolConfig.Menu_ToolBar, HaveToolbar);
            return true;
        }

        public static void TryOpenToolbar()
        {
            //检查EditorOpen是否开启 
            //检查是否已经有一个实例了
            //检查是否有SceneView

            bool open = EditorPrefs.GetBool("EditorOpen", false);
            if (open && !HaveToolbar && SceneView.lastActiveSceneView != null)
            {
                OpenEditor();
            }
        }
        private static void InitToolBar()
        {
            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView == null) return;

            VisualTreeAsset toolbarTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ThunderFireUIToolConfig.UIBuilderPath + "toolbar.uxml");
            VisualTreeAsset toolbarPopUpTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ThunderFireUIToolConfig.UIBuilderPath + "toolbar_secondPopup.uxml");

            toolbarBg = toolbarTreeAsset.CloneTree().Children().First();
            sceneView.rootVisualElement.Add(toolbarBg);
            toolbarBg.style.position = Position.Absolute;
            toolbarBg.style.bottom = 0;
            toolbarBg.BringToFront();

            toolBarPanel = toolbarBg.Q<VisualElement>("toolbar");
            toolBarPanel.style.alignSelf = Align.Center;

            VisualElement quickBgBtn = toolbarBg.Q<VisualElement>("qucikBg");
            quickBgBtn.tooltip = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_参考背景图片);
            quickBgBtn.RegisterCallback((MouseDownEvent e) =>
            {
                QuickBackground.CreateBackGround();
                StopQuickCreate();
            });
            //quickBgBtn Hover时要换图, 因为图片线条只改颜色看的不清楚
            //quickBgBtn.RegisterCallback((MouseEnterEvent e) =>
            //{
            //    SetButtonHoverState(quickBgBtn, true);
            //    quickBgBtn.Q<VisualElement>("Icon").style.backgroundImage = (StyleBackground)ToolUtils.GetIcon("ToolBar/quickbackground_white");
            //});
            //quickBgBtn.RegisterCallback((MouseLeaveEvent e) =>
            //{
            //    SetButtonHoverState(quickBgBtn, false);
            //    quickBgBtn.Q<VisualElement>("Icon").style.backgroundImage = (StyleBackground)ToolUtils.GetIcon("ToolBar/quickbackground_black");
            //});
            RegisterMouseHover(quickBgBtn);

            VisualElement recentOpenBtn = toolbarBg.Q<VisualElement>("clock");
            recentOpenBtn.tooltip = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_最近打开的模板);
            recentOpenBtn.RegisterCallback((MouseDownEvent e) =>
            {
                PrefabRecentWindow.OpenWindow();
                StopQuickCreate();
            });
            RegisterMouseHover(recentOpenBtn);

            VisualElement playBtn = toolbarBg.Q<VisualElement>("play");
            playBtn.tooltip = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_预览);
            playBtn.RegisterCallback((MouseDownEvent e) =>
            {
                PreviewLogic.Preview();
                StopQuickCreate();
            });
            RegisterMouseHover(playBtn);

            createImageBtn = toolbarBg.Q<VisualElement>("createImage");
            createImageBtn.tooltip = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_创建图片);
            createImageBtn.RegisterCallback((MouseDownEvent e) =>
            {
                if (InQuickCreateState && QuickCreateType == "Image")
                {
                    StopQuickCreate();
                }
                else if (InQuickCreateState && QuickCreateType != "Image")
                {
                    StopQuickCreate();
                    StartQuickCreate("Image");
                }
                else
                {
                    StartQuickCreate("Image");
                }
            });
            createImageBtn.RegisterCallback((MouseEnterEvent e) =>
            {
                //快速创建Image状态下不再受鼠标hover影响
                if (InQuickCreateState && QuickCreateType == "Image") return;

                SetButtonHoverState(createImageBtn, true);
            });
            createImageBtn.RegisterCallback((MouseLeaveEvent e) =>
            {
                //快速创建Image状态下不再受鼠标hover影响
                if (InQuickCreateState && QuickCreateType == "Image") return;

                SetButtonHoverState(createImageBtn, false);
            });

            createTextBtn = toolbarBg.Q<VisualElement>("createText");
            createTextBtn.tooltip = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_创建文字);
            createTextBtn.RegisterCallback((MouseDownEvent e) =>
            {
                if (InQuickCreateState && QuickCreateType == "Text")
                {
                    StopQuickCreate();
                }
                else if (InQuickCreateState && QuickCreateType != "Text")
                {
                    StopQuickCreate();
                    StartQuickCreate("Text");
                }
                else
                {
                    StartQuickCreate("Text");
                }

            });
            createTextBtn.RegisterCallback((MouseEnterEvent e) =>
            {
                //快速创建Image状态下不再受鼠标hover影响
                if (InQuickCreateState && QuickCreateType == "Text") return;

                SetButtonHoverState(createTextBtn, true);
            });
            createTextBtn.RegisterCallback((MouseLeaveEvent e) =>
            {
                //快速创建Image状态下不再受鼠标hover影响
                if (InQuickCreateState && QuickCreateType == "Text") return;

                SetButtonHoverState(createTextBtn, false);
            });


            VisualElement guideLineBtn = toolbarBg.Q<VisualElement>("guideLine");
            guideLineBtn.tooltip = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_辅助线);
            guideLineBtn.RegisterCallback((MouseDownEvent e) =>
            {
                ShowGuideLinePopUp();
                StopQuickCreate();
            });
            RegisterMouseHover(guideLineBtn);

            VisualElement widgetRepoBtn = toolbarBg.Q<VisualElement>("widgetRepo");
            widgetRepoBtn.tooltip = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_组件库);
            widgetRepoBtn.RegisterCallback((MouseDownEvent e) =>
            {
                WidgetRepositoryWindow.OpenWindow();
                StopQuickCreate();
            });
            RegisterMouseHover(widgetRepoBtn);

            VisualElement createWidgetBtn = toolbarBg.Q<VisualElement>("createWidget");
            createWidgetBtn.tooltip = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_新建组件);
            createWidgetBtn.RegisterCallback((MouseDownEvent e) =>
            {
                PrefabCreateWindow.OpenWindow();
                StopQuickCreate();
            });
            RegisterMouseHover(createWidgetBtn);

            VisualElement moreBtn = toolbarBg.Q<VisualElement>("more");
            moreBtn.tooltip = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_更多);
            moreBtn.RegisterCallback((MouseDownEvent e) =>
            {
                ShowMorePopUp();
                StopQuickCreate();
            });
            RegisterMouseHover(moreBtn);


            HideBarBtn = toolbarBg.Q<VisualElement>("HideBtn");
            ShowBarBtn = toolbarBg.Q<VisualElement>("ShowBtn");
            HideBarBtn.RegisterCallback((MouseDownEvent e) =>
            {
                SetToobarExpandState(false);
            });
            ShowBarBtn.RegisterCallback((MouseDownEvent e) =>
            {
                SetToobarExpandState(true);
            });

            toolbarBg.RegisterCallback((PointerOverEvent e) =>
            {
                OverToolBarBg = true;
            });
            toolbarBg.RegisterCallback((PointerOutEvent e) =>
            {
                OverToolBarBg = false;
            });
            toolBarPanel.RegisterCallback((PointerOverEvent e) =>
            {
                OverToolBar = true;
            });
            toolBarPanel.RegisterCallback((PointerOutEvent e) =>
            {
                OverToolBar = false;
            });
        }

        private static void RegisterMouseHover(VisualElement element)
        {
            element.RegisterCallback((MouseEnterEvent e) =>
            {
                SetButtonHoverState(element, true);
            });
            element.RegisterCallback((MouseLeaveEvent e) =>
            {
                SetButtonHoverState(element, false);
            });
        }

        public static void ShowMorePopUp()
        {
            ToolbarPopup popup = new ToolbarPopup();

            VisualElement moreBtn = toolbarBg.Q<VisualElement>("more");
            Vector2 pos = new Vector2(moreBtn.worldBound.x + moreBtn.worldBound.width, toolBarPanel.worldBound.y - toolBarPanel.worldBound.height);

            List<ToolbarPopupOption> list = new List<ToolbarPopupOption>();

            ToolbarPopupOption settingOption = new ToolbarPopupOption()
            {
                text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_设置),
                iconPath = "ToolBar/setting",
                action = () => { ConfigurationWindow.OpenWindow(); }
            };

            ToolbarPopupOption aboutOption = new ToolbarPopupOption()
            {
                text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_关于),
                iconPath = "ToolBar/about",
                action = () => { AboutWindow.OpenWindow(); }
            };
            list.Add(settingOption);
            list.Add(aboutOption);
            popup.Init(pos, list);
        }

        public static void ShowGuideLinePopUp()
        {
            ToolbarPopup popup = new ToolbarPopup();

            VisualElement guideLineBtn = toolbarBg.Q<VisualElement>("guideLine");
            Vector2 pos = new Vector2(guideLineBtn.worldBound.x + guideLineBtn.worldBound.width, toolBarPanel.worldBound.y - toolBarPanel.worldBound.height);

            List<ToolbarPopupOption> list = new List<ToolbarPopupOption>();

            ToolbarPopupOption bothGuideLineOption = new ToolbarPopupOption()
            {
                text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_横纵辅助线),
                iconPath = "ToolBar/referenceline",
                action = () => { LocationLineLogic.Instance.CreateLocationLine(CreateLineType.Both); }
            };

            ToolbarPopupOption vertGuideLineOption = new ToolbarPopupOption()
            {
                text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_水平辅助线),
                iconPath = "ToolBar/referenceline_horizontal",
                action = () => { LocationLineLogic.Instance.CreateLocationLine(CreateLineType.Horizon); }
            };

            ToolbarPopupOption horzGuideLineOption = new ToolbarPopupOption()
            {
                text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_垂直辅助线),
                iconPath = "ToolBar/referenceline_vertical",
                action = () => { LocationLineLogic.Instance.CreateLocationLine(CreateLineType.Vertical); }
            };
            list.Add(bothGuideLineOption);
            list.Add(vertGuideLineOption);
            list.Add(horzGuideLineOption);
            popup.Init(pos, list, 200);
        }

        private static void SetButtonHoverState(VisualElement button, bool isHover)
        {
            if (button == null) return;

            if (isHover)
            {
                button.style.backgroundColor = ThunderFireUIToolConfig.hoverColor;

                //var icon = button.Q<VisualElement>("icon");
                //if (icon != null)
                //{
                //    icon.style.backgroundColor = new Color(0.14f, 0.39f, 0.76f, 1f);
                //    icon.style.unityBackgroundImageTintColor = Color.white;
                //}
            }
            else
            {
                button.style.backgroundColor = ThunderFireUIToolConfig.normalColor;

                //var icon = button.Q<VisualElement>("icon");
                //if (icon != null)
                //{
                //    icon.style.backgroundColor = Color.white;
                //    icon.style.unityBackgroundImageTintColor = Color.black;
                //}
            }
        }

        private static void SetToobarExpandState(bool expand)
        {

            if (expand)
            {
                toolBarPanel.style.visibility = Visibility.Visible;
                HideBarBtn.style.visibility = Visibility.Visible;
                ShowBarBtn.style.visibility = Visibility.Hidden;
                toolbarBg.style.bottom = 0;
            }
            else
            {
                toolBarPanel.style.visibility = Visibility.Hidden;
                HideBarBtn.style.visibility = Visibility.Hidden;
                ShowBarBtn.style.visibility = Visibility.Visible;
                toolbarBg.style.bottom = -18f;
            }
        }

        #region Quick Create 

        static Vector2 mouseDownPos;
        static bool InQuickCreateState = false;
        static bool InCreateDrag = false;
        static string QuickCreateType = null;

        static GameObject[] selection = null;

        public static void StartQuickCreate(string type)
        {
            InQuickCreateState = true;
            QuickCreateType = type;
            UXCustomSceneView.RemoveDelegate(OnQuickCreate);
            EditorApplication.delayCall += () =>
            {
                UXCustomSceneView.AddDelegate(OnQuickCreate);
            };

            UXSceneViewCursor.Instance.SetCursor(UXCursorType.Crosshair);

            if (QuickCreateType == "Image")
            {
                SetButtonHoverState(createImageBtn, true);
            }

            if (QuickCreateType == "Text")
            {
                SetButtonHoverState(createTextBtn, true);
            }
        }

        public static void StopQuickCreate()
        {
            InQuickCreateState = false;
            QuickCreateType = "";

            UXCustomSceneView.RemoveDelegate(OnQuickCreate);
            UXSceneViewCursor.Instance.SetCursor(UXCursorType.None);

            SetButtonHoverState(createImageBtn, false);
            SetButtonHoverState(createTextBtn, false);
        }

        public static void OnQuickCreate(SceneView sceneView)
        {
            //点下鼠标开始拖动
            if (Event.current.type == EventType.MouseDown)
            {
                StartCreateDarg(sceneView);
            }

            if (InCreateDrag)
            {
                Handles.BeginGUI();
                GUI.Box(new Rect(mouseDownPos.x, mouseDownPos.y, Event.current.mousePosition.x - mouseDownPos.x, Event.current.mousePosition.y - mouseDownPos.y), "");
                Handles.EndGUI();
                //抬起鼠标或光标离开合法范围即完成框选
                if (Event.current.type == EventType.MouseUp || OutSceneViewBounds(sceneView))
                {
                    EndCreateDarg(sceneView);
                }
            }
            HandleUtility.AddDefaultControl(0);
            sceneView.Repaint();
        }

        public static void StartCreateDarg(SceneView sceneView)
        {
            mouseDownPos = Event.current.mousePosition;
            selection = Selection.gameObjects;
            Selection.objects = new UnityEngine.Object[0];
            InCreateDrag = true;
        }

        public static void EndCreateDarg(SceneView sceneView)
        {
            Cursor.visible = true;
            InCreateDrag = false;
            //找出视图上方ribbon的间距，解决y值的offset，可能以后有用
            //var style = (GUIStyle)"GV Gizmo DropDown";
            //Vector2 ribbon = style.CalcSize(sceneView.titleContent);
            //ribbon.y equal 18
            //把屏幕坐标转为世界坐标
            Transform parent = FindContainerLogic.GetObjectParent(selection);
            Vector2 startScreenPos = new Vector2(mouseDownPos.x, sceneView.camera.pixelHeight - mouseDownPos.y);
            Vector2 endScreenPos = new Vector2(Event.current.mousePosition.x, sceneView.camera.pixelHeight - Event.current.mousePosition.y);

            Vector3 startWorldPos = sceneView.camera.ScreenToWorldPoint(startScreenPos);
            Vector3 endWorldPos = sceneView.camera.ScreenToWorldPoint(endScreenPos);

            Vector2 startPos = parent.InverseTransformPoint(startWorldPos);
            Vector2 endPos = parent.InverseTransformPoint(endWorldPos);

            //RectTransform rect = parent as RectTransform;
            //Canvas canvas = parent.GetComponentsInParent<Canvas>(true)[0];
            //Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
            //RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, startScreenPos, cam, out startPos);
            //RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, endScreenPos, cam, out endPos);


            //Vector3 startPos = sceneView.camera.ScreenToWorldPoint(new Vector3(mouseDownPos.x, sceneView.camera.pixelHeight - mouseDownPos.y, 0));
            //Vector3 endPos = sceneView.camera.ScreenToWorldPoint(new Vector3(Event.current.mousePosition.x, sceneView.camera.pixelHeight - Event.current.mousePosition.y, 0));
            Vector2 size = new Vector2(System.Math.Abs(startPos.x - endPos.x), System.Math.Abs(startPos.y - endPos.y));
            if (size.x == 0 || size.y == 0)
            {
                size.x = 100;
                size.y = 100;
            }

            Vector3 localPosition = new Vector3((startPos.x + endPos.x) / 2, (startPos.y + endPos.y) / 2, 0);

            GameObject obj = WidgetGenerator.CreateUIObj(QuickCreateType, localPosition, size, selection);
            if (obj)
            {
                switch (QuickCreateType)
                {
                    case "Image":
                        Assembly imageAssembly;
                        try
                        {
                            imageAssembly = Assembly.Load("UXTools.Runtime");
                        }
                        catch (System.IO.FileNotFoundException)
                        {
                            imageAssembly = Assembly.Load(ThunderFireUIToolConfig.ImageAssemblyName);
                        }
                        Type imageType = imageAssembly.GetType(ThunderFireUIToolConfig.ImageClassName);
                        Undo.AddComponent(obj, imageType);
                        break;
                    case "Text":
                        Assembly textAssembly;
                        try
                        {
                            textAssembly = Assembly.Load("UXTools.Runtime");
                        }
                        catch (System.IO.FileNotFoundException)
                        {
                            textAssembly = Assembly.Load(ThunderFireUIToolConfig.TextAssemblyName);
                        }
                        Type textType = textAssembly.GetType(ThunderFireUIToolConfig.TextClassName);
                        Undo.AddComponent(obj, textType);
                        break;
                }
            }
            //生成节点被选中
            Selection.activeObject = obj;

            StopQuickCreate();
        }


        //当前鼠标是否在Toolbar的范围上
        public static bool OverToolbarRange()
        {
            return OverToolBarBg || OverToolBar;
        }
        //当前鼠标是否离开SceneView中的合法位置
        public static bool OutSceneViewBounds(SceneView sceneView)
        {
            bool OutOfSceneViewBounds = Event.current.mousePosition.y > sceneView.camera.pixelHeight ||
                                        Event.current.mousePosition.x > sceneView.camera.pixelWidth ||
                                        Event.current.mousePosition.y < 0 ||
                                        Event.current.mousePosition.x < 0;


            if (OutOfSceneViewBounds || OverToolbarRange())
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        #endregion
    }
}
#endif