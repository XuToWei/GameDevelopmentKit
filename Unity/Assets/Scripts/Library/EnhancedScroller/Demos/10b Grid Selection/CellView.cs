using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;
using EnhancedUI;
using System;

namespace EnhancedScrollerDemos.GridSelection
{
    /// <summary>
    /// This delegate handles the UI's button click
    /// </summary>
    /// <param name="cellView">The cell view that had the button click</param>
    public delegate void SelectedDelegate(RowCellView rowCellView);

    /// <summary>
    /// This is the view of our cell which handles how the cell looks.
    /// It stores references to sub cells
    /// </summary>
    public class CellView : EnhancedScrollerCellView
    {
        public RowCellView[] rowCellViews;

        /// <summary>
        /// This function just takes the Demo data and displays it
        /// </summary>
        /// <param name="data"></param>
        public void SetData(ref SmallList<Data> data, int startingIndex, SelectedDelegate selected)
        {
            // loop through the sub cells to display their data (or disable them if they are outside the bounds of the data)
            for (var i = 0; i < rowCellViews.Length; i++)
            {
                var dataIndex = startingIndex + i;

                // if the sub cell is outside the bounds of the data, we pass null to the sub cell
                rowCellViews[i].SetData(dataIndex, dataIndex < data.Count ? data[dataIndex] : null, selected);
            }
        }
    }
}