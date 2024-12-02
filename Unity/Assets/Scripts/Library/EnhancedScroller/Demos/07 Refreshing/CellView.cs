using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;

namespace EnhancedScrollerDemos.RefreshDemo
{
    /// <summary>
    /// This is the view of our cell which handles how the cell looks.
    /// </summary>
    public class CellView : EnhancedScrollerCellView
    {
        /// <summary>
        /// This is a reference to the cell's underlying data.
        /// We will store it in the SetData method, and use it
        /// in the RefreshCellView method.
        /// </summary>
        private Data _data;

        /// <summary>
        /// A reference to the UI Text element to display the cell data
        /// </summary>
        public Text someTextText;

        public RectTransform RectTransform
        {
            get
            {
                var rt = gameObject.GetComponent<RectTransform>();
                return rt;
            }
        }

        /// <summary>
        /// This function just takes the Demo data and displays it
        /// </summary>
        /// <param name="data"></param>
        public void SetData(Data data)
        {
            // store the data so that it can be used when refreshing
            _data = data;

            // update the cell's UI
            RefreshCellView();
        }

        public override void RefreshCellView()
        {
            // update the UI text with the cell data
            someTextText.text = _data.someText;
        }
    }
}