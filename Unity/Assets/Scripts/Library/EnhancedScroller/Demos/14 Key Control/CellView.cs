using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;

namespace EnhancedScrollerDemos.KeyControl
{
    /// <summary>
    /// This is the view of our cell which handles how the cell looks.
    /// </summary>
    public class CellView : EnhancedScrollerCellView
    {
        /// <summary>
        /// The underlying data source
        /// </summary>
        private Data _data;

        /// <summary>
        /// The background image to show the highlight if selected
        /// </summary>
        public Image backgroundImage;

        /// <summary>
        /// A reference to the UI Text element to display the cell data
        /// </summary>
        public Text someTextText;

        /// <summary>
        /// The color to show when the cell is selected
        /// </summary>
        public Color selectedColor;

        /// <summary>
        /// The color to show when the cell is not selected
        /// </summary>
        public Color unselectedColor;

        /// <summary>
        /// This function just takes the Demo data and displays it
        /// </summary>
        /// <param name="data"></param>
        public void SetData(Data data)
        {
            // set the underlying data source. This is used when
            // we need to refresh the cell view
            _data = data;

            // update the UI text with the cell data
            someTextText.text = _data.someText;

            // call the refresh method which just sets the selection highlight
            RefreshCellView();
        }

        /// <summary>
        /// Called when the selected cell index is changed in the controller
        /// </summary>
        public override void RefreshCellView()
        {
            // highlight the cell if necessary
            backgroundImage.color = _data.isSelected ? selectedColor : unselectedColor;
        }
    }
}
