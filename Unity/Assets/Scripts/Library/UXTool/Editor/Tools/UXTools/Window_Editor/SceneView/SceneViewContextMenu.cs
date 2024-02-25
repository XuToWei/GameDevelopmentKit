#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

namespace ThunderFireUITool
{

    public static class SceneViewContextMenu
    {
        static bool rightMouseDown;//右键按下事件

        static bool hasDrag; //本次点击过程中是否有拖动

        public delegate void AddContextMenuFunc();
        public static AddContextMenuFunc addContextMenuFunc;

        [InitializeOnLoadMethod]
        static void Init()
        {
            hasDrag = false;
            rightMouseDown = false;

            SceneView.duringSceneGui += OnSceneGUI;
        }

        static void OnSceneGUI(SceneView sceneView)
        {
            //只处理右键点击事件
            if (Event.current == null || Event.current.button != 1)
            {
                return;
            }

            //右键点下时初始化点击状态
            if (Event.current.type == EventType.MouseDown)
            {
                hasDrag = false;
                rightMouseDown = true;
                return;
            }

            //拖动时修改鼠标事件状态
            if (Event.current.type == EventType.MouseDrag && rightMouseDown)
            {
                hasDrag = true;
                return;
            }

            //如果点击在辅助线上就不处理
            if (LocationLineLogic.HasInstance && LocationLineLogic.Instance.CheckPosInLines(Event.current.mousePosition, out LocationLine line))
            {
                return;
            }


            //处理右键点击松开时弹出菜单
            if (Event.current.type == EventType.MouseUp && rightMouseDown && !hasDrag)
            {
                //右键快速选择UI列表
                GenRightClickSelectObjectMenuList();

                //右键快速操作RectTransform列表
                GenRightClickRectTransformOperationList();

                //其他系统的右键菜单委托
                addContextMenuFunc?.Invoke();

                if (!ContextMenu.IsEmpty())
                {
                    Vector2 mousePos = Event.current.mousePosition;
#if UNITY_2022_1_OR_NEWER
                    EditorApplication.delayCall += () =>
                        {
                            ContextMenu.Show(mousePos);
                        };
#else
                    ContextMenu.Show();
                    Event.current.Use();
#endif
                }

                hasDrag = false;
                rightMouseDown = false;
            }
        }

        #region Selected Transform Operation

        private static void GenRightClickRectTransformOperationList()
        {
            if (Selection.gameObjects != null && Selection.gameObjects.Length != 0 && Selection.gameObjects[0].transform is RectTransform)
            {
                ContextMenu.AddSeparator("");

                ContextMenu.AddCommonItems(Selection.gameObjects);

                ContextMenu.AddSeparator("");

                if (CombineWidgetLogic.CanCombine(Selection.gameObjects))
                {
                    ContextMenu.AddItem(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_组合), false, () =>
                    {
                        CombineWidgetLogic.GenCombineRootRect(Selection.gameObjects);
                    });
                }
                else
                {
                    ContextMenu.AddDisabledItem(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_组合));
                }
            }
        }

        #endregion

        #region Select UI List
        private class DuplicateKeyComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return -string.Compare(x, y);
            }

        }
        private static void GenRightClickSelectObjectMenuList()
        {
            if (SwitchSetting.CheckValid(SwitchSetting.SwitchType.RightClickList))
            {
                var prefabStage = PrefabStageUtils.GetCurrentPrefabStage();

                //获取当前Hierarchy中所有UI节点
                List<RectTransform> inSceneObjs = new List<RectTransform>();
                if (prefabStage != null)
                {

                    RectTransform[] allObjects = prefabStage.prefabContentsRoot.GetComponentsInChildren<RectTransform>();
                    foreach (RectTransform obj in allObjects)
                    {
                        if (EditorLogic.ObjectFit(obj.gameObject))
                        {
                            inSceneObjs.Add(obj);
                        }
                    }
                }
                else
                {
                    Scene scene = SceneManager.GetActiveScene();
                    GameObject[] allObjects = scene.GetRootGameObjects();
                    foreach (GameObject obj in allObjects)
                    {
                        RectTransform[] child = obj.GetComponentsInChildren<RectTransform>();
                        foreach (RectTransform rect in child)
                        {
                            if (EditorLogic.ObjectFit(rect.gameObject))
                            {
                                inSceneObjs.Add(rect);
                            }
                        }
                    }
                }

                //计算鼠标点击实际位置
                Camera camera = SceneView.currentDrawingSceneView.camera; //获取到编辑器模式下的相机，这个相机是看不到的，但是可以拿到
                Vector3 pos = Event.current.mousePosition; //低版本可能要×2
                pos = new Vector3(pos.x, camera.pixelHeight - pos.y);

                //UI节点排序
                List<string> liststr = new List<string>();
                foreach (var rect in inSceneObjs)
                {
                    string str = GetString("", rect);
                    liststr.Add(str);
                }
                SortedList<string, RectTransform> stlist = new SortedList<string, RectTransform>(new DuplicateKeyComparer());
                for (int i = 0; i < inSceneObjs.Count; i++)
                {
                    stlist.Add(liststr[i], inSceneObjs[i]);
                }

                //把UI节点名称添加到右键菜单中
                foreach (string d in stlist.Keys)
                {
                    RectTransform obj = stlist[d];
                    if (RectTransformUtility.RectangleContainsScreenPoint(obj, pos, camera))
                    {
                        ContextMenu.AddItem(obj.name, false, () =>
                        {
                            Selection.activeGameObject = obj.gameObject;
                        });
                    }
                }
            }
        }
        private static string GetString(string oldstr, Transform trans)
        {
            string str;
            if (oldstr == "") str = trans.GetSiblingIndex().ToString();
            else str = trans.GetSiblingIndex().ToString() + "." + oldstr;
            if (trans.parent != null)
            {
                return GetString(str, trans.parent);
            }
            return str;
        }
        #endregion
    }
}
#endif