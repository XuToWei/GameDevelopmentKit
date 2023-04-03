namespace EnhancedScrollerDemos.Chat
{
    /// <summary>
    /// Super simple data class to hold information for each cell.
    /// </summary>
    public class Data
    {
        /// <summary>
        /// The potential types of a cell
        /// </summary>
        public enum CellType
        {
            Spacer,
            MyText,
            OtherText
        }

        /// <summary>
        /// The type of the cell
        /// </summary>
        public CellType cellType;

        /// <summary>
        /// The text to display (only used on chat cells, not the spacer)
        /// </summary>
        public string someText;

        /// <summary>
        /// We will store the cell size in the model so that the cell view can update it.
        /// Only used on chat cells, not the spacer. Spacer always pulls the size of the scroll rect in the controller.
        /// </summary>
        public float cellSize;
    }
}
