#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;

namespace TF_TableList
{
    using System;
    using UnityEditor;
    using UnityEngine;

    public class TF_TableRowLayoutGroup
    {
        private static GUIStyle marginFix;
        private TableRow[] rows;
        private int spaceBefore;
        private int spaceAfter;
        private int avgCellHeight;
        private int currColIndex;
        private int totalHeight;
        private int rowCount;
        private Vector2 outerVisibleRect;
        private Rect innerVisibleRect;
        private Vector2 nextScrollPos;
        private Rect contentRect;
        private bool ignoreScrollView;
        private int isDirty = 3;

        /// <summary>
        /// Whether to draw a draw scroll view.
        /// </summary>
        public bool DrawScrollView = true;

        /// <summary>
        /// The number of pixels before a scroll view appears.
        /// </summary>
        public int MinScrollViewHeight;

        /// <summary>
        /// The maximum scroll view height.
        /// </summary>
        public int MaxScrollViewHeight;

        /// <summary>
        /// The scroll position
        /// </summary>
        public Vector2 ScrollPos;

        /// <summary>
        /// The cell style
        /// </summary>
        public GUIStyle CellStyle;

        /// <summary>
        /// Gets the rect containing all rows. 
        /// </summary>
        public Rect ContentRect { get { return this.contentRect; } }

        /// <summary>
        /// Gets the first visible row index.
        /// </summary>
        public int RowIndexFrom { get; private set; }

        /// <summary>
        /// Gets the last visible row index.
        /// </summary>
        public int RowIndexTo { get; private set; }

        /// <summary>
        /// Gets the outer rect. The height of this &lt;= <see cref="ContentRect"/>.height.
        /// </summary>
        public Rect OuterRect { get; private set; }

        /// <summary>
        /// Gets the row rect.
        /// </summary>
        public Rect GetRowRect(int index)
        {
            var r = this.rows[index];
            return new Rect(this.contentRect.x, this.contentRect.y + r.yMin, this.contentRect.width, r.yMax - r.yMin);
        }

        /// <summary>
        /// Begins the table.
        /// </summary>
        public void BeginTable(int rowCount)
        {
            marginFix = marginFix ?? new GUIStyle();
            this.currColIndex = 0;
            this.rows = this.rows ?? new TableRow[0];

            if (this.rowCount != rowCount)
            {
                this.isDirty = 3;
                this.rowCount = rowCount;
            }

            this.RowIndexFrom = Math.Min(this.rowCount, this.RowIndexFrom);
            this.RowIndexTo = Math.Min(this.rowCount, this.RowIndexTo);

            if (this.rows.Length != rowCount)
            {
                Array.Resize(ref this.rows, rowCount);
            }

            if (Event.current.type == EventType.Layout)
            {
                for (int i = this.RowIndexFrom; i < this.RowIndexTo; i++)
                {
                    var row = rows[i];
                    if (row.tmpRowHeight > 0)
                    {
                        if (row.rowHeight != row.tmpRowHeight)
                        {
                            this.isDirty = 3;
                            row.rowHeight = row.tmpRowHeight;
                        }

                        row.tmpRowHeight = 0;
                        this.rows[i] = row;
                    }
                }

                if (this.isDirty > 0)
                {
                    this.UpdateSpaceAllocation();
                    GUIHelper.RequestRepaint();
                }
            }

            if (Event.current.type == EventType.Repaint)
            {
                var p = GUIUtility.GUIToScreenPoint(this.OuterRect.position);
                if (this.outerVisibleRect != p)
                {
                    this.outerVisibleRect = p;
                    this.isDirty = 3;
                }
            }

            var outerRect = EditorGUILayout.BeginVertical(this.GetOuterRectLayoutOptions());

            if (Event.current.type != EventType.Layout && outerRect.height > 1 || Event.current.type == EventType.Repaint)
            {
                if (this.OuterRect != outerRect)
                {
                    this.isDirty = 3;
                    this.OuterRect = outerRect;
                }
            }

            Vector2 newScrollPos;
            if (this.DrawScrollBars())
            {
                newScrollPos = GUILayout.BeginScrollView(this.ScrollPos, false, false, this.GetScrollViewOptions(true));
            }
            else
            {
                this.ignoreScrollView = Event.current.rawType == EventType.ScrollWheel;
                if (this.ignoreScrollView) GUIHelper.PushEventType(EventType.Ignore);
                newScrollPos = GUILayout.BeginScrollView(this.ScrollPos, false, false, GUIStyle.none, GUIStyle.none, this.GetScrollViewOptions(false));
                if (this.ignoreScrollView)
                {
                    GUIHelper.PopEventType();
                    this.isDirty = 3;
                }
            }

            if (newScrollPos != this.ScrollPos)
            {
                this.nextScrollPos = newScrollPos;
                this.isDirty = 3;
            }

            var rect = GUILayoutUtility.GetRect(0, this.totalHeight);
            if (Event.current.type != EventType.Layout && rect.width > 1)
            {
                this.innerVisibleRect = GUIClipInfo.VisibleRect;

                if (this.DrawScrollView)
                {
                    var scrollDelta = this.nextScrollPos - this.ScrollPos;
                    this.innerVisibleRect.y += scrollDelta.y;

                    if (scrollDelta != Vector2.zero)
                    {
                        this.isDirty = 3;
                    }
                }

                if (rect != this.contentRect)
                {
                    this.isDirty = 3;
                }

                if (this.contentRect != rect)
                {
                    this.isDirty = 3;
                }

                this.contentRect = rect;
            }

            if (this.isDirty > 0)
            {
                GUIHelper.RequestRepaint();

                if (Event.current.type == EventType.Repaint)
                {
                    this.isDirty--;
                }
            }

        }

