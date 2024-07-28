#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ThunderFireUITool
{
    public static partial class EditorUIUtils
    {
        public abstract class UIIMTreeView<T> : TreeView where T : TreeViewItem
        {
            //图标宽度
            protected const float KIconWidth = 18f;
            //列表高度
            protected const float KRowHeights = 20f;

            private TreeViewItem _assetRoot;
            public TreeViewItem AssetRoot
            {
                get => _assetRoot;
                set
                {
                    _assetRoot = value;
                    Reload();
                }
            }

            protected UIIMTreeView(TreeViewState state = null) : base(state ?? new TreeViewState(), new MultiColumnHeader(new MultiColumnHeaderState(new[] { new MultiColumnHeaderState.Column() })))
            {
                rowHeight = KRowHeights;
                columnIndexForTreeFoldouts = 0;
                showAlternatingRowBackgrounds = true;
                showBorder = true;
                customFoldoutYOffset = (KRowHeights - EditorGUIUtility.singleLineHeight) * 0.5f; // center foldout in the row since we also center content. See RowGUI
                extraSpaceBeforeIconAndLabel = KIconWidth;

                AssetRoot = new TreeViewItem
                {
                    id = 0,
                    depth = -1,
                    displayName = "Result",
                    children = new List<TreeViewItem>()
                };
            }

            public static MultiColumnHeader ApplyColumnStyle(List<MultiColumnHeaderState.Column> customColumns)
            {
                var columns = new List<MultiColumnHeaderState.Column>();
                // 处理列样式，样式覆盖策略：如果用户自定义样式与原始样式不同，才会进行覆盖，否则采用默认样式
                foreach (var customColumn in customColumns)
                {
                    // 原始样式
                    var originColumn = new MultiColumnHeaderState.Column();
                    // 默认样式
                    var column = new MultiColumnHeaderState.Column()
                    {
                        headerTextAlignment = TextAlignment.Center,
                        sortedAscending = false,
                        allowToggleVisibility = false,
                        canSort = true,
                        sortingArrowAlignment = TextAlignment.Right,
                        autoResize = false,
                    };
                    var fields = column.GetType().GetFields();
                    foreach (var field in fields)
                    {
                        var value = field.GetValue(customColumn);
                        if (value != null && !value.Equals(field.GetValue(originColumn)))
                        {
                            field.SetValue(column, field.GetValue(customColumn));
                        }
                    }

                    columns.Add(column);
                }

                return new MultiColumnHeader(new MultiColumnHeaderState(columns.ToArray()));
            }

            protected override TreeViewItem BuildRoot()
            {
                SetupDepthsFromParentsAndChildren(AssetRoot);
                return AssetRoot;
            }

            protected override void RowGUI(RowGUIArgs args)
            {
                var item = args.item;
                if (!(item is T)) return;
                for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
                {
                    var rect = args.GetCellRect(i);
                    CellGUI(rect, (T)item, i, args);
                }
            }

            public void ChangeHeader(List<MultiColumnHeaderState.Column> header, T assetRoot)
            {
                multiColumnHeader = new MultiColumnHeader(new MultiColumnHeaderState(header.ToArray()));
                AssetRoot = assetRoot;
            }

            protected abstract void CellGUI(Rect rect, T item, int columnIndex, RowGUIArgs args);
            public abstract List<MultiColumnHeaderState.Column> CreateDefaultMultiColumns();
        }

        public static T1 CreateTreeView<T1, T2>(params object[] args) where T1 : UIIMTreeView<T2> where T2 : TreeViewItem, new()
        {
            var treeView = Activator.CreateInstance(typeof(T1), args) as T1;
            if (treeView == null) return null;
            treeView.multiColumnHeader = UIIMTreeView<T2>.ApplyColumnStyle(treeView.CreateDefaultMultiColumns());
            treeView.Reload();
            return treeView;
        }
    }
}
#endif