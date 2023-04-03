using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;

namespace EnhancedScrollerDemos.CellEvents
{
    /// <summary>
    /// These delegates will publish events when a button is clicked
    /// </summary>
    /// <param name="value"></param>
    public delegate void CellButtonTextClickedDelegate(string value);
    public delegate void CellButtonIntegerClickedDelegate(int value);

    public class CellView : EnhancedScrollerCellView
    {
        private Data _data;

        public Text someTextText;

        /// <summary>
        ///  These delegates will fire whenever one of the events occurs
        /// </summary>
        public CellButtonTextClickedDelegate cellButtonTextClicked;
        public CellButtonIntegerClickedDelegate cellButtonFixedIntegerClicked;
        public CellButtonIntegerClickedDelegate cellButtonDataIntegerClicked;

        public void SetData(Data data)
        {
            _data = data;
            someTextText.text = (_data.hour == 0 ? "Midnight" : string.Format("{0} 'o clock", _data.hour.ToString()));
        }

        // Handle the click of the fixed text button (this is hooked up in the Unity editor in the button's click event)
        public void CellButtonText_OnClick(string value)
        {
            // fire event if anyone has subscribed to it
            if (cellButtonTextClicked != null) cellButtonTextClicked(value);
        }

        // Handle the click of the fixed integer button (this is hooked up in the Unity editor in the button's click event)
        public void CellButtonFixedInteger_OnClick(int value)
        {
            // fire event if anyone has subscribed to it
            if (cellButtonFixedIntegerClicked != null) cellButtonFixedIntegerClicked(value);
        }

        // Handle the click of the data integer button (this is hooked up in the Unity editor in the button's click event)
        public void CellButtonDataInteger_OnClick()
        {
            // fire event if anyone has subscribed to it
            if (cellButtonDataIntegerClicked != null) cellButtonDataIntegerClicked(_data.hour);
        }
    }
}