namespace EnhancedScrollerDemos.ViewDrivenCellSizes
{
    /// <summary>
    /// Super simple data class to hold information for each cell.
    /// </summary>
    public class Data
    {
        public string someText;

        /// <summary>
        /// We will store the cell size in the model so that the cell view can update it
        /// </summary>
        public float cellSize;
    }
}