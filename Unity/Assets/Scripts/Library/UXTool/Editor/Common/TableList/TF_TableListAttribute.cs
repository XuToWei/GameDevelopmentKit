#if UNITY_EDITOR && ODIN_INSPECTOR
namespace TF_TableList
{
    using System;
    using UnityEngine;

    public enum IndexRowStyle
    {
        Normal = 0,
        Checkbox = 1,
    }

    /// <summary>
    /// Renders lists and arrays in the inspector as tables.
    /// </summary>
    /// <seealso cref="TableColumnWidthAttribute"/>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class TF_TableListAttribute : Attribute
    {
        public TF_TableListAttribute()
        {

        }

        public TF_TableListAttribute(bool isReadOnly)
        {
            IsReadOnly = isReadOnly;
        }

        public bool InlineFilter = false;
        /// <summary>
        /// 是否允许多选行
        /// </summary>
        public bool MultiRowSelect = true;
        /// <summary>
        /// 是否显示Confluence帮助图标
        /// </summary>
        public string ConfluenceURL = "";
        /// <summary>
        /// 是否常驻显示表头按钮, 不常驻显示时，鼠标移动到表头才会显示按钮
        /// </summary>
        public bool AlwaysShowColumnHeaderButton = false;
        /// <summary>
        /// index 列显示数字，或者checkbox
        /// </summary>
        public IndexRowStyle IndexRowStyle = IndexRowStyle.Normal;
        /// <summary>
        /// If ShowPaging is enabled, this will override the default setting specified in the Odin Preferences window.
        /// </summary>
        public int NumberOfItemsPerPage;

        /// <summary>
        /// Mark the table as read-only. This removes all editing capabilities from the list such as Add and delete,
        /// but without disabling GUI for each element drawn as otherwise would be the case if the <see cref="ReadOnlyAttribute"/> was used.
        /// </summary>
        public bool IsReadOnly = false;

        /// <summary>
        /// The default minimum column width - 40 by default. This can be overwriten by individual columns using the <see cref="TableColumnWidthAttribute"/>.
        /// </summary>
        public int DefaultMinColumnWidth = 40;

        /// <summary>
        /// If true, a label is drawn for each element which shows the index of the element.
        /// </summary>
        public bool ShowIndexLabels = true;

        /// <summary>
        /// Whether to draw all rows in a scroll-view.
        /// </summary>
        public bool DrawScrollView = true;

        /// <summary>
        /// The number of pixels before a scroll view appears. 350 by default.
        /// </summary>
        public int MinScrollViewHeight = 350;

        /// <summary>
        /// The number of pixels before a scroll view appears. 0 by default.
        /// </summary>
        public int MaxScrollViewHeight;

        /// <summary>
        /// If true, expanding and collapsing the table from the table title-bar is no longer an option.
        /// </summary>
        public bool AlwaysExpanded = true;

        /// <summary>
        /// Whether to hide the toolbar containing the add button and pagin etc.s
        /// </summary>
        public bool HideToolbar = false;

        /// <summary>
        /// The cell padding.
        /// </summary>
        public int CellPadding = 2;

        [SerializeField, HideInInspector]
        private bool showPagingHasValue = false;

        [SerializeField, HideInInspector]
        private bool showPaging = false;

        /// <summary>
        /// Whether paging buttons should be added to the title bar. The default value of this, can be customized from the Odin Preferences window.
        /// </summary>
        public bool ShowPaging
        {
            get { return this.showPaging; }
            set
            {
                this.showPaging = value;
                this.showPagingHasValue = true;
            }
        }

        /// <summary>
        /// Whether the ShowPaging property has been set.
        /// </summary>
        public bool ShowPagingHasValue { get { return this.showPagingHasValue; } }

        /// <summary>
        /// Sets the Min and Max ScrollViewHeight.
        /// </summary>
        public int ScrollViewHeight
        {
            get { return Math.Min(this.MinScrollViewHeight, this.MaxScrollViewHeight); }
            set { this.MinScrollViewHeight = this.MaxScrollViewHeight = value; }
        }
    }

    public class TF_TableListStringTooltip : Attribute
    {

    }

    public class TF_TableListFilterAttribute : Attribute
    {
        public Type FilterType;
        public bool Deletable;

        public TF_TableListFilterAttribute(Type filterType, bool deletable = true)
        {
            FilterType = filterType;
            Deletable = deletable;
        }
    }

    public class TF_TableListColumnNameAttribute : Attribute
    {
        public string Name;

        public TF_TableListColumnNameAttribute(string name)
        {
            Name = name;
        }
    }

    [Flags]
    public enum TableActionPlace
    {
        IndexRMenu,
        Toolbar,
        ColumnRMenu,
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class TF_TableListActionAttribute : Attribute
    {
        public string Name;
        public string MemberName;
        public TableActionPlace Place;

        public TF_TableListActionAttribute(string name, string memberName, TableActionPlace place = TableActionPlace.Toolbar | TableActionPlace.IndexRMenu)
        {
            this.Name = name;
            this.MemberName = memberName;
            this.Place = place;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class TF_TableListOnRowSelectAttribute : Attribute
    {
        public string MemberName;
        public bool Delayed = true;

        public TF_TableListOnRowSelectAttribute(string memberName, bool delayed = true)
        {
            this.MemberName = memberName;
            this.Delayed = delayed;
        }
    }



    public class EnableRowSelectionAttribute : Attribute
    {
    }

    public class TableValidatorAttribute : Attribute
    {
        public Type ValidatorType;
        public TableValidatorAttribute(Type type)
        {
            ValidatorType = type;
        }
    }

    //用于限制快捷筛选时显示的最大数目，负数表示不限制
    public class TF_TableListQuickFilterLimit : Attribute
    {
        public int Limit;
        public TF_TableListQuickFilterLimit(int limit)
        {
            Limit = limit;
        }
    }

    public sealed class PrimaryKeyTypeAttribute : Attribute
    {
        public string TypeName;
        public string RetTypeName;

        public PrimaryKeyTypeAttribute(string typeName, string retTypeName)
        {
            TypeName = typeName;
            RetTypeName = retTypeName;
        }
    }
}
#endif