#if UNITY_2022_3_OR_NEWER
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    public class UXBuilderMultiColumnTreeViewStruct
    {
        public const string ClassName = "UXMultiColumnTreeView";
        public readonly UXStyle Style = new UXStyle();
        public readonly Columns Columns = new Columns();
        public const bool SortingEnabled = false;
        public readonly Action SortAction = null;
    }

    public class UXBuilderMultiColumnTreeView : MultiColumnTreeView
    {
        private static UXBuilderMultiColumnTreeView _mUXMultiColumnTreeView;
        private static UXStyle _mStyle = new UXStyle();

        private static UXBuilderMultiColumnTreeViewStruct _mComponent = new UXBuilderMultiColumnTreeViewStruct();

        public UXBuilderMultiColumnTreeViewStruct GetComponents()
        {
            return _mComponent;
        }

        public UXBuilderMultiColumnTreeView SetComponents<T>(UXBuilderMultiColumnTreeViewStruct component, IList<TreeViewItemData<T>> itemDatas)
        {
            VisualElement parent = this.hierarchy.parent;
            if (parent == null) return this;
            _mComponent = component;
            var a = Create(parent, component, itemDatas);
            a.PlaceBehind(this);
            parent.Remove(this);
            return a;
        }

        public static UXBuilderMultiColumnTreeView Create<T>(VisualElement root, UXBuilderMultiColumnTreeViewStruct component, IList<TreeViewItemData<T>> itemDatas)
        {
            _mComponent = component;

            _mUXMultiColumnTreeView = new UXBuilderMultiColumnTreeView();

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ThunderFireUIToolConfig.UIBuilderPath + "USS/UXElements/UXBuilderMultiColumnTreeView.uss");
            _mUXMultiColumnTreeView.styleSheets.Add(styleSheet);
            _mUXMultiColumnTreeView.AddToClassList("ux-multi-column-treeview");
            _mUXMultiColumnTreeView.name = UXBuilderMultiColumnTreeViewStruct.ClassName;
            _mUXMultiColumnTreeView.showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly;
            _mUXMultiColumnTreeView.SetRootItems(itemDatas);
            _mUXMultiColumnTreeView.sortingEnabled = true;
            StyleCopy.IStyleToUXStyle(_mStyle, _mUXMultiColumnTreeView.style);
            InitComponent(component);
            
            root.Add(_mUXMultiColumnTreeView);

            return _mUXMultiColumnTreeView;
        }

        private static void InitComponent(UXBuilderMultiColumnTreeViewStruct component)
        {
            InitColumns(component.Columns);
            InitSort(UXBuilderMultiColumnTreeViewStruct.SortingEnabled, component.SortAction);
            InitStyle(component.Style);
        }

        private static void InitStyle(UXStyle style)
        {
            if (style == _mStyle) return;
            UXStyle compStyle = new UXStyle();
            Type type = typeof(UXStyle);
            PropertyInfo[] properties = type.GetProperties();
            foreach (var property in properties)
            {
                var value = property.GetValue(style);
                if (!value.Equals(property.GetValue(compStyle)))
                {
                    property.SetValue(_mStyle, property.GetValue(style));
                }
            }
            StyleCopy.UXStyleToIStyle(_mUXMultiColumnTreeView.style, _mStyle);
        }

        private static void InitColumns(Columns componentColumns)
        { 
            Column[] columns = new Column[componentColumns.Count];
            componentColumns.CopyTo(columns, 0);
            
            foreach (var column in columns)
            {
                _mUXMultiColumnTreeView.columns.Add(column);
            }
            _mUXMultiColumnTreeView.Query<VisualElement>("unity-multi-column-header-column-title")
                .ForEach(element => element.AddToClassList("ux-multi-column-treeview-header-label__custom"));
        }

        private static void InitSort(bool sortingEnabled, Action sortAction)
        {
            _mUXMultiColumnTreeView.sortingEnabled = sortingEnabled;
            _mUXMultiColumnTreeView.columnSortingChanged += sortAction;
        }
    }
}
#endif