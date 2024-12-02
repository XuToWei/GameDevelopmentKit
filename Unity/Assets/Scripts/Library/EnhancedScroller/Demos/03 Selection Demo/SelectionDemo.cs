using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using EnhancedUI.EnhancedScroller;
using EnhancedUI;

namespace EnhancedScrollerDemos.SelectionDemo
{
    /// <summary>
    /// This class sets up the data for the inventory and handles the EnhancedScrollers'
    /// callbacks. It also allows changes to the scrollers through some UI interfaces.
    /// </summary>
    public class SelectionDemo : MonoBehaviour, IEnhancedScrollerDelegate
    {
        /// <summary>
        /// The list of inventory data
        /// </summary>
        private SmallList<InventoryData> _data;

        /// <summary>
        /// The vertical inventory scroller
        /// </summary>
        public EnhancedScroller vScroller;

        /// <summary>
        /// The horizontal inventory scroller
        /// </summary>
        public EnhancedScroller hScroller;

        /// <summary>
        /// The cell view prefab for the vertical scroller
        /// </summary>
        public EnhancedScrollerCellView vCellViewPrefab;
        
        /// <summary>
        /// The cell view prefab for the horizontal scroller
        /// </summary>
        public EnhancedScrollerCellView hCellViewPrefab;

        /// <summary>
        /// The image that shows which item is selected
        /// </summary>
        public Image selectedImage;
        public Text selectedImageText;

        /// <summary>
        /// The base path to the resources folder where the inventory
        /// item sprites are located
        /// </summary>
        public string resourcePath;

        void Awake()
        {
            // set the application frame rate.
            // this improves smoothness on some devices
            Application.targetFrameRate = 60;

            // turn on the mask and loop functionality for each scroller based
            // on the UI settings of this controller

            var maskToggle = GameObject.Find("Mask Toggle").GetComponent<Toggle>();
            MaskToggle_OnValueChanged(maskToggle.isOn);

            var loopToggle = GameObject.Find("Loop Toggle").GetComponent<Toggle>();
            LoopToggle_OnValueChanged(loopToggle.isOn);

            CellViewSelected(null);
        }

        void Start()
        {
            // set up the delegates for each scroller

            vScroller.Delegate = this;
            hScroller.Delegate = this;

            // reload the data
            Reload();
        }

        /// <summary>
        /// This function sets up our inventory data and tells the scrollers to reload
        /// </summary>
        private void Reload()
        {
            // if the data existed previously, loop through
            // and remove the selection change handlers before
            // clearing out the data.
            if (_data != null)
            {
                for (var i = 0; i < _data.Count; i++)
                {
                    _data[i].selectedChanged = null;
                }
            }

            // set up a new inventory list
            _data = new SmallList<InventoryData>();

            // add inventory items to the list
            _data.Add(new InventoryData() { itemName = "Sword", itemCost = 123, itemDamage = 50, itemDefense = 0, itemWeight = 10, spritePath = resourcePath + "/sword", itemDescription = "Broadsword with a double-edged blade" });
            _data.Add(new InventoryData() { itemName = "Shield", itemCost = 80, itemDamage = 0, itemDefense = 60, itemWeight = 50, spritePath = resourcePath + "/shield", itemDescription = "Steel shield to deflect your enemy's blows" });
            _data.Add(new InventoryData() { itemName = "Amulet", itemCost = 260, itemDamage = 0, itemDefense = 0, itemWeight = 1, spritePath = resourcePath + "/amulet", itemDescription = "Magic amulet restores your health points gradually over time" });
            _data.Add(new InventoryData() { itemName = "Helmet", itemCost = 50, itemDamage = 0, itemDefense = 20, itemWeight = 20, spritePath = resourcePath + "/helmet", itemDescription = "Standard helm will decrease your vulnerability" });
            _data.Add(new InventoryData() { itemName = "Boots", itemCost = 40, itemDamage = 0, itemDefense = 10, itemWeight = 5, spritePath = resourcePath + "/boots", itemDescription = "Boots of speed will double your movement points" });
            _data.Add(new InventoryData() { itemName = "Bracers", itemCost = 30, itemDamage = 0, itemDefense = 20, itemWeight = 10, spritePath = resourcePath + "/bracers", itemDescription = "Bracers will upgrade your overall armor" });
            _data.Add(new InventoryData() { itemName = "Crossbow", itemCost = 100, itemDamage = 40, itemDefense = 0, itemWeight = 30, spritePath = resourcePath + "/crossbow", itemDescription = "Crossbow can attack from long range" });
            _data.Add(new InventoryData() { itemName = "Fire Ring", itemCost = 300, itemDamage = 100, itemDefense = 0, itemWeight = 1, spritePath = resourcePath + "/fireRing", itemDescription = "Fire ring gives you the magical ability to cast fireball spells" });
            _data.Add(new InventoryData() { itemName = "Knapsack", itemCost = 22, itemDamage = 0, itemDefense = 0, itemWeight = 0, spritePath = resourcePath + "/knapsack", itemDescription = "Knapsack will increase your carrying capacity by twofold" });

            // tell the scrollers to reload
            vScroller.ReloadData();
            hScroller.ReloadData();
        }

