using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;

namespace EnhancedScrollerDemos.NestedScrollers
{
    /// <summary>
    /// This is the view of our cell which handles how the cell looks.
    /// </summary>
    public class DetailCellView : EnhancedScrollerCellView
    {
        /// <summary>
        /// A reference to the UI Text element to display the cell data
        /// </summary>
        public Text someTextText;

        /// <summary>
        /// This function just takes the Demo data and displays it
        /// </summary>
        /// <param name="data"></param>
        public void SetData(DetailData data)
        {
            // update the UI text with the cell data
            someTextText.text = data.someText;
        }
    }
}