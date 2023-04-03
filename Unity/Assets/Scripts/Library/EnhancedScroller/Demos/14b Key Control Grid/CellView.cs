using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;
using EnhancedUI;
using System;

namespace EnhancedScrollerDemos.KeyControlGrid
{
    /// <summary>
    /// This is the view of our cell which handles how the cell looks.
    /// </summary>
    public class CellView : EnhancedScrollerCellView
    {
        public RowCellView[] rowCellViews;

        /// <summary>
        /// This function just takes the Demo data and displays it
        /// </summary>
        /// <param name="data"></param>
        public void SetData(ref SmallList<Data> data, int startingIndex)
        {
            // loop through the sub cells to display their data (or disable them if they are outside the bounds of the data)
            for (var i = 0; i < rowCellViews.Length; i++)
            {
                var dataIndex = startingIndex + i;

                // if the sub cell is outside the bounds of the data, we pass null to the sub cell
                rowCellViews[i].SetData(dataIndex < data.Count ? data[dataIndex] : null);
            }
        }

        /// <summary>
        /// Called when the selected cell index is changed in the controller
        /// </summary>
        public override void RefreshCellView()
        {
            for (var i = 0; i < rowCellViews.Length; i++)
            {
                rowCellViews[i].RefreshCellView();
            }
        }
    }
}
