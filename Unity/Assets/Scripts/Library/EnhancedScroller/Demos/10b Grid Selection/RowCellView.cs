using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;
using EnhancedUI;
using System;

namespace EnhancedScrollerDemos.GridSelection
{
    /// <summary>
    /// This is the sub cell of the row cell
    /// </summary>
    public class RowCellView : MonoBehaviour
    {
        /// <summary>
        /// These are the UI elements that will be updated when the data changes
        /// </summary>
        public GameObject container;
        public Text text;
        public Image selectionPanel;

        /// <summary>
        /// These are the colors for the selection of the cells
        /// </summary>
        public Color selectedColor;
        public Color unSelectedColor;

        /// <summary>
        /// Public reference to the index of the data
        /// </summary>
        public int DataIndex { get; private set; }

        /// <summary>
        /// The handler to call when this cell's button traps a click event
        /// </summary>
        public SelectedDelegate selected;

        /// <summary>
        /// Reference to the underlying data driving this view
        /// </summary>
        private Data _data;

        /// <summary>
        /// This is called if the cell is destroyed. The EnhancedScroller will
        /// not call this since it uses recycling, but we include it in case 
        /// the user decides to destroy the cell anyway
        /// </summary>
        void OnDestroy()
        {
            if (_data != null)
            {
                // remove the handler from the data so 
                // that any changes to the data won't try
                // to call this destroyed view's function
                _data.selectedChanged -= SelectedChanged;
            }
        }

        /// <summary>
        /// This function just takes the Demo data and displays it
        /// </summary>
        /// <param name="data"></param>
        public void SetData(int dataIndex, Data data, SelectedDelegate selected)
        {
            // set the selected delegate
            this.selected = selected;

            // this cell was outside the range of the data, so we disable the container.
            // Note: We could have disable the cell gameobject instead of a child container,
            // but that can cause problems if you are trying to get components (disabled objects are ignored).
            container.SetActive(data != null);

            if (data != null)
            {
                // set the text if the cell is inside the data range
                text.text = data.someText;
            }

            // if there was previous data assigned to this cell view,
            // we need to remove the handler for the selection change
            if (_data != null)
            {
                _data.selectedChanged -= SelectedChanged;
            }

            // link the data to the cell view
            DataIndex = dataIndex;
            _data = data;

            if (data != null)
            {
                // set up a handler so that when the data changes
                // the cell view will update accordingly. We only
                // want a single handler for this cell view, so 
                // first we remove any previous handlers before
                // adding the new one
                _data.selectedChanged -= SelectedChanged;
                _data.selectedChanged += SelectedChanged;

                // update the selection state UI
                SelectedChanged(data.Selected);
            }
        }

        /// <summary>
        /// This function changes the UI state when the item is 
        /// selected or unselected.
        /// </summary>
        /// <param name="selected">The selection state of the cell</param>
        private void SelectedChanged(bool selected)
        {
            selectionPanel.color = (selected ? selectedColor : unSelectedColor);
        }

        /// <summary>
        /// This function is called by the cell's button click event
        /// </summary>
        public void OnSelected()
        {
            // if a handler exists for this cell, then
            // call it.
            if (selected != null) selected(this);
        }
    }
}