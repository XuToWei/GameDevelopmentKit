#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;
using System.Linq;

using System.Reflection;

namespace ThunderFireUITool
{
    //带数据的TreeViewItem
    public class AssetViewItem : TreeViewItem
    {
        public ReferenceFinderData.AssetDescription data;
    }

    public struct ReferenceGoInfo
    {
        public string goName;   //所在节点的名字
        public string scriptName;   //所在脚本的名字
        public string goFileId;   //所在节点的FileId
    }

    //资源引用树
    public class AssetTreeView : TreeView
    {
        //图标宽度
        const float kIconWidth = 18f;
        //列表高度
        const float kRowHeights = 20f;
        public AssetViewItem assetRoot;

        private GUIStyle stateGUIStyle = new GUIStyle { richText = true, alignment = TextAnchor.MiddleCenter };
        private string ignorePath = "Canvas (Environment)/";

        //列信息
        enum MyColumns
        {
            Name,
            Path,
            State,
            RefCount
        }

        public AssetTreeView(TreeViewState state, ClickColumn multicolumnHeader) : base(state, multicolumnHeader)
        {
            rowHeight = kRowHeights;
            columnIndexForTreeFoldouts = 0;
            showAlternatingRowBackgrounds = true;
            showBorder = false;
            customFoldoutYOffset = (kRowHeights - EditorGUIUtility.singleLineHeight) * 0.5f; // center foldout in the row since we also center content. See RowGUI
            extraSpaceBeforeIconAndLabel = kIconWidth;
        }
        private static long GetLocalIdentfierInFile(UnityEngine.Object obj)
        {
            PropertyInfo info = typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);
            SerializedObject sObj = new SerializedObject(obj);
            info.SetValue(sObj, InspectorMode.Debug, null);
            SerializedProperty localIdProp = sObj.FindProperty("m_LocalIdentfierInFile");
            return localIdProp.longValue;
        }

