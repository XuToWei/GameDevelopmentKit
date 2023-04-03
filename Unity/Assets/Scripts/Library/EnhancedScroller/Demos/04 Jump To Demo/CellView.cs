using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;

namespace EnhancedScrollerDemos.JumpToDemo
{
    public class CellView : EnhancedScrollerCellView
    {
        public Text cellText;

        public void SetData(Data data)
        {
            cellText.text = data.cellText;
        }
    }
}