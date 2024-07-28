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

    public struct ReferenceGoInfo
    {
        public string goName; //所在节点的名字
        public string scriptName; //所在脚本的名字
        public string goFileId; //所在节点的FileId
    }

    //资源引用树
    public class AssetTreeView : EditorUIUtils.UIIMTreeView<AssetDescription>
    {
        private readonly GUIStyle _stateGUIStyle = new GUIStyle { richText = true, alignment = TextAnchor.MiddleCenter };
        //public bool IsDepend = false;

        //列信息
        enum MyColumns
        {
            Name,
            Path,
            State,
            RefCount
        }
        
        public AssetTreeView() : base()
        {
            // IsDepend = viewStruct.IsDepend;
            /*if (args != null)
            {
                IsDepend = (bool)args[0];
            }*/
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
            var item = (AssetDescription)FindItem(id, rootItem);

            if (item != null)
            {
                var assetObject = AssetDatabase.LoadAssetAtPath(item.path, typeof(UnityEngine.Object));
                if (assetObject is GameObject)
                {
                    //是Prefab 打开prefab 并高亮引用资源的节点
                    AssetDatabase.OpenAsset(assetObject);
                    if (item.parent is AssetDescription assetViewItem && assetViewItem != null)
                    {
                        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetViewItem.path);
                        if (sprite == null) return;
                        var go = PrefabStageUtils.GetCurrentPrefabStage().prefabContentsRoot;

                        string guid = AssetDatabase.AssetPathToGUID(assetViewItem.path);
                        string prefabPath = item.path;

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

                            Debug.Log($"共计 <color=red>[{referenceInfos.Count}]</color> 个节点引用 <color=red>[{assetViewItem.name}]</color> ");

                            ReferenceInfo.referenceGoTransList.Clear();


                            ReferenceInfo.referenceGoTransList.AddRange(go.GetComponentsInChildren<Transform>(true)
                                .Where(trans => goFileIds.Contains(GetLocalIdentfierInFile(trans.gameObject).ToString())));

                            if (ReferenceInfo.referenceGoTransList.Count > 0)
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
            foreach (AssetDescription item in GetRows())
            {
                item.count = ReferenceInfo.data.GetRefCount(item, item.parent as AssetDescription);
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
                var item = (AssetDescription)FindItem(i, rootItem);
                foreach (AssetDescription child in item.children)
                {
                    child.count = ReferenceInfo.data.GetRefCount(child, (child.parent as AssetDescription));
                }

                SortHelper.SortChild(item);
            }

            ReferenceInfo curWindow = EditorWindow.GetWindow<ReferenceInfo>();
            curWindow.needUpdateAssetTree = true;
        }

        protected override void CellGUI(Rect rect, AssetDescription item, int columnIndex, RowGUIArgs args)
        {
            CenterRectUsingSingleLineHeight(ref rect);
            switch ((MyColumns)args.GetColumn(columnIndex))
            {
                case MyColumns.Name:
                {
                    var iconRect = rect;
                    iconRect.x += GetContentIndent(item);
                    iconRect.width = KIconWidth;
                    if (iconRect.x < rect.xMax)
                    {
                        var icon = GetIcon(item.path);
                        if (icon != null)
                            GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit);
                    }

                    var nameRect = rect;
                    nameRect.x += GetContentIndent(item) + KIconWidth;
                    GUI.Label(nameRect, item.name);
                }
                    break;
                case MyColumns.Path:
                {
                    GUI.Label(rect, item.path);
                }
                    break;
                case MyColumns.State:
                {
                    GUI.Label(rect, ReferenceFinderData.GetInfoByState(item.state), _stateGUIStyle);
                }
                    break;
                case MyColumns.RefCount:
                {
                    item.count = ReferenceInfo.data.GetRefCount(item, (item.parent as AssetDescription));
                    GUI.Label(rect, item.count.ToString(), _stateGUIStyle);
                }
                    break;
            }
        }

        public override List<MultiColumnHeaderState.Column> CreateDefaultMultiColumns()
        {
            var columns = new List<MultiColumnHeaderState.Column>
            {
                //图标+名称
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("名字"),
                    width = 200,
                    minWidth = 60,
                },
                //路径
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("路径"),
                    width = 360,
                    minWidth = 60,
                },
                //状态
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("状态"),
                    width = 60,
                    minWidth = 60,
                    canSort = false
                },
            };
            // if (!IsDepend)
            // {
            columns.Add(
                //引用数
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("引用数"),
                    width = 70,
                    minWidth = 70,
                });
            // }

            return columns;
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