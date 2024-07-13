#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ThunderFireUITool
{
    public class UIComponentCheckResult
    {
        public GameObject prefabGo;
        public string prefabPath;
        public List<string> nodePaths;
        public List<long> nodeFileIds;
    }

    //带数据的TreeViewItem
    public class UIComponentCheckResultViewItem : TreeViewItem
    {
        public UIComponentCheckResult data;
    }

    //结果展示
    public class UIComponentCheckResultTableView : TreeView
    {
        //图标宽度
        const float kIconWidth = 18f;
        //列表高度
        const float kRowHeights = 20f;
        public UIComponentCheckResultViewItem root;

        private GUIStyle cellStyle;

        //列信息
        enum ResultColumns
        {
            Name,
            Path,
            Nodes
        }

        public UIComponentCheckResultTableView(TreeViewState state, MultiColumnHeader multicolumnHeader) : base(state, multicolumnHeader)
        {
            rowHeight = kRowHeights;
            columnIndexForTreeFoldouts = 0;
            showAlternatingRowBackgrounds = true;
            showBorder = true;
            customFoldoutYOffset = (kRowHeights - EditorGUIUtility.singleLineHeight) * 0.5f; // center foldout in the row since we also center content. See RowGUI
            extraSpaceBeforeIconAndLabel = kIconWidth;

            cellStyle = new GUIStyle();
            cellStyle.alignment = TextAnchor.UpperCenter;
            cellStyle.normal.textColor = Color.white;

            root = new UIComponentCheckResultViewItem { id = 0, depth = -1, displayName = "Result" };
            //初始化的时候需要在根节点下插入一个子节点，不然会报Children null的错误
            root.AddChild(new UIComponentCheckResultViewItem { id = 1, depth = 0, displayName = "temp" });
        }

        public static long GetLocalIdentfierInFile(UnityEngine.Object obj)
        {
            PropertyInfo info = typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);
            SerializedObject sObj = new SerializedObject(obj);
            info.SetValue(sObj, InspectorMode.Debug, null);
            SerializedProperty localIdProp = sObj.FindProperty("m_LocalIdentfierInFile");
            return localIdProp.longValue;
        }

        //生成ColumnHeader
        public static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState(float treeViewWidth)
        {
            var columns = new List<MultiColumnHeaderState.Column>
            {
			    //图标+名称
			    new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent(UICommonScriptCheckWindow.CheckResult_NameString),
                    headerTextAlignment = TextAlignment.Center,
                    sortedAscending = false,
                    width = 200,
                    minWidth = 60,
                    autoResize = false,
                    allowToggleVisibility = false,
                    canSort = true,
                    sortingArrowAlignment = TextAlignment.Right
                },
			    //路径
			    new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent(UICommonScriptCheckWindow.CheckResult_PathString),
                    headerTextAlignment = TextAlignment.Center,
                    sortedAscending = false,
                    width = 660,
                    minWidth = 60,
                    autoResize = false,
                    allowToggleVisibility = false,
                    canSort = true,
                    sortingArrowAlignment = TextAlignment.Right
                },
			    //符合条件的节点
			    new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent(UICommonScriptCheckWindow.CheckResult_ObjectListString),
                    headerTextAlignment = TextAlignment.Center,
                    sortedAscending = false,
                    width = 60,
                    minWidth = 60,
                    autoResize = false,
                    allowToggleVisibility = true,
                    canSort = false
                }
            };
            var state = new MultiColumnHeaderState(columns.ToArray());
            return state;
        }
        //生成Tree
        public void UpdateTree(List<UIComponentCheckResult> checkResults)
        {
            root.children.Clear();
            int elementCount = 0;
            foreach (var result in checkResults)
            {
                elementCount++;
                UIComponentCheckResultViewItem rs = new UIComponentCheckResultViewItem()
                {
                    id = elementCount,
                    depth = 0,
                    data = result
                };
                root.AddChild(rs);
            }
            Reload();
        }


        //响应双击事件
        protected override void DoubleClickedItem(int id)
        {
            var item = (UIComponentCheckResultViewItem)FindItem(id, rootItem);

            if (item != null)
            {
                var assetObject = AssetDatabase.LoadAssetAtPath(item.data.prefabPath, typeof(UnityEngine.Object));
                if (assetObject is GameObject)
                {
                    //是Prefab 打开prefab 并在Hierarchy中高亮引用资源的节点
                    AssetDatabase.OpenAsset(assetObject);
                    if (PrefabStageUtils.GetCurrentPrefabStage() != null)
                    {
                        var go = PrefabStageUtils.GetCurrentPrefabStage().prefabContentsRoot;
                        var transList = go.GetComponentsInChildren<Transform>().ToList();

                        UICommonScriptCheckWindow.checkResultGoTransList.Clear();
                        foreach (var trans in transList)
                        {
                            var fileId = GetLocalIdentfierInFile(trans.gameObject);

                            if (item.data.nodeFileIds.Contains(fileId))
                            {
                                UICommonScriptCheckWindow.checkResultGoTransList.Add(trans);
                            }
                        }
                    }
                }
                else
                {
                    //不是Prefab 在ProjectWindow中高亮双击资源
                    EditorUtility.FocusProjectWindow();
                    Selection.activeObject = assetObject;
                    EditorGUIUtility.PingObject(assetObject);
                }
            }
        }
        protected override void ExpandedStateChanged()
        {
        }
        protected override TreeViewItem BuildRoot()
        {
            SetupDepthsFromParentsAndChildren(root);
            return root;
        }

        protected override float GetCustomRowHeight(int row, TreeViewItem item)
        {
            UIComponentCheckResultViewItem e = item as UIComponentCheckResultViewItem;
            if (e == null || e.data == null) return base.GetCustomRowHeight(row, item);

            if (e.data.nodePaths.Count > 0)
            {
                return EditorGUIUtility.singleLineHeight * e.data.nodePaths.Count + 5;
            }

            return base.GetCustomRowHeight(row, item);
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            var item = (UIComponentCheckResultViewItem)args.item;
            if (item == null || item.data == null) return;

            for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
            {
                var rect = args.GetCellRect(i);
                CellGUI(rect, item, (ResultColumns)args.GetColumn(i), ref args);
            }
        }


        //绘制列表中的每项内容
        void CellGUI(Rect cellRect, UIComponentCheckResultViewItem item, ResultColumns column, ref RowGUIArgs args)
        {
            switch (column)
            {
                case ResultColumns.Name:
                    {
                        //CenterRectUsingSingleLineHeight(ref cellRect);
                        cellRect.height = EditorGUIUtility.singleLineHeight;
                        EditorGUI.ObjectField(cellRect, item.data.prefabGo, typeof(GameObject), false);
                    }
                    break;
                case ResultColumns.Path:
                    {
                        GUI.Label(cellRect, item.data.prefabPath, cellStyle);
                    }
                    break;
                case ResultColumns.Nodes:
                    {
                        EditorGUILayout.BeginVertical();
                        for (int i = 0; i < item.data.nodePaths.Count; i++)
                        {
                            var rect = new Rect()
                            {
                                x = cellRect.x,
                                y = cellRect.y + EditorGUIUtility.singleLineHeight * i,
                                width = cellRect.width,
                                height = EditorGUIUtility.singleLineHeight
                            };
                            GUI.Label(rect, item.data.nodePaths[i], cellStyle);
                        }
                        EditorGUILayout.EndVertical();
                    }
                    break;
            }
        }

        //根据资源信息获取资源图标
        private Texture2D GetIcon(string path)
        {
            Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
            if (obj != null)
            {
                Texture2D icon = AssetPreview.GetMiniThumbnail(obj);
                if (icon == null)
                    icon = AssetPreview.GetMiniTypeThumbnail(obj.GetType());
                return icon;
            }
            return null;
        }
    }
}
#endif