        //响应双击事件
        protected override void DoubleClickedItem(int id)
        {
            var item = (AssetViewItem)FindItem(id, rootItem);
            
            if (item != null)
            {
                var assetObject = AssetDatabase.LoadAssetAtPath(item.data.path, typeof(UnityEngine.Object));
                if (assetObject is GameObject)
                {
                    //是Prefab 打开prefab 并高亮引用资源的节点
                    AssetDatabase.OpenAsset(assetObject);
                    if (item.parent is AssetViewItem assetViewItem && assetViewItem.data != null)
                    {
                        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetViewItem.data.path);
                        if (sprite == null) return;
                        var go = PrefabStageUtils.GetCurrentPrefabStage().prefabContentsRoot;

                        string guid = AssetDatabase.AssetPathToGUID(assetViewItem.data.path);
                        string prefabPath = item.data.path;

                        List<ReferenceGoInfo> referenceInfos = new List<ReferenceGoInfo>();
                        PrefabYamlUtils.FindGuidReference(prefabPath, guid, ref referenceInfos);
                        if (referenceInfos.Count > 0)
                        {
                            List<string> goFileIds = new List<string>();
                            foreach (ReferenceGoInfo info in referenceInfos)
                            {
                                Debug.Log($"引用资源的节点: <color=red>[{info.goName}]</color>, 引用脚本: <color=red>[{info.scriptName}]</color> ");
                                goFileIds.Add(info.goFileId);
                            }
                            Debug.Log($"共计 <color=red>[{referenceInfos.Count}]</color> 个节点引用 <color=red>[{assetViewItem.data.name}]</color> ");

                            ReferenceInfo.referenceGoTransList.Clear();
                            

                            ReferenceInfo.referenceGoTransList.AddRange(go.GetComponentsInChildren<Transform>(true).Where(trans => goFileIds.Contains(GetLocalIdentfierInFile(trans.gameObject).ToString())));

                            if(ReferenceInfo.referenceGoTransList.Count > 0)
                            {
                                EditorGUIUtility.PingObject(ReferenceInfo.referenceGoTransList[0].gameObject);
                                Selection.activeGameObject = ReferenceInfo.referenceGoTransList[0].gameObject;
                                Selection.objects = ReferenceInfo.referenceGoTransList.Select(trans => trans.gameObject).ToArray();
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
            SortExpandItem();
        }

        public void GetRefCount()
        {
            foreach(AssetViewItem item in GetRows())
            {
                item.data.count = ReferenceInfo.data.GetRefCount(item.data, (item.parent as AssetViewItem)?.data);
            }      
        }
        public void SortExpandItem()
        {
            if (SortHelper.curSortType == SortType.None)
            {
                return;
            }
            var expandItemList = GetExpanded();
            foreach (var i in expandItemList)
            {
                var item = (AssetViewItem)FindItem(i, rootItem);
                foreach(AssetViewItem child in item.children)
                {
                    child.data.count = ReferenceInfo.data.GetRefCount(child.data, (child.parent as AssetViewItem)?.data);
                }
                SortHelper.SortChild(item.data);
            }
            ReferenceInfo curWindow = EditorWindow.GetWindow<ReferenceInfo>();
            curWindow.needUpdateAssetTree = true;
        }

        //生成ColumnHeader
        public static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState(float treeViewWidth, bool isDepend)
        {
            var columns = new List<MultiColumnHeaderState.Column>
            {
			//图标+名称
			new MultiColumnHeaderState.Column
            {
                headerContent = new GUIContent("名字"),
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
                headerContent = new GUIContent("路径"),
                headerTextAlignment = TextAlignment.Center,
                sortedAscending = false,
                width = 360,
                minWidth = 60,
                autoResize = false,
                allowToggleVisibility = false,
                canSort = true,
                sortingArrowAlignment = TextAlignment.Right
            },
			//状态
			new MultiColumnHeaderState.Column
            {
                headerContent = new GUIContent("状态"),
                headerTextAlignment = TextAlignment.Center,
                sortedAscending = false,
                width = 60,
                minWidth = 60,
                autoResize = false,
                allowToggleVisibility = true,
                canSort = false
            },
        };
            if (!isDepend)
            {
                columns.Add(
                   //引用数
                   new MultiColumnHeaderState.Column
                   {
                       headerContent = new GUIContent("引用数"),
                       headerTextAlignment = TextAlignment.Center,
                       sortedAscending = false,
                       width = 70,
                       minWidth = 70,
                       autoResize = true,
                       allowToggleVisibility = true,
                       canSort = true,
                       sortingArrowAlignment = TextAlignment.Right
                   });
            }
            var state = new MultiColumnHeaderState(columns.ToArray());
            return state;
        }

        protected override TreeViewItem BuildRoot()
        {
            return assetRoot;
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            var item = (AssetViewItem)args.item;
            for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
            {
                CellGUI(args.GetCellRect(i), item, (MyColumns)args.GetColumn(i), ref args);
            }
        }

        //绘制列表中的每项内容
        void CellGUI(Rect cellRect, AssetViewItem item, MyColumns column, ref RowGUIArgs args)
        {
            CenterRectUsingSingleLineHeight(ref cellRect);
            switch (column)
            {
                case MyColumns.Name:
                    {
                        var iconRect = cellRect;
                        iconRect.x += GetContentIndent(item);
                        iconRect.width = kIconWidth;
                        if (iconRect.x < cellRect.xMax)
                        {
                            var icon = GetIcon(item.data.path);
                            if (icon != null)
                                GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit);
                        }
                        args.rowRect = cellRect;
                        base.RowGUI(args);
                    }
                    break;
                case MyColumns.Path:
                    {
                        GUI.Label(cellRect, item.data.path);
                    }
                    break;
                case MyColumns.State:
                    {
                        GUI.Label(cellRect, ReferenceFinderData.GetInfoByState(item.data.state), stateGUIStyle);
                    }
                    break;
                case MyColumns.RefCount:
                    {
                        //GUI.Label(cellRect, ReferenceInfo.data.GetRefCount(item.data, (item.parent as AssetViewItem)?.data).ToString(), stateGUIStyle);
                        item.data.count = ReferenceInfo.data.GetRefCount(item.data, (item.parent as AssetViewItem)?.data);
                        GUI.Label(cellRect, item.data.count.ToString(), stateGUIStyle);
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