        /// <summary>
        /// This function handles the cell view's button click event
        /// </summary>
        /// <param name="cellView">The cell view that had the button clicked</param>
        private void CellViewSelected(EnhancedScrollerCellView cellView)
        {
            if (cellView == null)
            {
                // nothing was selected
                selectedImage.gameObject.SetActive(false);
                selectedImageText.text = "None";
            }
            else
            {
                // get the selected data index of the cell view
                var selectedDataIndex = (cellView as InventoryCellView).DataIndex;

                // loop through each item in the data list and turn
                // on or off the selection state. This is done so that
                // any previous selection states are removed and new
                // ones are added.
                for (var i = 0; i < _data.Count; i++)
                {
                    _data[i].Selected = (selectedDataIndex == i);
                }

                selectedImage.gameObject.SetActive(true);
                selectedImage.sprite = Resources.Load<Sprite>(_data[selectedDataIndex].spritePath + "_v");

                selectedImageText.text = _data[selectedDataIndex].itemName;
            }
        }

        #region Controller UI Handlers

        /// <summary>
        /// This handles the toggle for the masks
        /// </summary>
        /// <param name="val">Is the mask on?</param>
        public void MaskToggle_OnValueChanged(bool val)
        {
            // set the mask component of each scroller
            vScroller.GetComponent<Mask>().enabled = val;
            hScroller.GetComponent<Mask>().enabled = val;
        }

        /// <summary>
        /// This handles the toggle fof the looping
        /// </summary>
        /// <param name="val">Is the looping on?</param>
        public void LoopToggle_OnValueChanged(bool val)
        {
            // set the loop property of each scroller
            vScroller.Loop = val;
            hScroller.Loop = val;
        }

        #endregion

        #region EnhancedScroller Callbacks

        /// <summary>
        /// This callback tells the scroller how many inventory items to expect
        /// </summary>
        /// <param name="scroller">The scroller requesting the number of cells</param>
        /// <returns>The number of cells</returns>
        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            return _data.Count;
        }

        /// <summary>
        /// This callback tells the scroller what size each cell is.
        /// </summary>
        /// <param name="scroller">The scroller requesting the cell size</param>
        /// <param name="dataIndex">The index of the data list</param>
        /// <returns>The size of the cell (Height for vertical scrollers, Width for Horizontal scrollers)</returns>
        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            if (scroller == vScroller)
            {
                // return a static height for all vertical scroller cells
                return 320f;
            }
            else
            {
                // return a static width for all horizontal scroller cells
                return 150f;
            }
        }

        /// <summary>
        /// This callback gets the cell to be displayed by the scroller
        /// </summary>
        /// <param name="scroller">The scroller requesting the cell</param>
        /// <param name="dataIndex">The index of the data list</param>
        /// <param name="cellIndex">The cell index (This will be different from dataindex if looping is involved)</param>
        /// <returns>The cell to display</returns>
        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            // first get a cell from the scroller. The scroller will recycle if it can.
            // We want a vertical cell prefab for the vertical scroller and a horizontal
            // prefab for the horizontal scroller.
            InventoryCellView cellView = scroller.GetCellView(scroller == vScroller ? vCellViewPrefab : hCellViewPrefab) as InventoryCellView;

            // set the name of the cell. This just makes it easier to see in our
            // hierarchy what the cell is
            cellView.name = (scroller == vScroller ? "Vertical" : "Horizontal") + " " + _data[dataIndex].itemName;

            // set the selected callback to the CellViewSelected function of this controller. 
            // this will be fired when the cell's button is clicked
            cellView.selected = CellViewSelected;

            // set the data for the cell
            cellView.SetData(dataIndex, _data[dataIndex], (scroller == vScroller));

            // return the cell view to the scroller
            return cellView;
        }

        #endregion
    }
}