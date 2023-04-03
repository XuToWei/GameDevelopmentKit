using UnityEngine;
using System.Collections;

namespace EnhancedScrollerDemos.SelectionDemo
{
    /// <summary>
    /// This delegate handles any changes to the selection state of the data
    /// </summary>
    /// <param name="val">The state of the selection</param>
    public delegate void SelectedChangedDelegate(bool val);

    /// <summary>
    /// This class represents an inventory record
    /// </summary>
    public class InventoryData
    {
        /// <summary>
        /// The name of the inventory item
        /// </summary>
        public string itemName;

        /// <summary>
        /// The cost of the inventory item
        /// </summary>
        public int itemCost;

        /// <summary>
        /// The damage the item can do
        /// </summary>
        public int itemDamage;

        /// <summary>
        /// The armor the item provides
        /// </summary>
        public int itemDefense;

        /// <summary>
        /// The weight of the item
        /// </summary>
        public int itemWeight;

        /// <summary>
        /// This description of the inventory item
        /// </summary>
        public string itemDescription;

        /// <summary>
        /// The path to the resources folder for the sprite
        /// representing this inventory item
        /// </summary>
        public string spritePath;

        /// <summary>
        /// The delegate to call if the data's selection state
        /// has changed. This will update any views that are hooked
        /// to the data so that they show the proper selection state UI.
        /// </summary>
        public SelectedChangedDelegate selectedChanged;

        /// <summary>
        /// The selection state
        /// </summary>
        private bool _selected;
        public bool Selected
        {
            get { return _selected; }
            set
            {
                // if the value has changed
                if (_selected != value)
                {
                    // update the state and call the selection handler if it exists
                    _selected = value;
                    if (selectedChanged != null) selectedChanged(_selected);
                }
            }
        }
    }
}