        private bool DrawScrollBars()
        {
            return this.DrawScrollView;
        }

        private GUILayoutOption[] GetScrollViewOptions(bool drawScrollBars)
        {
            if (drawScrollBars)
            {
                return GUILayoutOptions.ExpandHeight(false);
            }
            else
            {
                return GUILayoutOptions.Height(this.contentRect.height);
            }
        }

        private GUILayoutOption[] GetOuterRectLayoutOptions()
        {
            if (this.DrawScrollView == false)
            {
                return GUILayoutOptions.Height(this.contentRect.height);
            }

            var g = GUILayoutOptions.ExpandHeight(false);

            if (this.MinScrollViewHeight > 0)
            {
                g = g.MinHeight(Mathf.Min(this.MinScrollViewHeight, this.contentRect.height));
            }

            if (this.MaxScrollViewHeight > 0)
            {
                g = g.MaxHeight(Mathf.Min(this.MaxScrollViewHeight, this.contentRect.height));
            }
            else
            {
                g = g.MaxHeight(Mathf.Min(99999, this.contentRect.height));
            }

            return g;
            //return GUILayoutOptions.MinHeight(minHeight).MaxHeight(maxHeight);
        }

        /// <summary>
        /// Begins the column.
        /// </summary>
        public void BeginColumn(int xOffset, int width)
        {
            this.currColIndex++;
            var rect = this.contentRect;
            rect.x += xOffset;
            rect.width = width;
            GUILayout.BeginArea(rect);
            GUILayout.Space(this.spaceBefore);
        }

        /// <summary>
        /// Begins the cell.
        /// </summary>
        public void BeginCell(int rowIndex)
        {
            ref var row = ref this.rows[rowIndex];
            GUIUtility.GetControlID(GetControlIdHash(rowIndex, currColIndex), FocusType.Passive);
            GUILayout.BeginVertical(row.layoutOptions);
            GUILayout.BeginVertical(this.CellStyle ?? marginFix);
        }

        public static int GetControlIdHash(int row, int col)
        {
            return Math.Abs(117 * row + 3102919 + col * 79) + 1;
        }
        /// <summary>
        /// Ends the cell.
        /// </summary>
        public void EndCell(int rowIndex)
        {
            ref var row = ref this.rows[rowIndex];
            if (Event.current.type == EventType.Repaint)
            {
                var r = GUIHelper.GetCurrentLayoutRect();
                row.tmpRowHeight = Mathf.Max(row.tmpRowHeight, (int)r.height);
            }

            GUILayout.EndVertical();
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Ends the column.
        /// </summary>
        public void EndColumn()
        {
            GUILayout.Space(this.spaceAfter);
            GUILayout.EndArea();
        }

        /// <summary>
        /// Ends the table.
        /// </summary>
        public void EndTable()
        {
            if (this.ignoreScrollView) GUIHelper.PushEventType(EventType.Ignore);
            GUILayout.EndScrollView();
            if (this.ignoreScrollView) GUIHelper.PopEventType();

            if (Event.current.type == EventType.Repaint)
            {
                this.ScrollPos = this.nextScrollPos;
            }

            EditorGUILayout.EndVertical();
        }

        private void UpdateSpaceAllocation()
        {
            var isFirstVisible = true;
            var visibleRectYMin = this.innerVisibleRect.yMin;
            var visibleRectYMax = this.innerVisibleRect.yMax;
            var visibleCellsHeight = 0;
            var visibleCellsCount = 0;

            this.RowIndexFrom = 0;
            this.RowIndexTo = 0;
            this.totalHeight = 0;
            this.spaceBefore = 0;
            this.spaceAfter = 0;

            var yOffset = this.contentRect.y;

            for (int i = 0; i < this.rowCount; i++)
            {
                var row = rows[i];
                var heightToAllocate = row.rowHeight;                                   // When it has been drawn before and has a height
                if (heightToAllocate == 0) heightToAllocate = row.avgRowHeight;         // When it's not been drawn yet, and is below the visibleRect
                if (heightToAllocate == 0) heightToAllocate = this.avgCellHeight;       // When there is no already defined avgHeight.
                if (heightToAllocate == 0) heightToAllocate = 18;                       // When no average height has been set for the cell

                row.yMin = totalHeight;
                row.yMax = totalHeight + heightToAllocate;
                var isVisible = ((row.yMax + yOffset) >= visibleRectYMin && (row.yMin + yOffset) <= visibleRectYMax);

                if (isVisible)
                {
                    row.layoutOptions = GUILayoutOptions.Height(heightToAllocate);
                    row.tmpRowHeight = 0;
                    if (isFirstVisible)
                    {
                        this.RowIndexFrom = i;
                        isFirstVisible = false;
                    }
                    this.RowIndexTo = i + 1;
                }
                else if ((row.yMin + yOffset) < this.innerVisibleRect.yMax)
                {
                    this.spaceBefore += heightToAllocate;
                }
                else
                {
                    this.spaceAfter += heightToAllocate;
                    row.avgRowHeight = this.avgCellHeight;
                }

                this.rows[i] = row;
                this.totalHeight = row.yMax;

                if (row.rowHeight > 0)
                {
                    visibleCellsCount++;
                    visibleCellsHeight += row.rowHeight;
                }
            }

            if (visibleCellsCount > 0 && visibleCellsHeight > 0)
            {
                this.avgCellHeight = visibleCellsHeight / visibleCellsCount;
            }
        }

        private struct TableRow
        {
            public int yMin;
            public int yMax;
            public int rowHeight;
            public int avgRowHeight;
            public int tmpRowHeight;
            public GUILayoutOption[] layoutOptions;
        }
    }
}

#endif