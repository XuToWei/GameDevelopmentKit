using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;
using EnhancedUI;
using System;

namespace EnhancedScrollerDemos.GridSimulation
{
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
        public void SetData(ref SmallList<Data> data, int startingIndex)
        {
            // loop through the sub cells to display their data (or disable them if they are outside the bounds of the data)
            for (var i = 0; i < rowCellViews.Length; i++)
            {
                // if the sub cell is outside the bounds of the data, we pass null to the sub cell
                rowCellViews[i].SetData(startingIndex + i < data.Count ? data[startingIndex + i] : null);
            }
        }
    }
}