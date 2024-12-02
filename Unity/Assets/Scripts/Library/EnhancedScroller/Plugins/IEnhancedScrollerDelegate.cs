using UnityEngine;
using System.Collections;

namespace EnhancedUI.EnhancedScroller
{
    /// <summary>
    /// All scripts that handle the scroller's callbacks should inherit from this interface
    /// </summary>
    public interface IEnhancedScrollerDelegate
    {
        /// <summary>
        /// Gets the number of cells in a list of data
        /// </summary>
        /// <param name="scroller"></param>
        /// <returns></returns>
        int GetNumberOfCells(EnhancedScroller scroller);

        /// <summary>
        /// Gets the size of a cell view given the index of the data set.
        /// This allows you to have different sized cells
        /// </summary>
        /// <param name="scroller"></param>
        /// <param name="dataIndex"></param>
        /// <returns></returns>
        float GetCellViewSize(EnhancedScroller scroller, int dataIndex);

        /// <summary>
        /// Gets the cell view that should be used for the data index. Your implementation
        /// of this function should request a new cell from the scroller so that it can
        /// properly recycle old cells.
        /// </summary>
        /// <param name="scroller"></param>
        /// <param name="dataIndex"></param>
        /// <param name="cellIndex"></param>
        /// <returns></returns>
        EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex);
    }
}