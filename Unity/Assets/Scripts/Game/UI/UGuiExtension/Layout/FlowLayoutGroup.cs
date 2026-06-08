using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    /// <summary>
    /// Arranges child RectTransforms from left to right and wraps them to new rows when the available width is exceeded.
    /// </summary>
    [AddComponentMenu("Layout/Flow Layout Group", 153)]
    public class FlowLayoutGroup : LayoutGroup
    {
        private readonly List<RowInfo> m_Rows = new List<RowInfo>();

        [SerializeField]
        private bool m_ChildForceExpandHeight;
        [SerializeField]
        private bool m_ChildForceExpandWidth;
        [SerializeField]
        private float m_Spacing = 0f;

        private bool isCenterAlign
        {
            get
            {
                return childAlignment == TextAnchor.LowerCenter ||
                       childAlignment == TextAnchor.MiddleCenter ||
                       childAlignment == TextAnchor.UpperCenter;
            }
        }

        private bool isRightAlign
        {
            get
            {
                return childAlignment == TextAnchor.LowerRight ||
                       childAlignment == TextAnchor.MiddleRight ||
                       childAlignment == TextAnchor.UpperRight;
            }
        }

        private bool isMiddleAlign
        {
            get
            {
                return childAlignment == TextAnchor.MiddleLeft ||
                       childAlignment == TextAnchor.MiddleCenter ||
                       childAlignment == TextAnchor.MiddleRight;
            }
        }

        private bool isLowerAlign
        {
            get
            {
                return childAlignment == TextAnchor.LowerLeft ||
                       childAlignment == TextAnchor.LowerCenter ||
                       childAlignment == TextAnchor.LowerRight;
            }
        }

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

            float minWidth = GetGreatestMinimumChildWidth() + padding.horizontal;
            SetLayoutInputForAxis(minWidth, -1f, -1f, 0);
        }

        public override void CalculateLayoutInputVertical()
        {
            SetLayout(rectTransform.rect.width, 1, true);
        }

        public override void SetLayoutHorizontal()
        {
            SetLayout(rectTransform.rect.width, 0, false);
        }

        public override void SetLayoutVertical()
        {
            SetLayout(rectTransform.rect.width, 1, false);
        }

        /// <summary>
        /// Calculates layout input or applies child positions for the requested axis.
        /// </summary>
        /// <param name="width">Layout width including horizontal padding.</param>
        /// <param name="axis">0 for horizontal axis, 1 for vertical axis.</param>
        /// <param name="layoutInput">True to calculate input size, false to position children.</param>
        public float SetLayout(float width, int axis, bool layoutInput)
        {
            float totalHeight = CalculateRows(width);

            if (layoutInput)
            {
                if (axis == 1)
                {
                    SetLayoutInputForAxis(totalHeight, totalHeight, -1f, axis);
                }

                return totalHeight;
            }

            float workingWidth = GetWorkingWidth(width);
            float yOffset = GetStartYOffset(rectTransform.rect.height, totalHeight);

            for (int i = 0; i < m_Rows.Count; i++)
            {
                RowInfo row = m_Rows[i];
                LayoutRow(row, workingWidth, padding.left, yOffset, axis);
                yOffset += row.Height + m_Spacing;
            }

            return totalHeight;
        }

        public float GetGreatestMinimumChildWidth()
        {
            float maxWidth = 0f;

            for (int i = 0; i < rectChildren.Count; i++)
            {
                maxWidth = Mathf.Max(maxWidth, LayoutUtility.GetMinWidth(rectChildren[i]));
            }

            return maxWidth;
        }

        private float CalculateRows(float width)
        {
            m_Rows.Clear();

            float workingWidth = GetWorkingWidth(width);
            float contentHeight = 0f;
            float rowWidth = 0f;
            float rowHeight = 0f;
            int rowStartIndex = 0;
            int rowChildCount = 0;

            for (int i = 0; i < rectChildren.Count; i++)
            {
                RectTransform child = rectChildren[i];
                float childWidth = GetPreferredChildWidth(child, workingWidth);
                float childHeight = LayoutUtility.GetPreferredSize(child, 1);
                float nextRowWidth = rowChildCount == 0 ? childWidth : rowWidth + m_Spacing + childWidth;

                if (rowChildCount > 0 && nextRowWidth > workingWidth)
                {
                    AddRow(rowStartIndex, rowChildCount, rowWidth, rowHeight, ref contentHeight);

                    rowStartIndex = i;
                    rowChildCount = 0;
                    rowWidth = 0f;
                    rowHeight = 0f;
                    nextRowWidth = childWidth;
                }

                rowWidth = nextRowWidth;
                rowHeight = Mathf.Max(rowHeight, childHeight);
                rowChildCount++;
            }

            if (rowChildCount > 0)
            {
                AddRow(rowStartIndex, rowChildCount, rowWidth, rowHeight, ref contentHeight);
            }

            return contentHeight + padding.vertical;
        }

        private void AddRow(int startIndex, int childCount, float rowWidth, float rowHeight, ref float contentHeight)
        {
            if (m_Rows.Count > 0)
            {
                contentHeight += m_Spacing;
            }

            m_Rows.Add(new RowInfo(startIndex, childCount, rowWidth, rowHeight));
            contentHeight += rowHeight;
        }

        private void LayoutRow(RowInfo row, float maxWidth, float xOffset, float yOffset, int axis)
        {
            float extraWidthPerFlexibleUnit = GetExtraWidthPerFlexibleUnit(row, maxWidth);
            float xPosition = xOffset;

            if (extraWidthPerFlexibleUnit <= 0f)
            {
                if (isCenterAlign)
                {
                    xPosition += (maxWidth - row.Width) * 0.5f;
                }
                else if (isRightAlign)
                {
                    xPosition += maxWidth - row.Width;
                }
            }

            // 宽度扩展按 LayoutElement.flexibleWidth 分配；ChildForceExpandWidth 会让所有子项至少占 1 份。
            for (int i = 0; i < row.ChildCount; i++)
            {
                RectTransform child = rectChildren[row.StartIndex + i];
                float childWidth = GetPreferredChildWidth(child, maxWidth);
                float childHeight = LayoutUtility.GetPreferredSize(child, 1);
                float childFlexibleWidth = GetChildFlexibleWidth(child);

                if (extraWidthPerFlexibleUnit > 0f && childFlexibleWidth > 0f)
                {
                    childWidth += childFlexibleWidth * extraWidthPerFlexibleUnit;
                }

                if (m_ChildForceExpandHeight)
                {
                    childHeight = row.Height;
                }

                float childYOffset = yOffset;

                if (isMiddleAlign)
                {
                    childYOffset += (row.Height - childHeight) * 0.5f;
                }
                else if (isLowerAlign)
                {
                    childYOffset += row.Height - childHeight;
                }

                if (axis == 0)
                {
                    SetChildAlongAxis(child, 0, xPosition, childWidth);
                }
                else
                {
                    SetChildAlongAxis(child, 1, childYOffset, childHeight);
                }

                xPosition += childWidth + m_Spacing;
            }
        }

        private float GetExtraWidthPerFlexibleUnit(RowInfo row, float maxWidth)
        {
            float remainingWidth = Mathf.Max(0f, maxWidth - row.Width);

            if (remainingWidth <= 0f)
            {
                return 0f;
            }

            float totalFlexibleWidth = 0f;

            for (int i = 0; i < row.ChildCount; i++)
            {
                RectTransform child = rectChildren[row.StartIndex + i];
                totalFlexibleWidth += GetChildFlexibleWidth(child);
            }

            if (totalFlexibleWidth <= 0f)
            {
                return 0f;
            }

            return remainingWidth / totalFlexibleWidth;
        }

        private float GetChildFlexibleWidth(RectTransform child)
        {
            float flexibleWidth = LayoutUtility.GetFlexibleWidth(child);

            if (m_ChildForceExpandWidth)
            {
                flexibleWidth = Mathf.Max(1f, flexibleWidth);
            }

            return Mathf.Max(0f, flexibleWidth);
        }

        private float GetPreferredChildWidth(RectTransform child, float maxWidth)
        {
            return Mathf.Min(LayoutUtility.GetPreferredSize(child, 0), maxWidth);
        }

        private float GetWorkingWidth(float width)
        {
            return Mathf.Max(0f, width - padding.horizontal);
        }

        private float GetStartYOffset(float groupHeight, float totalHeight)
        {
            float contentHeight = Mathf.Max(0f, totalHeight - padding.vertical);

            if (isLowerAlign)
            {
                return groupHeight - padding.bottom - contentHeight;
            }

            if (isMiddleAlign)
            {
                return padding.top + (groupHeight - padding.vertical - contentHeight) * 0.5f;
            }

            return padding.top;
        }

        private readonly struct RowInfo
        {
            public RowInfo(int startIndex, int childCount, float width, float height)
            {
                StartIndex = startIndex;
                ChildCount = childCount;
                Width = width;
                Height = height;
            }

            public int StartIndex { get; }

            public int ChildCount { get; }

            public float Width { get; }

            public float Height { get; }
        }
    }
}
