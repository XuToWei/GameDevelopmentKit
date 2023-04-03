namespace EnhancedScrollerDemos.ExpandingCells
{
    /// <summary>
    /// Super simple data class to hold information for each cell.
    /// </summary>
    public class Data
    {
        public bool isExpanded;

        public string headerText;

        public string descriptionText;

        public float collapsedSize;

        public float expandedSize;

        public EnhancedUI.EnhancedScroller.Tween.TweenType tweenType;

        public float tweenTimeExpand;

        public float tweenTimeCollapse;

        /// <summary>
        /// Calculates the size of the cell based on its expansion state
        /// </summary>
        public float Size
        {
            get
            {
                return isExpanded ? expandedSize : collapsedSize;
            }
        }

        /// <summary>
        /// Calculates the difference in the expanded and collapsed sizes
        /// </summary>
        public float SizeDifference
        {
            get
            {
                return expandedSize - collapsedSize;
            }
        }
    }
}
