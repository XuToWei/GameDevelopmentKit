using UnityEngine;
using System.Collections;
using EnhancedUI.EnhancedScroller;

namespace EnhancedScrollerDemos.MultipleCellTypesDemo
{
    /// <summary>
    /// This is the base class for the different cell types. We use a base class
    /// to make calling SetData easier in the demo script.
    /// </summary>
    public class CellView : EnhancedScrollerCellView
    {
        /// <summary>
        /// Internal reference to our base data class
        /// </summary>
        protected Data _data;

        /// <summary>
        /// Sets the data for the cell view. Note that the base data class is passed in,
        /// but through polymorphism we will actually pass the inherited data classes
        /// </summary>
        /// <param name="data"></param>
        public virtual void SetData(Data data)
        {
            _data = data;
        }
    }
}