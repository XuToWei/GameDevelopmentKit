#if UNITY_EDITOR && ODIN_INSPECTOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.ActionResolvers;
using Sirenix.OdinInspector.Editor.Drawers;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TF_TableList
{
    public class ExtraTableInfo
    {
        public readonly List<RowExtraInfo> ReOrderedRows = new List<RowExtraInfo>();
        public readonly List<RowFilter> RowFilters = new List<RowFilter>();
        public int SelectionCount = 0;
        public int FilterSuccessCount = 0;

        public void Reset()
        {
            ReOrderedRows.Clear();
            SelectionCount = 0;
            FilterSuccessCount = 0;
        }

        public void Update()
        {
            SelectionCount = ReOrderedRows.Count(x => x.selected);
            FilterSuccessCount = ReOrderedRows.Count(x => x.filterSuccess);
        }
    }

    public class TF_TableListAttributeDrawer : OdinAttributeDrawer<TF_TableListAttribute>
    {
        private IOrderedCollectionResolver resolver;
        private LocalPersistentContext<bool> isVisible;
        private LocalPersistentContext<bool> isPagingExpanded;
        private LocalPersistentContext<Vector2> scrollPos;
        private LocalPersistentContext<int> currPage;
        private LocalPersistentContext<TablePersistentComponentsData> persistentComponentsData;
        private TF_TableRowLayoutGroup table;
        private HashSet<string> seenColumnNames;
        private List<Column> columns;
        private ObjectPicker picker;
        private int colOffset;
        private GUIContent indexLabel;
        private bool isReadOnly;
        private int indexLabelWidth;
        private Rect columnHeaderRect;
        private GUIPagingHelper paging;
        private bool drawAsList;
        private bool isFirstFrame = true;

        private StringFilterEdit _stringFilterEdit;
        private ScriptFilterEdit _scriptFilterEdit;
        private ScriptModifyEdit _scriptModifyEdit;
        TableFilterSet filterSet = new TableFilterSet();
        private SortMode lastSortMode = SortMode.NONE;
        private string lastSortCol;
        private ExtraTableInfo extraTableInfo = new ExtraTableInfo();
        private bool enableFilter = true;
        private bool sortOrFilterDirty = false;
        private readonly Dictionary<RowFilter, PropertyTree> CustomRowFilters = new Dictionary<RowFilter, PropertyTree>();
        private readonly Dictionary<string, RowFilter> InlineRowFilters = new Dictionary<string, RowFilter>();
        private readonly List<TableAction> TableActions = new List<TableAction>();
        private readonly List<TableRowSelectCallBack> RowSelectCallBacks = new List<TableRowSelectCallBack>();
        private readonly TableActionContext TableActionContext = new TableActionContext();
        private readonly TableRowSelectContext TableRowSelectContext = new TableRowSelectContext();
        private readonly Dictionary<string, bool> EnableRowSelectionColumnDict = new Dictionary<string, bool>();
        private readonly List<TableValidator> Validators = new List<TableValidator>();
        private readonly TableValidatorResult ValidatorResult = new TableValidatorResult();
        private readonly Dictionary<string, int> QuickFilterLimitDict = new Dictionary<string, int>();
        private bool ValidatorDirty = true;
        private bool enableMultiRowSelect;
        protected MethodInfo mClearAndDisposeChildrenFunc;
        /// <summary>
        /// Determines whether this instance [can draw attribute property] the specified property.
        /// </summary>
        protected override bool CanDrawAttributeProperty(InspectorProperty property)
        {
            return property.ChildResolver is IOrderedCollectionResolver;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Initialize()
        {
            this.drawAsList = false;
            this.isReadOnly = this.Attribute.IsReadOnly || !this.Property.ValueEntry.IsEditable;
            this.enableMultiRowSelect = this.Attribute.MultiRowSelect;
            this.indexLabelWidth = (int)SirenixGUIStyles.Label.CalcSize(new GUIContent("100")).x + 15;
            this.indexLabel = new GUIContent();
            this.colOffset = 0;
            this.seenColumnNames = new HashSet<string>();
            this.table = new TF_TableRowLayoutGroup();
            this.table.MinScrollViewHeight = this.Attribute.MinScrollViewHeight;
            this.table.MaxScrollViewHeight = this.Attribute.MaxScrollViewHeight;
            this.resolver = this.Property.ChildResolver as IOrderedCollectionResolver;
            this.isVisible = this.GetPersistentValue("toggle", GeneralDrawerConfig.Instance.ExpandFoldoutByDefault);
            this.scrollPos = this.GetPersistentValue("scrollPos", Vector2.zero);
            this.currPage = this.GetPersistentValue("currPage", 0);
            this.isPagingExpanded = this.GetPersistentValue("expanded", false);
            this.persistentComponentsData = this.GetPersistentValue("persistentData", default(TablePersistentComponentsData));
            this.columns = new List<Column>(10);
            this.paging = new GUIPagingHelper();
            this.paging.NumberOfItemsPerPage = this.Attribute.NumberOfItemsPerPage > 0 ? this.Attribute.NumberOfItemsPerPage : GeneralDrawerConfig.Instance.NumberOfItemsPrPage;
            this.paging.IsExpanded = this.isPagingExpanded.Value;
            this.paging.IsEnabled = GeneralDrawerConfig.Instance.ShowPagingInTables || this.Attribute.ShowPaging;
            this.paging.CurrentPage = this.currPage.Value;
            this.Property.ValueEntry.OnChildValueChanged += OnChildValueChanged;
            this.enableFilter = true;
            this.mClearAndDisposeChildrenFunc = this.Property.Children.GetType().GetMethod("ClearAndDisposeChildren",
                BindingFlags.Instance | BindingFlags.NonPublic);
            var resolverMaxCollectionLength = this.resolver?.MaxCollectionLength ?? 0;
            for (int i = 0; i < resolverMaxCollectionLength; i++)
            {
                extraTableInfo.ReOrderedRows.Add(new RowExtraInfo(i));
            }

            if (this.Attribute.AlwaysExpanded)
            {
                this.isVisible.Value = true;
            }

            var p = this.Attribute.CellPadding;
            if (p > 0)
            {
                this.table.CellStyle = new GUIStyle() { padding = new RectOffset(p, p, p, p) };
            }

            GUIHelper.RequestRepaint();

            if (this.Attribute.ShowIndexLabels)
            {
                this.colOffset++;
                this.columns.Add(new Column(this.indexLabelWidth, true, false, null, ColumnType.Index));
            }

            //			if (!this.isReadOnly) {
            //				this.columns.Add(new Column(22, true, false, null, ColumnType.DeleteButton));
            //			}

            PrepareActions();
            PrepareSelectionCallbacks();

            if (Property.Children.Count > 0)
                PrepareCustomAttributes();
        }

        private void UpdateTableActionContext()
        {
            TableActionContext.extraInfo = this.extraTableInfo;
            TableActionContext.targetTable = Property.ValueEntry.WeakSmartValue;
        }

        private void PrepareActions()
        {
            UpdateTableActionContext();
            foreach (var attribute in Property.GetAttributes<TF_TableListActionAttribute>())
            {
                var action = new TableAction();
                action.Name = attribute.Name;
                action.MemberName = attribute.MemberName;
                action.Parent = Property.ParentValues[0];
                action.Place = attribute.Place;
                var actionResolver = ActionResolver.Get(Property, attribute.MemberName, new NamedValue()
                {
                    Type = typeof(TableActionContext)
                });
                if (actionResolver.HasError)
                {
                    action.ErrorMessage = actionResolver.ErrorMessage;
                }
                else
                {
                    action.methodInfo = actionResolver.Action.GetMethodInfo();
                }
                TableActions.Add(action);
            }
        }

        private void PrepareSelectionCallbacks()
        {
            TableRowSelectContext.table = Property.ValueEntry.WeakSmartValue;
            foreach (var attribute in Property.GetAttributes<TF_TableListOnRowSelectAttribute>())
            {
                var callback = new TableRowSelectCallBack();
                callback.MemberName = attribute.MemberName;
                callback.Parent = Property.ParentValues[0];
                callback.Delayed = attribute.Delayed;
                
                var actionResolver = ActionResolver.Get(Property, attribute.MemberName, new NamedValue()
                {
                    Type = typeof(TableRowSelectContext)
                });
                if (actionResolver.HasError)
                {
                    callback.ErrorMessage = actionResolver.ErrorMessage;
                }
                else
                {
                    callback.methodInfo = actionResolver.Action.GetMethodInfo();
                }
                RowSelectCallBacks.Add(callback);
            }
        }

        private bool customFilterPrepared = false;
        private void PrepareCustomAttributes()
        {
            customFilterPrepared = true;
            foreach (var attribute in this.Property.GetAttributes<TF_TableListFilterAttribute>())
            {
                if (attribute.FilterType != null && typeof(RowFilter).IsAssignableFrom(attribute.FilterType))
                {
                    var instance = (RowFilter)Activator.CreateInstance(attribute.FilterType);
                    instance.Name = "row filter";
                    instance.TargetType = RowFilterTargetType.Row;
                    instance.Deletable = attribute.Deletable;
                    instance.Initialize();
                    var tree = PropertyTree.Create(instance);
                    LoadPersistentComponent(instance);
                    CustomRowFilters.Add(instance, tree);
                }
            }

            if (Property.Children.Count > 0)
            {
                var row = Property.Children[0];
                foreach (var property in row.Children)
                {
                    foreach (var attribute in property.GetAttributes<TF_TableListFilterAttribute>())
                    {
                        if (attribute.FilterType != null && typeof(RowFilter).IsAssignableFrom(attribute.FilterType))
                        {
                            var instance = (RowFilter)Activator.CreateInstance(attribute.FilterType);
                            instance.Name = property.Name;
                            instance.PropertyName = property.Name;
                            instance.TargetType = RowFilterTargetType.Cell;
                            instance.CellValueGetter = GetCellValueGetter(property.Name);
                            instance.Initialize();
                            var tree = PropertyTree.Create(instance);
                            LoadPersistentComponent(instance);
                            CustomRowFilters.Add(instance, tree);
                        }
                    }

                    if (Attribute.InlineFilter)
                    {
                        if (!InlineRowFilters.ContainsKey(property.Name))
                        {
                            var commonInlineFilter = CreateCommonInlineFilter(property);
                            if (commonInlineFilter != null)
                            {
                                InlineRowFilters[property.Name] = commonInlineFilter;
                                LoadPersistentComponent(commonInlineFilter);
                            }
                        }
                    }

                    foreach (var attribute in property.GetAttributes<TableValidatorAttribute>())
                    {
                        if (attribute.ValidatorType != null && typeof(TableValidator).IsAssignableFrom(attribute.ValidatorType))
                        {
                            var instance = (TableValidator)Activator.CreateInstance(attribute.ValidatorType);
                            if (instance is IColumnValidator columnValidator)
                            {
                                columnValidator.ColName = property.Name;
                                columnValidator.CellValueGetter = GetCellValueGetter(property.Name);
                            }
                            Validators.Add(instance);
                        }
                    }

                    if (property.GetAttribute<TF_TableListStringTooltip>() != null)
                    {
                        enableStringTooltipDict[property.Name] = true;
                    }

                    var quickFilterLimit = property.GetAttribute<TF_TableListQuickFilterLimit>();
                    if (quickFilterLimit != null)
                        QuickFilterLimitDict[property.Name] = quickFilterLimit.Limit;
                }
            }
            var rowFilters = extraTableInfo.RowFilters;
            rowFilters.Clear();
            rowFilters.AddRange(CustomRowFilters.Keys);
        }

        private RowFilter CreateCommonInlineFilter(InspectorProperty inspectorProperty)
        {
            if (inspectorProperty.ValueEntry == null)
                return null;
            var valueType = inspectorProperty.ValueEntry.BaseValueType;
            var resolvedType = InlineRowFilterResolver.Resolve(valueType);
            if (resolvedType != null)
            {
                var instance = (RowFilter)Activator.CreateInstance(resolvedType);
                instance.Name = inspectorProperty.Name;
                instance.PropertyName = inspectorProperty.Name;
                instance.TargetType = RowFilterTargetType.Cell;
                instance.CellValueGetter = GetCellValueGetter(inspectorProperty.Name);
                instance.Initialize();
                return instance;
            }
            return null;
        }

        private void LoadPersistentComponent(ITablePersistentComponent comp)
        {
            var tableContextDataItem = persistentComponentsData?.Value?.list?.Find(x => x.key == comp.PersistentContextKey);
            if (tableContextDataItem != null)
            {
                comp.OnLoadPersistentValue(tableContextDataItem.value);
            }
        }

        private void SavePersistentComponent(ITablePersistentComponent comp, TablePersistentComponentsData container)
        {
            var value = comp.GetPersistentValue();
            if (value != null)
            {
                container.list.Add(new TablePersistentDataItem
                {
                    key = comp.PersistentContextKey,
                    value = value
                });
            }
        }

        /// <summary>
        /// Draws the property layout.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (Property.ValueEntry.ValueCount > 1)
            {
                EditorGUILayout.LabelField("Multi Target Is Not Supported by TF_TableListFilterAttribute");
                return;
            }

            if (!customFilterPrepared && Property.Children.Count > 0)
            {
                PrepareCustomAttributes();
            }
            if (this.drawAsList)
            {
                if (GUILayout.Button("Draw as table"))
                {
                    this.drawAsList = false;
                }

                this.CallNextDrawer(label);
                return;
            }


            this.picker = ObjectPicker.GetObjectPicker(this, this.resolver.ElementType);
            this.paging.Update(this.resolver.MaxCollectionLength);
            this.currPage.Value = this.paging.CurrentPage;
            this.isPagingExpanded.Value = this.paging.IsExpanded;
            this.tooltipStr = null;

            var rect = SirenixEditorGUI.BeginIndentedVertical(SirenixGUIStyles.PropertyPadding);
            {
                this.DrawFilter();
                if (!this.Attribute.HideToolbar)
                {
                    this.DrawToolbar(label);
                }

                if (this.Attribute.AlwaysExpanded)
                {
                    this.DrawColumnHeaders();
                    this.DrawInlineFilters();
                    this.DrawTable();
                }
                else
                {
                    if (SirenixEditorGUI.BeginFadeGroup(this, this.isVisible.Value) && this.Property.Children.Count > 0)
                    {
                        this.DrawColumnHeaders();
                        this.DrawInlineFilters();
                        this.DrawTable();
                    }

                    SirenixEditorGUI.EndFadeGroup();
                }
            }
            SirenixEditorGUI.EndIndentedVertical();
            if (Event.current.type == EventType.Repaint)
            {
                rect.yMin -= 1;
                rect.height -= 3;
                SirenixEditorGUI.DrawBorders(rect, 1);
            }

            this.DropZone(rect);
            this.HandleObjectPickerEvents();

            if (tooltipStr != null)
                DrawStringTooltip();

            if (Event.current.type == EventType.Repaint)
            {
                this.isFirstFrame = false;
            }

            if (sortOrFilterDirty)
                RefreshSortAndFilter();

            if (ValidatorDirty)
            {
                ValidatorDirty = false;
                EditorApplication.delayCall += RunValidation;
            }

            HandleShortcut();
        }

        private void DrawInlineFilters()
        {
            if (!Attribute.InlineFilter || InlineRowFilters.Count == 0)
                return;
            var rect = GUILayoutUtility.GetRect(0, 21);
            var inlineRect = rect;
            var ColIndex = 0;
            for (int i = 0; i < this.columns.Count; i++)
            {
                var col = this.columns[i];
                if (rect.x > this.columnHeaderRect.xMax)
                {
                    break;
                }

                rect.width = col.ColWidth;
                rect.xMax = Mathf.Min(this.columnHeaderRect.xMax, rect.xMax);
                if (col.ColumnType == ColumnType.Index)
                {
                    if (InlineRowFilters.Any(x => x.Value.IsValid && x.Value.IsEnable))
                    {
                        if (SirenixEditorGUI.IconButton(rect.AlignCenter(16), EditorIcons.X))
                        {
                            foreach (var inlineRowFilter in InlineRowFilters)
                            {
                                inlineRowFilter.Value.Reset();
                            }
                        }
                    }
                    else
                    {
                        GUI.Label(rect.AlignCenterXY(16), "筛");
                    }
                }
                else if (!string.IsNullOrEmpty(col.Name) && InlineRowFilters.TryGetValue(col.Name, out var filter))
                {
                    var id = TF_TableRowLayoutGroup.GetControlIdHash(-1, ColIndex);
                    GUIUtility.GetControlID(id, FocusType.Passive);
                    filter.OnDrawInline(rect.Padding(1, 1));
                    if (filter.Dirty)
                    {
                        MarkSortAndFilterDirty();
                        filter.Dirty = false;
                    }
                }
                rect.x += col.ColWidth;
                ++ColIndex;
            }
            if (Event.current.type == EventType.Repaint)
                GUITableUtilities.DrawColumnHeaderSeperators(inlineRect, this.columns, SirenixGUIStyles.BorderColor * 0.4f);
        }

        private enum SelectAllMode
        {
            None, AllFiltered, AllRow
        }

        private SelectAllMode lastSelectAllMode;

        private void SelectAllFiltered()
        {
            lastSelectAllMode = SelectAllMode.AllFiltered;
            foreach (var reOrderedRow in extraTableInfo.ReOrderedRows)
            {
                reOrderedRow.selected = reOrderedRow.filterSuccess;
            }
            extraTableInfo.Update();
        }

        private void SelectAllRow()
        {
            lastSelectAllMode = SelectAllMode.AllRow;
            foreach (var reOrderedRow in extraTableInfo.ReOrderedRows)
            {
                reOrderedRow.selected = true;
            }
            extraTableInfo.Update();
        }


        private void HandleShortcut()
        {
            if (Event.current.type == EventType.KeyDown)
            {
                if (Event.current.keyCode == KeyCode.A && Event.current.control)
                {
                    if (extraTableInfo.FilterSuccessCount > 0)
                    {
                        if (lastSelectAllMode == SelectAllMode.None || lastSelectAllMode == SelectAllMode.AllRow)
                        {
                            EditorApplication.delayCall += SelectAllFiltered;
                        }
                        else if (lastSelectAllMode == SelectAllMode.AllFiltered)
                        {
                            EditorApplication.delayCall += SelectAllRow;
                        }
                    }
                    else
                    {
                        EditorApplication.delayCall += SelectAllRow;
                    }
                    Event.current.Use();
                }
                else if (Event.current.keyCode == KeyCode.C && Event.current.control)
                {
                    if (extraTableInfo.SelectionCount > 0)
                    {
                        var rowIndex = extraTableInfo.ReOrderedRows.First(x => x.selected).origIndex;
                        EditorApplication.delayCall += () => { CopyRow(rowIndex); };
                        Event.current.Use();
                    }
                }
                else if (Event.current.keyCode == KeyCode.V && Event.current.control)
                {
                    if (extraTableInfo.SelectionCount > 0)
                    {
                        var rowIndex = extraTableInfo.ReOrderedRows.First(x => x.selected).origIndex;
                        EditorApplication.delayCall += () => { PasteRow(rowIndex); };
                        Event.current.Use();
                    }
                }
                else if (Event.current.keyCode == KeyCode.D && Event.current.control)
                {
                    if (extraTableInfo.SelectionCount > 0)
                    {
                        var tableType = Property.ValueEntry.TypeOfValue;
                        if (tableType.ImplementsOpenGenericInterface(typeof(IList<>)))
                        {
                            var rowType = tableType.GenericTypeArguments[0];
                            var rows = extraTableInfo.ReOrderedRows.FindAll(x => x.selected);
                            if (rows.Count > 0)
                            {
                                var lastRowIndex = rows[rows.Count - 1].origIndex + 1;
                                foreach (var row in rows)
                                {
                                    var obj = Activator.CreateInstance(rowType);
                                    resolver.QueueInsertAt(lastRowIndex, obj, 0);
                                }
                                foreach (var row in rows)
                                {
                                    EditorApplication.delayCall += () =>
                                    {
                                        CopyRow(row.origIndex);
                                        PasteRow(lastRowIndex);
                                        lastRowIndex += 1;
                                    };
                                }
                                Event.current.Use();
                            }
                        }
                    }
                }
            }
        }

        private void RunValidation()
        {
            ValidatorResult.Reset();
            foreach (var tableValidator in Validators)
            {
                ValidatorResult.Merge(tableValidator.RunValidation(Property.ValueEntry.WeakSmartValue));
            }
            ValidatorResult.RefreshErrorCount();
        }

        private void OnChildValueChanged(int index)
        {
            ValidatorDirty = true;
            var valueEntry = this.Property.Children[index].ValueEntry;
            if (valueEntry == null)
            {
                return;
            }

            if (!typeof(ScriptableObject).IsAssignableFrom(valueEntry.TypeOfValue))
            {
                return;
            }

            for (int i = 0; i < valueEntry.ValueCount; i++)
            {
                var uObj = valueEntry.WeakValues[i] as Object;
                if (uObj)
                {
                    EditorUtility.SetDirty(uObj);
                }
            }
        }

        private void DropZone(Rect rect)
        {
            if (this.isReadOnly) return;

            var eventType = Event.current.type;
            if ((eventType == EventType.DragUpdated || eventType == EventType.DragPerform) && rect.Contains(Event.current.mousePosition))
            {
                Object[] objReferences = null;

                if (DragAndDrop.objectReferences.Any(n => n != null && this.resolver.ElementType.IsAssignableFrom(n.GetType())))
                {
                    objReferences = DragAndDrop.objectReferences.Where(x => x != null && this.resolver.ElementType.IsAssignableFrom(x.GetType())).Reverse().ToArray();
                }
                else if (this.resolver.ElementType.InheritsFrom(typeof(Component)))
                {
                    objReferences = DragAndDrop.objectReferences.OfType<GameObject>().Select(x => x.GetComponent(this.resolver.ElementType)).Where(x => x != null).Reverse().ToArray();
                }
                else if (this.resolver.ElementType.InheritsFrom(typeof(Sprite)) && DragAndDrop.objectReferences.Any(n => n is Texture2D && AssetDatabase.Contains(n)))
                {
                    objReferences = DragAndDrop.objectReferences.OfType<Texture2D>().Select(x =>
                    {
                        var path = AssetDatabase.GetAssetPath(x);
                        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
                    }).Where(x => x != null).Reverse().ToArray();
                }

                bool acceptsDrag = objReferences != null && objReferences.Length > 0;

                if (acceptsDrag)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    Event.current.Use();
                    if (eventType == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        foreach (var obj in objReferences)
                        {
                            object[] values = new object[this.Property.ParentValues.Count];
                            for (int i = 0; i < values.Length; i++)
                            {
                                values[i] = obj;
                            }

                            this.resolver.QueueAdd(values);
                        }
                    }
                }
            }
        }

        private void AddColumns(int rowIndexFrom, int rowIndexTo)
        {
            if (Event.current.type != EventType.Layout)
            {
                return;
            }

            for (int j = this.table.RowIndexFrom; j < this.table.RowIndexTo; j++)
            {
                var y = extraTableInfo.ReOrderedRows[j].origIndex;
                int skip = 0;
                var row = this.Property.Children[y];
                for (int x = 0; x < row.Children.Count; x++)
                {
                    var col = row.Children[x];
                    if (this.seenColumnNames.Add(col.Name))
                    {
                        var hide = GetColumnAttribute<HideInTablesAttribute>(col);
                        if (hide != null)
                        {
                            skip++;
                            continue;
                        }

                        var preserve = false;
                        var resizable = true;
                        var preferWide = true;
                        var width = this.Attribute.DefaultMinColumnWidth;

                        var colAttr = GetColumnAttribute<TableColumnWidthAttribute>(col);
                        if (colAttr != null)
                        {
                            preserve = !colAttr.Resizable;
                            resizable = colAttr.Resizable;
                            width = colAttr.Width;
                            preferWide = false;
                        }

                        Column newCol = new Column(width, preserve, resizable, col.Name, ColumnType.Property);
                        var colName = GetColumnAttribute<TF_TableListColumnNameAttribute>(col);
                        newCol.NiceName = col.NiceName;
                        if (colName != null)
                        {
                            newCol.NiceName = colName.Name;
                        }
                        newCol.NiceNameLabelWidth = (int)SirenixGUIStyles.Label.CalcSize(new GUIContent(newCol.NiceName)).x;
                        newCol.PreferWide = preferWide;

                        var index = x + this.colOffset - skip;
                        this.columns.Insert(Math.Min(index, this.columns.Count), newCol);

                        GUIHelper.RequestRepaint();
                    }
                }
            }
        }

        private void DrawToolbar(GUIContent label)
        {
            const int iconBtnSize = 23;
            var rect = GUILayoutUtility.GetRect(0, 22);
            var isRepaint = Event.current.type == EventType.Repaint;

            // Background
            if (isRepaint)
            {
                SirenixGUIStyles.ToolbarBackground.Draw(rect, GUIContent.none, 0);
            }

            if (!string.IsNullOrEmpty(Attribute.ConfluenceURL))
            {
                var helperRect = rect.AlignRight(iconBtnSize);
                helperRect.width -= 1;
                rect.xMax = helperRect.xMin;
                //ConfluenceHelper.Instance.DrawButton(helperRect, "帮助文档", Attribute.ConfluenceURL);
            }

            // Add
            if (!this.isReadOnly)
            {
                var btnRect = rect.AlignRight(iconBtnSize);
                btnRect.width -= 1;
                rect.xMax = btnRect.xMin;
                if (GUI.Button(btnRect, GUIContent.none, SirenixGUIStyles.ToolbarButton))
                {
                    this.picker.ShowObjectPicker(
                        null,
                        this.Property.GetAttribute<AssetsOnlyAttribute>() == null && !typeof(ScriptableObject).IsAssignableFrom(this.resolver.ElementType),
                        rect,
                        this.Property.ValueEntry.SerializationBackend == SerializationBackend.Unity);
                }

                EditorIcons.Plus.Draw(btnRect, 16);
            }

            // Draw as list toggle
            if (!this.isReadOnly)
            {
                var btnRect = rect.AlignRight(iconBtnSize);
                rect.xMax = btnRect.xMin;
                if (GUI.Button(btnRect, GUIContent.none, SirenixGUIStyles.ToolbarButton))
                {
                    this.drawAsList = !this.drawAsList;
                }

                EditorIcons.HamburgerMenu.Draw(btnRect, 13);
            }

            // Paging
            this.paging.DrawToolbarPagingButtons(ref rect, this.isVisible.Value, true);

            if (extraTableInfo.FilterSuccessCount > 0)
            {
                AddToolbarLabel(ref rect, new GUIContent($"{extraTableInfo.FilterSuccessCount} filtered "), new Color(0.45f, 0.84f, 0.29f));
            }

            if (extraTableInfo.SelectionCount > 0)
            {
                AddToolbarLabel(ref rect, new GUIContent($"{extraTableInfo.SelectionCount} selected "), new Color(0.28f, 0.61f, 0.91f));
            }

            // Errors
            if (ValidatorResult.ErrorCount > 0)
            {
                var errorRect = AddToolbarLabel(ref rect, new GUIContent($"{ValidatorResult.ErrorCount} errors!"), new Color(0.91f, 0.2f, 0.21f));
                if (Event.current.type == EventType.MouseDown && errorRect.Contains(Event.current.mousePosition))
                {
                    ValidatorDirty = true;
                    Event.current.Use();
                }
            }

            // Label
            var labelRect = rect.SetWidth(0);
            if (label != null)
            {
                labelRect.x += 5;
                labelRect.y += 3;
                labelRect.height = 16;
                var labelWidth = (int)SirenixGUIStyles.Label.CalcSize(label).x;
                labelRect.width = labelWidth + 10;
                if (this.Property.Children.Count > 0)
                {
                    GUIHelper.PushHierarchyMode(false);
                    if (this.Attribute.AlwaysExpanded)
                    {
                        GUI.Label(labelRect, label);
                    }
                    else
                    {
                        this.isVisible.Value = SirenixEditorGUI.Foldout(labelRect, this.isVisible.Value, label);
                    }

                    GUIHelper.PushHierarchyMode(true);
                }
                else if (isRepaint)
                {
                    GUI.Label(labelRect, label);
                }
            }

            var startX = label == null ? rect.xMin : labelRect.xMax + 10;
            foreach (var tableAction in TableActions)
            {
                if (tableAction.Place.HasFlag(TableActionPlace.Toolbar))
                {
                    var buttonRect = rect.SetX(startX);
                    var content = new GUIContent(tableAction.Name);
                    buttonRect.width = (int)SirenixGUIStyles.Button.CalcSize(content).x;
                    if (GUI.Button(buttonRect, content))
                    {
                        EditorApplication.delayCall += () =>
                        {
                            UpdateTableActionContext();
                            tableAction.DoAction(TableActionContext);
                        };
                    }
                    startX = buttonRect.xMax + 5;
                }
            }
        }

        private Rect AddToolbarLabel(ref Rect rect, GUIContent content, Color color)
        {
            var labelWidth = (int)SirenixGUIStyles.LeftAlignedCenteredLabel.CalcSize(content).x;
            var labelRect = rect.AlignRight(labelWidth);
            rect.xMax = labelRect.xMin;
            GUIHelper.PushColor(color);
            GUI.Label(labelRect, content, SirenixGUIStyles.LeftAlignedCenteredLabel);
            GUIHelper.PopColor();
            return labelRect;
        }

        private void DrawColumnHeaders()
        {
            if (this.Property.Children.Count == 0)
            {
                return;
            }

            this.columnHeaderRect = GUILayoutUtility.GetRect(0, 21);

            this.columnHeaderRect.height += 1;
            this.columnHeaderRect.y -= 1;

            if (Event.current.type == EventType.Repaint)
            {
                SirenixEditorGUI.DrawBorders(this.columnHeaderRect, 1);
                EditorGUI.DrawRect(this.columnHeaderRect, SirenixGUIStyles.ColumnTitleBg);
            }

            var offset = this.columnHeaderRect.width - this.table.ContentRect.width;
            this.columnHeaderRect.width -= offset;
            GUITableUtilities.ResizeColumns(this.columnHeaderRect, this.columns);

            if (Event.current.type == EventType.Repaint)
            {
                GUITableUtilities.DrawColumnHeaderSeperators(this.columnHeaderRect, this.columns, SirenixGUIStyles.BorderColor);

                var rect = this.columnHeaderRect;
                for (int i = 0; i < this.columns.Count; i++)
                {
                    var col = this.columns[i];
                    if (rect.x > this.columnHeaderRect.xMax)
                    {
                        break;
                    }

                    rect.width = col.ColWidth;
                    rect.xMax = Mathf.Min(this.columnHeaderRect.xMax, rect.xMax);

                    if (col.NiceName != null)
                    {
                        var lblRect = rect;
                        GUI.Label(lblRect, col.NiceName, SirenixGUIStyles.LabelCentered);
                    }

                    rect.x += col.ColWidth;
                }
            }

            DrawColumnHeaderActions();
        }

        private void DrawColumnHeaderActions()
        {
            var rect = this.columnHeaderRect;
            for (var index = 0; index < this.columns.Count; index++)
            {
                var col = this.columns[index];
                if (rect.x > this.columnHeaderRect.xMax)
                {
                    break;
                }

                rect.width = col.ColWidth;
                rect.xMax = Mathf.Min(this.columnHeaderRect.xMax, rect.xMax);
                var buttonRect = rect.AlignRight(rect.height);
                var popupPos = GUIUtility.GUIToScreenPoint(buttonRect.position);
                var mousePosition = Event.current.mousePosition;
                if (Attribute.AlwaysShowColumnHeaderButton || rect.Contains(mousePosition))
                {
                    if (col.ColumnType == ColumnType.Property)
                    {
                        if (Event.current.type == EventType.MouseDown && buttonRect.Contains(Event.current.mousePosition))
                        {
                            OnClickColumnHeader(col, popupPos);
                        }
                        else if (Event.current.type == EventType.Repaint)
                        {
                            if (buttonRect.Contains(mousePosition))
                            {
                                GUI.DrawTexture(buttonRect, EditorIcons.TriangleDown.Highlighted);
                            }
                            else
                            {
                                GUI.DrawTexture(buttonRect, EditorIcons.TriangleDown.Active);
                            }
                        }
                    }
                    else if (col.ColumnType == ColumnType.Index)
                    {
                        if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
                        {
                            HandleIndexHeaderClick();
                        }
                    }
                }
                rect.x += col.ColWidth;
            }
        }

        private void HandleIndexHeaderClick()
        {
            if (Event.current.button == 1 || Event.current.button == 0)
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("应用当前排序"), false, () => { EditorApplication.delayCall += ApplyCurrentSorting; });
                menu.ShowAsContext();
            }
        }

        private void ApplyCurrentSorting()
        {
            if (Property.ValueEntry.WeakSmartValue is IList list && list.Count == extraTableInfo.ReOrderedRows.Count)
            {
                var ret = extraTableInfo.ReOrderedRows.Select(x => list[x.origIndex]).ToList();
                list.Clear();
                foreach (object obj in ret)
                {
                    list.Add(obj);
                }
                extraTableInfo.ReOrderedRows.Clear();
            }
        }

        private void OnClickColumnHeader(Column col, Vector2 pos)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("升序"), false, () =>
            {
                SetSortColumnAndMode(col.Name, SortMode.ASCENDING);
                MarkSortAndFilterDirty();
            });
            menu.AddItem(new GUIContent("降序"), false, () =>
            {
                SetSortColumnAndMode(col.Name, SortMode.DESCENDING);
                MarkSortAndFilterDirty();
            });
            menu.AddItem(new GUIContent("取消排序"), false, () =>
            {
                SetSortColumnAndMode(col.Name, SortMode.NONE);
                MarkSortAndFilterDirty();
            });
            menu.AddItem(new GUIContent("筛选（by文字）"), false, () =>
            {
                EditorWindow window = null;
                Action cancel = () => { EditorApplication.delayCall += window.Close; };

                Action<string> confirm = (filterStr) =>
                {
                    EditorApplication.delayCall += window.Close;
                    OnConfirmFilterStr(col.Name, filterStr);
                };
                _stringFilterEdit = new StringFilterEdit("", confirm, cancel);
                var popupPos = GUIUtility.ScreenToGUIPoint(pos);
                window = OdinEditorWindow.InspectObjectInDropDown(this._stringFilterEdit, new Rect(popupPos.x, popupPos.y, 10, 10), 300);
            });
            menu.AddItem(new GUIContent("筛选（by脚本）"), false, () =>
            {
                EditorWindow window = null;
                Action cancel = () => { EditorApplication.delayCall += window.Close; };

                Action<string> confirm = (filterStr) =>
                {
                    EditorApplication.delayCall += window.Close;
                    OnConfirmFilterScriptStr(col.Name, filterStr);
                };
                _scriptFilterEdit = new ScriptFilterEdit("", confirm, cancel);
                var popupPos = GUIUtility.ScreenToGUIPoint(pos);
                window = OdinEditorWindow.InspectObjectInDropDown(this._scriptFilterEdit, new Rect(popupPos.x, popupPos.y, 10, 10), 300);
            });

            menu.AddItem(new GUIContent("筛选（Distinct Values）"), false, () =>
            {
                var name = col.Name;
                EditorApplication.delayCall += () => { OnConfirmDistinctFilter(name); };
            });

            menu.AddItem(new GUIContent("批量修改"), false, () =>
            {
                EditorWindow window = null;
                Action cancel = () => { EditorApplication.delayCall += window.Close; };

                Action<string, bool> confirm = (modifyStr, applyToSelection) =>
                {
                    EditorApplication.delayCall += window.Close;
                    OnConfirmModifyScriptStr(col.Name, modifyStr, applyToSelection);
                };

                Action<object, bool> confirmValue = (value, applyToSelection) =>
                {
                    EditorApplication.delayCall += window.Close;
                    OnConfirmModifyValue(col.Name, value, applyToSelection);
                };
                List<object> previewTargetList = new List<object>();
                foreach (var reOrderedRow in extraTableInfo.ReOrderedRows)
                {
                    var target = Property.Children[reOrderedRow.origIndex].Children[col.Name];
                    var obj = target.ValueEntry.WeakSmartValue;
                    if (!previewTargetList.Contains(obj))
                    {
                        previewTargetList.Add(obj);
                        if (previewTargetList.Count >= 3)
                            break;
                    }
                }

                string defaultStr = "";
                modifyByScriptStrCache.TryGetValue(col.Name, out defaultStr);
                _scriptModifyEdit = new ScriptModifyEdit(defaultStr, confirm, cancel, confirmValue, previewTargetList);
                var popupPos = GUIUtility.ScreenToGUIPoint(pos);
                window = OdinEditorWindow.InspectObjectInDropDown(this._scriptModifyEdit, new Rect(popupPos.x, popupPos.y, 10, 10), 500);
            });

            var getter = GetCellValueGetter(col.Name);
            Dictionary<object, int> countDict = new Dictionary<object, int>();
            if (getter != null && Property.ValueEntry.WeakSmartValue is IList list)
            {
                foreach (var reOrderedRow in extraTableInfo.ReOrderedRows)
                {
                    var cell = getter(list[reOrderedRow.origIndex]);
                    if (cell != null)
                    {
                        if (countDict.TryGetValue(cell, out var t))
                        {
                            countDict[cell] = t + 1;
                        }
                        else
                        {
                            countDict.Add(cell, 1);
                        }
                    }
                }
            }

            var takeNum = 20;
            if (QuickFilterLimitDict.TryGetValue(col.Name, out var num))
            {
                takeNum = num < 0 ? Int32.MaxValue : num;
            }

            var values = countDict.Select(x => new { key = x.Key, count = x.Value }).OrderByDescending(g => g.count).Take(takeNum).ToList();

            if (values.Count > 0)
            {
                menu.AddSeparator("");
            }

            foreach (var value in values)
            {
                menu.AddItem(new GUIContent($"{value.key}({value.count.ToString()})"), false, () => { OnConfirmValueFilter(col.Name, value.key); });
            }

            menu.ShowAsContext();
        }


        private void DrawTable()
        {
            GUIHelper.PushHierarchyMode(false);
            this.table.DrawScrollView = this.Attribute.DrawScrollView && (this.paging.IsExpanded || !this.paging.IsEnabled);
            this.table.ScrollPos = this.scrollPos.Value;
            var totalRowCount = this.paging.EndIndex - this.paging.StartIndex;
            if (totalRowCount != extraTableInfo.ReOrderedRows.Count)
            {
                extraTableInfo.Reset();
                for (int i = 0; i < totalRowCount; i++)
                {
                    extraTableInfo.ReOrderedRows.Add(new RowExtraInfo(i));
                }
                MarkSortAndFilterDirty();
            }

            this.table.BeginTable(extraTableInfo.ReOrderedRows.Count);
            {
                this.AddColumns(this.table.RowIndexFrom, this.table.RowIndexTo);
                this.DrawListItemBackGrounds();

                var currX = 0f;
                for (int i = 0; i < this.columns.Count; i++)
                {
                    var col = this.columns[i];

                    var colWidth = (int)col.ColWidth;
                    if (this.isFirstFrame && col.PreferWide)
                    {
                        // First frame is often rendered with minWidth becase we don't know the full width yet.
                        // resulting in very tall rows. This tweak will give a better first guess at how tall a row is.
                        colWidth = 200;
                    }

                    this.table.BeginColumn((int)currX, colWidth);
                    GUIHelper.PushLabelWidth(colWidth * 0.3f);
                    currX += col.ColWidth;
                    for (int j = this.table.RowIndexFrom; j < this.table.RowIndexTo; j++)
                    {
                        var rowId = extraTableInfo.ReOrderedRows[j].origIndex;
                        this.table.BeginCell(j);
                        DrawCell(col, j, rowId);
                        this.table.EndCell(j);
                    }

                    GUIHelper.PopLabelWidth();
                    this.table.EndColumn();
                }

                this.DrawRightClickContextMenuAreas();
            }
            this.table.EndTable();
            this.scrollPos.Value = this.table.ScrollPos;
            this.DrawColumnSeperators();

            GUIHelper.PopHierarchyMode();

            if (this.columns.Count > 0 && this.columns[0].ColumnType == ColumnType.Index)
            {
                // The indexLabelWidth changes: (1 - 10 - 100 - 1000)
                this.columns[0].ColWidth = this.indexLabelWidth;
                this.columns[0].MinWidth = this.indexLabelWidth;
            }
        }

        private void DrawColumnSeperators()
        {
            if (Event.current.type == EventType.Repaint)
            {
                var bcol = SirenixGUIStyles.BorderColor;
                bcol.a *= 0.4f;
                var r = this.table.OuterRect;
                GUITableUtilities.DrawColumnHeaderSeperators(r, this.columns, bcol);
            }
        }

        private void DrawListItemBackGrounds()
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }

            for (int j = this.table.RowIndexFrom; j < this.table.RowIndexTo; j++)
            {
                var rowId = extraTableInfo.ReOrderedRows[j].origIndex;
                var col = new Color();
                var rect = this.table.GetRowRect(j);
                col = j % 2 == 0 ? SirenixGUIStyles.ListItemColorEven : SirenixGUIStyles.ListItemColorOdd;

                if (extraTableInfo.ReOrderedRows[j].selected)
                {
                    EditorGUI.DrawRect(rect, new Color(0.15f, 0.35f, 0.71f));
                    col = j % 2 == 0 ? new Color(0.27f, 0.27f, 0.35f) : new Color(0.22f, 0.22f, 0.35f);
                    EditorGUI.DrawRect(rect.Padding(1), col);
                }
                else
                {
                    EditorGUI.DrawRect(rect, col);
                }
            }
        }

        private void DrawRightClickContextMenuAreas()
        {
            for (int j = this.table.RowIndexFrom; j < this.table.RowIndexTo; j++)
            {
                int rowId = extraTableInfo.ReOrderedRows[j].origIndex;
                if (rowId >= 0 && rowId < this.Property.Children.Count)
                {
                    var rect = this.table.GetRowRect(j);
                    this.Property.Children[rowId].Update();
                    PropertyContextMenuDrawer.AddRightClickArea(this.Property.Children[rowId], rect);
                }
            }
        }

        private int lastSelectViewIndex = -1;
        private void DrawCell(Column col, int viewIndex, int rowIndex)
        {
            rowIndex += this.paging.StartIndex;

            if (col.ColumnType == ColumnType.Index)
            {
                var cellRect = GUIHelper.GetCurrentLayoutRect().SetHeight(table.GetRowRect(viewIndex).height);
                Rect rect = GUILayoutUtility.GetRect(0, 16);
                rect.xMin += 5;
                rect.width -= 2;
                var extraRowInfo = extraTableInfo.ReOrderedRows[viewIndex];
                if (Event.current.type == EventType.Repaint)
                {
                    indexLabel.text = rowIndex.ToString();
                    if (extraRowInfo.filterSuccess)
                        EditorGUI.DrawRect(cellRect.Padding(2), new Color(0f, 0.3f, 0f));
                    if (Attribute.IndexRowStyle == IndexRowStyle.Normal)
                    {
                        GUI.Label(rect, indexLabel, SirenixGUIStyles.Label);
                        var labelWidth = (int)SirenixGUIStyles.Label.CalcSize(indexLabel).x;
                        this.indexLabelWidth = Mathf.Max(this.indexLabelWidth, labelWidth + 15);
                    }
                    else if (Attribute.IndexRowStyle == IndexRowStyle.Checkbox)
                    {
                        GUI.Toggle(rect, extraRowInfo.selected, "");
                        this.indexLabelWidth = 30;
                    }
                }

                if (Event.current.type == EventType.MouseDown)
                {
                    if (cellRect.Contains(Event.current.mousePosition))
                    {
                        HandleIndexClick(viewIndex, extraRowInfo);
                    }
                }
            }
            else if (col.ColumnType == ColumnType.DeleteButton)
            {
                Rect rect = GUILayoutUtility.GetRect(20, 20).AlignCenter(16);
                if (SirenixEditorGUI.IconButton(rect, EditorIcons.X))
                {
                    this.resolver.QueueRemoveAt(rowIndex);
                }
            }
            else if (col.ColumnType == ColumnType.Property)
            {
                if (rowIndex >= 0 && rowIndex < this.Property.Children.Count)
                {
                    var cell = this.Property.Children[rowIndex].Children[col.Name];
                    DrawValidatorResult(col, viewIndex, cell);
                    HandleCellClick(col, viewIndex, cell);
                    CheckStringTooltip(col, viewIndex, cell);
                    if (cell != null)
                    {
                        cell.Draw(null);
                    }
                }
            }
            else
            {
                throw new NotImplementedException(col.ColumnType.ToString());
            }
        }

        private static GUIStyle tooltipStyle = new GUIStyle("Tooltip")
        {
            alignment = TextAnchor.MiddleLeft,
        };


        private Dictionary<string, bool> enableStringTooltipDict = new Dictionary<string, bool>();
        private string tooltipStr = null;
        private Vector2 tooltipPos;


        private void CheckStringTooltip(Column col, int viewIndex, InspectorProperty cell)
        {
            if (enableStringTooltipDict.TryGetValue(col.Name, out var ret) && ret)
            {
                var value = cell?.ValueEntry?.WeakSmartValue;
                if (value != null)
                {
                    if (GUIHelper.GetCurrentLayoutRect().Contains(Event.current.mousePosition))
                    {
                        tooltipPos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
                        tooltipStr = value.ToString();
                    }
                }
            }
        }

        private void DrawStringTooltip()
        {
            GUIContent content = new GUIContent();
            content.text = tooltipStr;
            Vector2 size = tooltipStyle.CalcSize(content);
            Vector2 offset = GUIUtility.ScreenToGUIPoint(tooltipPos) - size;
            if (offset.y < 10) offset.y = 10;
            Rect rect = new Rect(offset, size).Expand(3);
            EditorGUI.LabelField(rect, content, tooltipStyle);
        }

        private void DrawValidatorResult(Column col, int viewIndex, InspectorProperty cell)
        {
            if (Event.current.type == EventType.Repaint)
            {
                var rowId = extraTableInfo.ReOrderedRows[viewIndex].origIndex;
                if (ValidatorResult.InvalidCells.ContainsKeys(rowId, col.Name))
                {
                    var rect = GUIHelper.GetCurrentLayoutRect();
                    EditorGUI.DrawRect(rect, new Color(0.3f, 0.05f, 0.06f));
                }
            }
        }

        private void HandleCellClick(Column col, int viewIndex, InspectorProperty cell)
        {
            if (Event.current.type == EventType.MouseDown)
            {
                if (GUIHelper.GetCurrentLayoutRect().Contains(Event.current.mousePosition))
                {
                    if (EnableRowSelectionColumnDict.TryGetValue(col.Name, out var enable))
                    {
                        if (enable)
                        {
                            HandleRowSelectClick(viewIndex, extraTableInfo.ReOrderedRows[viewIndex]);
                        }
                    }
                    else if (cell != null)
                    {
                        EnableRowSelectionColumnDict[col.Name] = cell.GetAttribute<EnableRowSelectionAttribute>() != null;
                    }
                }
            }
        }

        private bool CopyRow(int rowIndex)
        {
            var property = Property.Children[rowIndex];
            var valueToCopy = property.ValueEntry.WeakSmartValue;
            bool isUnityObject = property.ValueEntry.BaseValueType.InheritsFrom(typeof(Object));
            bool hasValue = valueToCopy != null;
            if (hasValue)
            {
                if (isUnityObject)
                {
                    Clipboard.Copy(valueToCopy, CopyModes.CopyReference);
                }
                else if (property.ValueEntry.TypeOfValue.IsNullableType() == false)
                {
                    Clipboard.Copy(valueToCopy, CopyModes.CopyReference);
                }
                else if (property.ValueEntry.SerializationBackend == SerializationBackend.Unity)
                {
                    Clipboard.Copy(valueToCopy, CopyModes.DeepCopy);
                }
                return true;
            }
            return false;
        }

        private void PasteRow(int rowIndex)
        {
            var property = Property.Children[rowIndex];
            bool canPaste = Clipboard.CanPaste(property.ValueEntry.BaseValueType);
            bool isEditable = property.ValueEntry.IsEditable;
            if (canPaste && isEditable)
            {
                property.ValueEntry.WeakSmartValue = Clipboard.Paste();
                GUIHelper.RequestRepaint();
            }
        }

        private void HandleIndexClick(int viewIndex, RowExtraInfo extraRowInfo)
        {
            if (Event.current.button == 0)
            {
                HandleRowSelectClick(viewIndex, extraRowInfo);
            }
            else if (Event.current.button == 1)
            {
                var menu = new GenericMenu();
                var property = Property.Children[extraRowInfo.origIndex];
                var objs = property.ValueEntry.WeakValues.FilterCast<object>().Where(x => x != null).ToArray();
                var valueToCopy = (objs == null || objs.Length == 0) ? null : (objs.Length == 1 ? objs[0] : objs);
                bool isUnityObject = property.ValueEntry.BaseValueType.InheritsFrom(typeof(Object));
                bool hasValue = valueToCopy != null;
                bool canPaste = Clipboard.CanPaste(property.ValueEntry.BaseValueType);
                bool isEditable = property.ValueEntry.IsEditable;
                if (canPaste && isEditable)
                {
                    menu.AddItem(new GUIContent("粘贴"), false, () =>
                    {
                        property.Tree.DelayActionUntilRepaint(() =>
                        {
                            for (int i = 0; i < property.ValueEntry.ValueCount; i++)
                            {
                                property.ValueEntry.WeakValues[i] = Clipboard.Paste();
                            }
                            // Apply happens after the action is invoked in repaint
                            //property.ValueEntry.ApplyChanges();
                            GUIHelper.RequestRepaint();
                        });
                    });
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent("粘贴"));
                }

                if (hasValue)
                {
                    if (isUnityObject)
                    {
                        menu.AddItem(new GUIContent("复制"), false, () => Clipboard.Copy(valueToCopy, CopyModes.CopyReference));
                    }
                    else if (property.ValueEntry.TypeOfValue.IsNullableType() == false)
                    {
                        menu.AddItem(new GUIContent("复制"), false, () => Clipboard.Copy(valueToCopy, CopyModes.CopyReference));
                    }
                    else if (property.ValueEntry.SerializationBackend == SerializationBackend.Unity)
                    {
                        menu.AddItem(new GUIContent("复制"), false, () => Clipboard.Copy(valueToCopy, CopyModes.DeepCopy));
                    }
                    else
                    {
                        menu.AddDisabledItem(new GUIContent("复制"));
                    }
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent("复制"));
                }
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("插入"), false, () =>
                {
                    var tableType = Property.ValueEntry.TypeOfValue;
                    if (tableType.ImplementsOpenGenericInterface(typeof(IList<>)))
                    {
                        var rowType = tableType.GenericTypeArguments[0];
                        var obj = Activator.CreateInstance(rowType);
                        resolver.QueueInsertAt(extraRowInfo.origIndex, obj, 0);
                    }
                });
                menu.AddItem(new GUIContent("删除"), false, () =>
                {
                    if (extraRowInfo.selected)
                    {
                        EditorApplication.delayCall += DeleteSelectedRows;
                    }
                    else
                    {
                        resolver.QueueRemoveAt(extraRowInfo.origIndex);
                    }
                    ValidatorDirty = true;
                });
                menu.AddSeparator("");
                var pos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
                menu.AddItem(new GUIContent("Inspect"), false, () =>
                {
                    var pos2 = GUIUtility.ScreenToGUIPoint(pos);
                    OdinEditorWindow.InspectObjectInDropDown(property.ValueEntry.WeakSmartValue, new Rect(pos2.x, pos2.y, 10, 10), 400);
                });
                menu.AddSeparator("");
                foreach (var tableAction in TableActions)
                {
                    if (tableAction.Place.HasFlag(TableActionPlace.IndexRMenu))
                        menu.AddItem(new GUIContent(tableAction.Name), false, () =>
                        {
                            UpdateTableActionContext();
                            tableAction.DoAction(TableActionContext);
                        });
                }

                menu.ShowAsContext();
            }
        }

        private void HandleRowSelectClick(int viewIndex, RowExtraInfo extraRowInfo)
        {
            if (!enableMultiRowSelect)
            {
                var target = !extraRowInfo.selected;
                foreach (var reOrderedRow in extraTableInfo.ReOrderedRows)
                {
                    reOrderedRow.selected = false;
                }

                extraRowInfo.selected = target;
                lastSelectViewIndex = viewIndex;
                if (extraRowInfo.selected)
                {
                    OnRowSelect(viewIndex, extraTableInfo);
                }
                Event.current.Use();
            }
            else
            {

                if (Event.current.control)
                {
                    extraRowInfo.selected = !extraRowInfo.selected;
                    if (extraRowInfo.selected)
                    {
                        lastSelectViewIndex = viewIndex;
                    }
                }
                else if (Event.current.shift && lastSelectViewIndex != -1)
                {
                    var min = Mathf.Min(lastSelectViewIndex, viewIndex);
                    var max = Mathf.Max(lastSelectViewIndex, viewIndex);
                    min = Mathf.Min(min, extraTableInfo.ReOrderedRows.Count);
                    max = Mathf.Min(max, extraTableInfo.ReOrderedRows.Count);
                    for (int i = min; i <= max; i++)
                    {
                        extraTableInfo.ReOrderedRows[i].selected = true;
                    }
                }
                else
                {
                    var target = !extraRowInfo.selected;
                    foreach (var reOrderedRow in extraTableInfo.ReOrderedRows)
                    {
                        reOrderedRow.selected = false;
                    }

                    extraRowInfo.selected = target;
                    lastSelectViewIndex = viewIndex;
                    if (extraRowInfo.selected)
                    {
                        OnRowSelect(viewIndex, extraTableInfo);
                    }
                    Event.current.Use();
                }
            }
            extraTableInfo.Update();
        }

        private void OnRowSelect(int viewIndex, ExtraTableInfo extraTableInfo)
        {
            var origIndex = extraTableInfo.ReOrderedRows[viewIndex].origIndex;
            UpdateTableActionContext();
            var table = TableRowSelectContext.table as IList;
            if (table == null)
                return;
            if (origIndex >= table.Count)
                return;
            TableRowSelectContext.row = table[origIndex];
            foreach (var callBack in RowSelectCallBacks)
            {
                callBack.DoAction(TableRowSelectContext);
            }
        }

        private void DeleteSelectedRows()
        {
            var list = Property.ValueEntry.WeakSmartValue as IList;
            if (list == null) return;
            extraTableInfo.ReOrderedRows.Where(x => x.selected).Select(x => x.origIndex).OrderByDescending(x => x).ForEach(x =>
            {
                list.RemoveAt(x);
            });
            extraTableInfo.Reset();
            Property.Children.Update();
            //需要清理下列表元素
            if (this.mClearAndDisposeChildrenFunc != null)
            {
                this.mClearAndDisposeChildrenFunc.Invoke(Property.Children, new object[] { });
            }
            MarkSortAndFilterDirty();
        }


        private void HandleObjectPickerEvents()
        {
            if (this.picker.IsReadyToClaim && Event.current.type == EventType.Repaint)
            {
                var value = this.picker.ClaimObject();
                object[] values = new object[this.Property.Tree.WeakTargets.Count];
                values[0] = value;
                for (int j = 1; j < values.Length; j++)
                {
                    values[j] = Sirenix.Serialization.SerializationUtility.CreateCopy(value);
                }

                this.resolver.QueueAdd(values);
            }
        }

        private IEnumerable<InspectorProperty> EnumerateGroupMembers(InspectorProperty groupProperty)
        {
            for (int i = 0; i < groupProperty.Children.Count; i++)
            {
                var info = groupProperty.Children[i].Info;
                if (info.PropertyType != PropertyType.Group)
                {
                    yield return groupProperty.Children[i];
                }
                else
                {
                    foreach (var item in EnumerateGroupMembers(groupProperty.Children[i]))
                    {
                        yield return item;
                    }
                }
            }
        }

        private T GetColumnAttribute<T>(InspectorProperty col)
            where T : Attribute
        {
            T colAttr;
            if (col.Info.PropertyType == PropertyType.Group)
            {
                colAttr = EnumerateGroupMembers(col)
                    .Select(c => c.Info.GetAttribute<T>())
                    .FirstOrDefault(c => c != null);
            }
            else
            {
                colAttr = col.GetAttribute<T>();
            }

            return colAttr;
        }

        #region sort & filter


        private void SetSortColumnAndMode(string name, SortMode mode)
        {
            lastSortCol = name;
            lastSortMode = mode;
        }

        private void RefreshSortAndFilter()
        {
            sortOrFilterDirty = false;
            EditorApplication.delayCall += () =>
            {
                SortColumn();
                ApplyFilter();
            };
        }

        private void MarkSortAndFilterDirty()
        {
            sortOrFilterDirty = true;
        }

        private void SortColumn()
        {
            if (lastSortMode == SortMode.NONE)
            {
                extraTableInfo.ReOrderedRows.Sort((x, y) => x.origIndex.CompareTo(y.origIndex));
                return;
            }

            Dictionary<int, IComparable> dict = new Dictionary<int, IComparable>();
            var totalCount = this.paging.EndIndex - this.paging.StartIndex;
            if (totalCount == 0)
                return;
            var row = this.Property.Children[0];
            var cellType = row.Children[lastSortCol]?.ValueEntry?.TypeOfValue;
            if (!typeof(IComparable).IsAssignableFrom(cellType))
                return;
            if (cellType == null) return;
            var valueGetter = DeepReflection.CreateWeakInstanceValueGetter(row.ValueEntry.TypeOfValue, cellType, lastSortCol, true);
            var list = Property.ValueEntry.WeakSmartValue as IList;
            if (list == null) return;
            for (var index = 0; index < list.Count; index++)
            {
                var obj = list[index];
                var cell = valueGetter(obj);
                if (cell == null) return;
                dict.Add(index, (IComparable)cell);
            }

            if (dict.Count == totalCount)
            {
                if (lastSortMode == SortMode.ASCENDING)
                    extraTableInfo.ReOrderedRows.Sort((x, y) => dict[x.origIndex].CompareTo(dict[y.origIndex]));
                else if (lastSortMode == SortMode.DESCENDING)
                    extraTableInfo.ReOrderedRows.Sort((x, y) => dict[y.origIndex].CompareTo(dict[x.origIndex]));
            }
        }

        private enum SortMode
        {
            NONE,
            ASCENDING,
            DESCENDING,
        }

        private Dictionary<string, Func<object, object>> cellValueGetterDict = new Dictionary<string, Func<object, object>>();

        public class TableColumnFilterSet
        {
            public FilterConditionMode mode = FilterConditionMode.And;
            public Dictionary<RowFilter, PropertyTree> FilterUnits = new Dictionary<RowFilter, PropertyTree>();

            public bool ApplyFilter(object row)
            {
                if (mode == FilterConditionMode.And)
                    return FilterUnits.All(x => !x.Key.IsValid || !x.Key.IsEnable || x.Key.SuccessRow(row));
                else if (mode == FilterConditionMode.Or)
                    return FilterUnits.Any(x => x.Key.IsValid && x.Key.IsEnable && x.Key.SuccessRow(row));
                else
                    return false;
            }
        }

        public class TableFilterSet
        {
            public FilterConditionMode mode = FilterConditionMode.And;
            public Dictionary<string, TableColumnFilterSet> FilterSet = new Dictionary<string, TableColumnFilterSet>();

            public bool ApplyFilter(object row)
            {
                if (mode == FilterConditionMode.And)
                    return FilterSet.All(x => x.Value.ApplyFilter(row));
                else if (mode == FilterConditionMode.Or)
                    return FilterSet.Any(x => x.Value.ApplyFilter(row));
                return false;
            }

            public void AddFilter(string col, RowFilter filter)
            {
                if (!FilterSet.TryGetValue(col, out var columnFilterSet))
                {
                    var columnSet = new TableColumnFilterSet();
                    columnSet.FilterUnits.Add(filter, PropertyTree.Create(filter));
                    FilterSet.Add(col, columnSet);
                }
                else
                {
                    columnFilterSet.FilterUnits.Clear();
                    columnFilterSet.FilterUnits.Add(filter, PropertyTree.Create(filter));
                }
            }

            public void RemoveFilter(RowFilter key)
            {
                foreach (var tableColumnFilterSet in FilterSet)
                {
                    tableColumnFilterSet.Value.FilterUnits.Remove(key);
                }
            }
        }

        private void OnConfirmDistinctFilter(string col)
        {
            var filter = new DistinctFilter();
            filter.PropertyName = col;
            filter.TargetType = RowFilterTargetType.Cell;
            filter.CellValueGetter = GetCellValueGetter(col);
            filterSet.AddFilter(col, filter);
            MarkSortAndFilterDirty();
        }

        private void OnConfirmValueFilter(string col, object value)
        {
            var filter = new ValueRowFilter(value);
            filter.PropertyName = col;
            filter.TargetType = RowFilterTargetType.Cell;
            filter.CellValueGetter = GetCellValueGetter(col);
            filterSet.AddFilter(col, filter);
            MarkSortAndFilterDirty();
        }

        private void OnConfirmModifyValue(string colName, object value, bool applyToSelection)
        {
            foreach (var reOrderedRow in extraTableInfo.ReOrderedRows)
            {
                if (!applyToSelection || reOrderedRow.selected)
                {
                    Property.Children[reOrderedRow.origIndex].Children[colName].ValueEntry.WeakSmartValue = value;
                }
            }
        }

        private Dictionary<string, string> modifyByScriptStrCache = new Dictionary<string, string>();

        private void OnConfirmModifyScriptStr(string colName, string modifyStr, bool applyToSelection)
        {
            if (Property.Children.Count == 0) return;
            var cell = Property.Children[0].Children[colName];
            var type = cell.ValueEntry.TypeOfValue;
            var modifyHelper = new ScriptModifyHelper(type, modifyStr);
            modifyByScriptStrCache[colName] = modifyStr;
            if (!modifyHelper.IsValid)
                return;
            try
            {
                foreach (var reOrderedRow in extraTableInfo.ReOrderedRows)
                {
                    if (!applyToSelection || reOrderedRow.selected)
                    {
                        var target = Property.Children[reOrderedRow.origIndex].Children[colName];
                        var obj = modifyHelper.Process(target.ValueEntry.WeakSmartValue);
                        Property.Children[reOrderedRow.origIndex].Children[colName].ValueEntry.WeakSmartValue = obj;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                throw;
            }
        }

        private void OnConfirmFilterScriptStr(string colName, string filterStr)
        {
            var cell = Property.Children[0].Children[colName];
            var filter = new ScriptRowFilter(cell.ValueEntry.TypeOfValue, filterStr);
            filter.PropertyName = colName;
            filter.TargetType = RowFilterTargetType.Cell;
            filter.CellValueGetter = GetCellValueGetter(colName);
            filterSet.AddFilter(colName, filter);
            MarkSortAndFilterDirty();
        }

        private void OnConfirmFilterStr(string col, string filterStr)
        {
            var filter = new StringRowFilter(filterStr);
            filter.PropertyName = col;
            filter.TargetType = RowFilterTargetType.Cell;
            filter.CellValueGetter = GetCellValueGetter(col);
            filterSet.AddFilter(col, filter);
            MarkSortAndFilterDirty();
        }

        private Func<object, object> GetCellValueGetter(string col)
        {
            if (cellValueGetterDict.TryGetValue(col, out var ret))
                return ret;
            if (Property.Children.Count > 0)
            {
                var row = this.Property.Children[0];
                var cellType = row.Children[col]?.ValueEntry?.BaseValueType;
                if (cellType != null)
                {
                    var valueGetter = DeepReflection.CreateWeakInstanceValueGetter(row.ValueEntry.TypeOfValue, cellType, col, true);
                    if (valueGetter != null)
                    {
                        cellValueGetterDict.Add(col, valueGetter);
                        return valueGetter;
                    }
                }
            }

            var tableType = Property.ValueEntry.TypeOfValue;
            if (tableType.ImplementsOpenGenericInterface(typeof(IList<>)))
            {
                var rowType = tableType.GenericTypeArguments[0];
                try
                {
                    var valueGetter = DeepReflection.CreateWeakInstanceValueGetter<object>(rowType, col, true);
                    if (valueGetter != null)
                    {
                        cellValueGetterDict.Add(col, valueGetter);
                        return valueGetter;
                    }
                }
                catch (Exception e)
                {
                    Debug.Log($"Create Value Getter Fail. Column Name:{col} Exception:{e}");
                }
            }
            return null;
        }

        private void ApplyFilter()
        {
            List<RowExtraInfo> success = new List<RowExtraInfo>();
            List<RowExtraInfo> fail = new List<RowExtraInfo>();

            var pData = new TablePersistentComponentsData();

            foreach (var tableColumnFilterSet in filterSet.FilterSet)
            {
                foreach (var valueFilterUnit in tableColumnFilterSet.Value.FilterUnits)
                {
                    valueFilterUnit.Key.Prepare(Property.ValueEntry.WeakSmartValue);
                    SavePersistentComponent(valueFilterUnit.Key, pData);
                }
            }

            foreach (var customRowFilter in CustomRowFilters)
            {
                customRowFilter.Key.Prepare(Property.ValueEntry.WeakSmartValue);
                SavePersistentComponent(customRowFilter.Key, pData);
            }

            foreach (var inlineRowFilter in InlineRowFilters)
            {
                inlineRowFilter.Value.Prepare(Property.ValueEntry.WeakSmartValue);
                SavePersistentComponent(inlineRowFilter.Value, pData);
            }

            this.persistentComponentsData.Value = pData;

            enableFilter = filterSet.FilterSet.Any(x => x.Value.FilterUnits.Any(y => y.Key.IsEnable && y.Key.IsValid))
                            || CustomRowFilters.Any(x => x.Key.IsValid && x.Key.IsEnable)
                            || InlineRowFilters.Any(x => x.Value.IsValid && x.Value.IsEnable);
            if (!enableFilter)
            {
                foreach (var info in extraTableInfo.ReOrderedRows)
                {
                    info.filterSuccess = false;
                    fail.Add(info);
                }
            }
            else
            {
                var list = Property.ValueEntry.WeakSmartValue as IList;
                if (list == null) return;
                foreach (var info in extraTableInfo.ReOrderedRows)
                {
                    var row = list[info.origIndex];
                    if (filterSet.ApplyFilter(row) && CheckCustomFilter(row) && CheckInlineFilter(row))
                    {
                        info.filterSuccess = true;
                        success.Add(info);
                    }
                    else
                    {
                        info.filterSuccess = false;
                        fail.Add(info);
                    }
                }
            }

            var ReOrderedRows = extraTableInfo.ReOrderedRows;
            ReOrderedRows.Clear();
            ReOrderedRows.AddRange(success);
            ReOrderedRows.AddRange(fail);
            var rowFilters = extraTableInfo.RowFilters;
            rowFilters.Clear();
            rowFilters.AddRange(CustomRowFilters.Keys);
            extraTableInfo.Update();
            GUIHelper.RequestRepaint();
        }


        private bool CheckCustomFilter(object row)
        {
            return CustomRowFilters.All(x => (!x.Key.IsValid || !x.Key.IsEnable || x.Key.SuccessRow(row)));
        }

        private bool CheckInlineFilter(object row)
        {
            return InlineRowFilters.All(x => (!x.Value.IsValid || !x.Value.IsEnable || x.Value.SuccessRow(row)));
        }

        public void DrawFilter()
        {
            foreach (var customRowFilter in CustomRowFilters)
            {
                DrawSingleFilter(customRowFilter);
            }
            foreach (var tableColumnFilterSet in filterSet.FilterSet)
            {
                var data = tableColumnFilterSet.Value;
                foreach (var tableRowFilterUnit in data.FilterUnits)
                {
                    DrawSingleFilter(tableRowFilterUnit);
                }
            }
        }

        private void DrawSingleFilter(KeyValuePair<RowFilter, PropertyTree> customRowFilter)
        {
            var isEnable = customRowFilter.Key.IsEnable;
            SirenixEditorGUI.BeginBox();
            {
                GUILayout.BeginHorizontal();
                {
                    customRowFilter.Key.IsEnable = GUILayout.Toggle(isEnable, "", GUILayoutOptions.MaxWidth(20));
                    GUIHelper.PushGUIEnabled(customRowFilter.Key.IsEnable);
                    GUILayout.BeginVertical();
                    customRowFilter.Value.Draw(false);
                    GUILayout.EndVertical();
                    GUIHelper.PopGUIEnabled();
                    if (customRowFilter.Key.Deletable)
                    {
                        if (SirenixEditorGUI.IconButton(EditorIcons.X))
                        {
                            EditorApplication.delayCall += () =>
                            {
                                DeleteFilter(customRowFilter.Key);
                                MarkSortAndFilterDirty();
                            };
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }
            SirenixEditorGUI.EndBox();
            if (customRowFilter.Key.Dirty || isEnable != customRowFilter.Key.IsEnable)
            {
                MarkSortAndFilterDirty();
                customRowFilter.Key.Dirty = false;
            }
        }

        private void DeleteFilter(RowFilter key)
        {
            CustomRowFilters.Remove(key);
            filterSet.RemoveFilter(key);
        }

        #endregion


        private enum ColumnType
        {
            Property,
            Index,
            DeleteButton,
        }

        private class Column : IResizableColumn
        {
            public string Name;
            public float ColWidth;
            public float MinWidth;
            public bool Preserve;
            public bool Resizable;
            public string NiceName;
            public int NiceNameLabelWidth;
            public ColumnType ColumnType;
            public bool PreferWide;

            public Column(int minWidth, bool preserveWidth, bool resizable, string name, ColumnType colType)
            {
                this.MinWidth = minWidth;
                this.ColWidth = minWidth;
                this.Preserve = preserveWidth;
                this.Name = name;
                this.ColumnType = colType;
                this.Resizable = resizable;
            }

            float IResizableColumn.ColWidth
            {
                get { return this.ColWidth; }
                set { this.ColWidth = value; }
            }

            float IResizableColumn.MinWidth
            {
                get { return this.MinWidth; }
            }

            bool IResizableColumn.PreserveWidth
            {
                get { return this.Preserve; }
            }

            bool IResizableColumn.Resizable
            {
                get { return this.Resizable; }
            }
        }
    }

    [System.Serializable]
    public class TablePersistentDataItem
    {
        public string key;
        public string value;
    }

    [System.Serializable]
    public class TablePersistentComponentsData
    {
        public List<TablePersistentDataItem> list = new List<TablePersistentDataItem>();
    }

    [ResolverPriority(2)]
    internal class ScriptableObjectTableListResolver<T> : BaseMemberPropertyResolver<T>
        where T : ScriptableObject
    {
        private List<OdinPropertyProcessor> processors;

        public override bool CanResolveForPropertyFilter(InspectorProperty property)
        {
            return property.Parent != null && property.Parent.GetAttribute<TableListAttribute>() != null && property.Parent.ChildResolver is IOrderedCollectionResolver;
        }

        protected override InspectorPropertyInfo[] GetPropertyInfos()
        {
            if (this.processors == null)
            {
                this.processors = OdinPropertyProcessorLocator.GetMemberProcessors(this.Property);
            }

            var includeSpeciallySerializedMembers = typeof(T).IsDefined<ShowOdinSerializedPropertiesInInspectorAttribute>(true);
            var infos = InspectorPropertyInfoUtility.CreateMemberProperties(this.Property, typeof(T), includeSpeciallySerializedMembers);

            for (int i = 0; i < this.processors.Count; i++)
            {
                ProcessedMemberPropertyResolverExtensions.ProcessingOwnerType = typeof(T);
                this.processors[i].ProcessMemberProperties(infos);
            }

            return InspectorPropertyInfoUtility.BuildPropertyGroupsAndFinalize(this.Property, typeof(T), infos, includeSpeciallySerializedMembers);
        }
    }
}
#endif