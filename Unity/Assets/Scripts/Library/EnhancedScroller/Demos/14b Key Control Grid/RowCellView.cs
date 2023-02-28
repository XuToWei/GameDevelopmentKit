using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;
using EnhancedUI;
using System;

namespace EnhancedScrollerDemos.KeyControlGrid
{
    /// <summary>
    /// This is the sub cell of the row cell
    /// </summary>
    public class RowCellView : MonoBehaviour
    {
        /// <summary>
        /// The underlying data source
        /// </summary>
        private Data _data;

        public GameObject container;

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

            // this cell was outside the range of the data, so we disable the container.
            // Note: We could have disable the cell gameobject instead of a child container,
            // but that can cause problems if you are trying to get components (disabled objects are ignored).
            container.SetActive(data != null);

            if (_data != null)
            {
                // update the UI text with the cell data
                someTextText.text = _data.someText;

                // call the refresh method which just sets the selection highlight
                RefreshCellView();
            }
        }

        /// <summary>
        /// Called when the selected cell index is changed in the controller
        /// </summary>
        public void RefreshCellView()
        {
            if (_data != null)
            {
                // highlight the cell if necessary
                backgroundImage.color = _data.isSelected ? selectedColor : unselectedColor;
            }
        }
    }
}
