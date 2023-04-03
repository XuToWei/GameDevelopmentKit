using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;

namespace EnhancedScrollerDemos.SelectionDemo
{
    /// <summary>
    /// This delegate handles the UI's button click
    /// </summary>
    /// <param name="cellView">The cell view that had the button click</param>
    public delegate void SelectedDelegate(EnhancedScrollerCellView cellView);

    /// <summary>
    /// This class handles the presentation of the inventory cell view. Both the 
    /// horizontal and vertical cell views share the same view class. The difference
    /// between them is layout and sprite for this example.
    /// </summary>
    public class InventoryCellView : EnhancedScrollerCellView
    {
        /// <summary>
        /// Reference to the underlying data driving this view
        /// </summary>
        private InventoryData _data;

        /// <summary>
        /// These are the UI elements that will be updated when the data changes
        /// </summary>
        public Image selectionPanel;
        public Text itemNameText;
        public Text itemCostText;
        public Text itemDamageText;
        public Text itemDefenseText;
        public Text itemWeightText;
        public Text itemDescriptionText;
        public Image image;

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
        /// This function sets up the data for the cell view
        /// </summary>
        /// <param name="dataIndex">The index of the data</param>
        /// <param name="data">The data to use</param>
        /// <param name="isVertical">Whether this view is vertical or horizontal (which will determine the sprite to use)</param>
        public void SetData(int dataIndex, InventoryData data, bool isVertical)
        {
            // if there was previous data assigned to this cell view,
            // we need to remove the handler for the selection change
            if (_data != null)
            {
                _data.selectedChanged -= SelectedChanged;
            }

            // link the data to the cell view
            DataIndex = dataIndex;
            _data = data;

            // update the cell view's UI
            itemNameText.text = data.itemName;

            if (itemCostText != null) itemCostText.text = (data.itemCost > 0 ? data.itemCost.ToString() : "-");
            if (itemDamageText != null) itemDamageText.text = (data.itemDamage > 0 ? data.itemDamage.ToString() : "-");
            if (itemDefenseText != null) itemDefenseText.text = (data.itemDefense > 0 ? data.itemDefense.ToString() : "-");
            if (itemWeightText != null) itemWeightText.text = (data.itemWeight > 0 ? data.itemWeight.ToString() : "-");

            // the description is only shown on the vertical cell view
            if (isVertical)
                itemDescriptionText.text = data.itemDescription;

            // set up the sprite based on the sprite path and whether the
            // view is horizontal or vertical
            image.sprite = Resources.Load<Sprite>(data.spritePath + (isVertical ? "_v" : "_h"));

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