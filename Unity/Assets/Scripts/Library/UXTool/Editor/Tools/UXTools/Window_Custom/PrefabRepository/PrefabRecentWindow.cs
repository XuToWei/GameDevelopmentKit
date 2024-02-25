#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using ThunderFireUITool;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


namespace ThunderFireUITool
{
    public class PrefabRecentWindow : EditorWindow
    {
        private static PrefabRecentWindow r_window;
        public static bool clickFlag = false;

        [MenuItem(ThunderFireUIToolConfig.Menu_RecentlyOpened, false, 151)]

        public static void OpenWindow()
        {
            int width = 732 + 12 + 13;
            int height = 468 + 30;
            r_window = GetWindow<PrefabRecentWindow>();
            r_window.minSize = new Vector2(width, height);
            //m_window.position = GUIHelper.GetEditorWindowRect().AlignCenter(510, 220);
            //r_window.position = new Rect((Screen.currentResolution.width - width) / 2, (Screen.currentResolution.height - height) / 2, width, height);
            r_window.titleContent.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_最近打开的模板);
            r_window.titleContent.image = ToolUtils.GetIcon("clock_w");
            UXToolAnalysis.SendUXToolLog(UXToolAnalysisLog.RecentlyOpen);
        }

        static PrefabRecentWindow()
        {
            EditorApplication.playModeStateChanged += (obj) =>
            {
                if (HasOpenInstances<PrefabRecentWindow>())
                    r_window = GetWindow<PrefabRecentWindow>();
                if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    if (r_window)
                        r_window.RefreshWindow();
                }
            };
        }

        private VisualElement leftContainer;
        private VisualElement container;
        private PrefabOpenedSetting list;
        private ScrollView scroll;
        private List<string> List;
        private List<AssetsItem> assetsItems = new List<AssetsItem>();
        private List<FileInfo> prefabInfoList;
        private GameObject LoadPrefab = null;
        Texture texture = null;
        private void OnEnable()
        {
            if (!SwitchSetting.CheckValid(SwitchSetting.SwitchType.RecentlyOpened)) return;
            InitWindowData();
            InitWindowUI();
            EditorApplication.delayCall += RefreshWindow;
            EditorApplication.hierarchyWindowItemOnGUI += (int instanceID, Rect selectionRect) =>
            {
                if (Event.current.type == EventType.MouseDrag)
                {
                    UXCustomSceneView.ClearDelegate();
                }
            };
        }

        private void InitWindowData()
        {
            list = JsonAssetManager.GetAssets<PrefabOpenedSetting>();

            List = list.List;

            assetsItems.Clear();

            prefabInfoList = GetPrefabList();
            for (int i = 0; i < prefabInfoList.Count; i++)
            {
                AssetsItem item = new AssetsItem(prefabInfoList[i], true);
                assetsItems.Add(item);
            }
        }

        private void InitWindowUI()
        {
            VisualElement root = rootVisualElement;
            root.style.paddingBottom = 36;
            root.style.paddingTop = 36;
            root.style.paddingLeft = 36;
            root.style.paddingRight = 36;
            leftContainer = UXBuilder.Row(root, new UXBuilderRowStruct()
            {
                style = new UXStyle() { height = Length.Percent(100) }
            });

            scroll = UXBuilder.ScrollView(leftContainer, new UXBuilderScrollViewStruct()
            {
                style = new UXStyle() { width = Length.Percent(100) }
            });
            var ve = scroll.contentContainer;
            ve.style.flexDirection = FlexDirection.Row;
            ve.style.flexWrap = Wrap.Wrap;
            ve.style.overflow = Overflow.Visible;
            scroll.RegisterCallback<MouseDownEvent>(evt =>
            {
                if (clickFlag)
                {
                    clickFlag = false;
                }
                else
                {
                    for (int i = 0; i < assetsItems.Count; i++)
                    {
                        assetsItems[i].SetSelected(false);
                    }
                }
            });
        }

        public void RefreshWindow()
        {
            InitWindowData();
            scroll.Clear();

            var btn = UXBuilder.Button(scroll, new UXBuilderButtonStruct()
            {
                OnClick = RecentCreateWindow.OpenWindow,
                style = new UXStyle()
                {
                    width = 156,
                    height = 156,
                    marginTop = Length.Percent(0),
                    marginLeft = Length.Percent(0),
                    marginBottom = 12,
                    marginRight = 12,
                    alignItems = Align.Center,
                    justifyContent = Justify.Center,
                    fontSize = 50,
                    color = Color.white,
                },
                text = "+",
            });
            btn.tooltip = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_新建模板);

            if (assetsItems.Count == 0) return;
            for (int i = 0; i < assetsItems.Count; i++)
            {
                int tmp = i;

                assetsItems[i].RegisterCallback((MouseDownEvent e) =>
                {
                    for (int j = 0; j < assetsItems.Count; j++)
                        assetsItems[j].SetSelected(false);
                    assetsItems[tmp].SetSelected(true);
                    DragAndDrop.PrepareStartDrag();
                    DragAndDrop.StartDrag("prefab");
                    LoadPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetsItems[tmp].path);
                    string guid = AssetDatabase.AssetPathToGUID(assetsItems[tmp].path);
                    texture = Utils.GetAssetsPreviewTexture(guid);
                    UnityEngine.Object[] obj = { LoadPrefab };
                    DragAndDrop.objectReferences = obj;
                    UXCustomSceneView.ClearDelegate();
                    UXCustomSceneView.AddDelegate(CustomScene);
                });
                scroll.Add(assetsItems[i]);
            }
            Repaint();
        }



        private void ReLayoutLeftContainer()
        {
            if (assetsItems.Count == 0) return;
            List<FileInfo> tmp1 = GetPrefabList();
            // ||(tmp1.All(prefabInfoList.Contains) && tmp1.Count != prefabInfoList.Count)
            if (tmp1.Count != prefabInfoList.Count)
            {
                RefreshWindow();
            }
        }



        private List<FileInfo> GetPrefabList()
        {
            List<FileInfo> prefabList = new List<FileInfo>();
            for (int i = List.Count - 1; i >= 0; i--)
            {
                //AssetDatabase.Refresh();
                string path = AssetDatabase.GUIDToAssetPath(List[i]);
                if (!File.Exists(path) || path == "")
                {
                    list.Remove(List[i]);
                }
                else
                {
                    prefabList.Add(new FileInfo(path));
                }

            }
            return prefabList;

        }

        [UnityEditor.Callbacks.DidReloadScripts(0)]
        private static void OnScriptReload()
        {
            if (HasOpenInstances<PrefabRecentWindow>())
                r_window = GetWindow<PrefabRecentWindow>();

        }
        public static PrefabRecentWindow GetInstance()
        {
            return r_window;
        }


        private void OnGUI()
        {
            Event e = Event.current;
            switch (e.type)
            {
                case EventType.Layout:
                    // Debug.Log("Layout");
                    ReLayoutLeftContainer();
                    break;
                case EventType.MouseEnterWindow:
                    //Debug.Log("MouseEnterWindow");
                    break;
                case EventType.MouseLeaveWindow:
                    //Debug.Log("MouseLeaveWindow");
                    break;
                case EventType.KeyDown:
                    if (Event.current.keyCode == KeyCode.Delete)
                    {
                        var assetItem = assetsItems.Find(s => s.Selected);
                        assetItem?.OpenDeleteRecent();
                    }
                    break;
            }
        }

        private void CustomScene(SceneView sceneView)
        {
            if ((Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                UXCustomSceneView.RemoveDelegate(DrawTexture);
                UXCustomSceneView.AddDelegate(DrawTexture);
                Transform container = FindContainerLogic.GetObjectParent(Selection.gameObjects);
                Vector2 mousePos = Event.current.mousePosition;
                mousePos.y = sceneView.camera.pixelHeight - mousePos.y;
                if (Event.current.type == EventType.DragPerform)
                {
                    if (container != null)
                    {
                        GameObject currentPrefab = PrefabUtility.InstantiatePrefab(LoadPrefab) as GameObject;
                        currentPrefab.transform.SetParent(container);
                        Vector3 WorldPos = sceneView.camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0));
                        Vector3 localPos = container.InverseTransformPoint(new Vector3(WorldPos.x, WorldPos.y, 0));
                        currentPrefab.transform.localPosition = localPos;
                        Selection.activeObject = currentPrefab;
                    }
                    LoadPrefab = null;
                    UXCustomSceneView.RemoveDelegate(DrawTexture);
                    UXCustomSceneView.RemoveDelegate(CustomScene);
                }

                Event.current.Use();
            }

        }


        private void DrawTexture(SceneView sceneView)
        {
            if (SceneViewToolBar.OutSceneViewBounds(sceneView))
            {
                UXCustomSceneView.RemoveDelegate(DrawTexture);
            }
            Handles.BeginGUI();
            GUI.DrawTexture(new Rect(Event.current.mousePosition.x - texture.width / 2, Event.current.mousePosition.y - texture.height / 2, texture.width, texture.height), texture);
            Handles.EndGUI();
            sceneView.Repaint();
        }
    }
}
#endif
