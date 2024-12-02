using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using EnhancedUI.EnhancedScroller;
using EnhancedUI;

namespace EnhancedScrollerDemos.SnappingDemo
{
    /// <summary>
    /// This class controls one slot scroller. We could have shared the slot data between the 
    /// three slot controllers, but for demonstration purposes we gave each slot controller their 
    /// own set of data.
    /// </summary>
    public class SlotController : MonoBehaviour, IEnhancedScrollerDelegate
    {
        /// <summary>
        /// This list of slot cells
        /// </summary>
        private SmallList<SlotData> _data;

        /// <summary>
        /// The scroller that will display the slot cells
        /// </summary>
        public EnhancedScroller scroller;

        /// <summary>
        /// The slot cell view prefab to use in the scroller
        /// </summary>
        public EnhancedScrollerCellView slotCellViewPrefab;

        void Awake()
        {
            // create a new data list for the slots
            _data = new SmallList<SlotData>();
        }

        void Start()
        {
            // set this controller as the scroller's delegate
            scroller.Delegate = this;
        }

        public void Reload(Sprite[] sprites)
        {
            // reset the data list
            _data.Clear();

            // at the sprites from the demo script to this scroller's data cells
            foreach (var slotSprite in sprites)
            {
                _data.Add(new SlotData() { sprite = slotSprite });
            }

            // reload the scroller
            scroller.ReloadData();
        }

        /// <summary>
        /// This makes the scroller move without having an explicit touch event
        /// </summary>
        /// <param name="amount"></param>
        public void AddVelocity(float amount)
        {
            // set the scroller's linear velocity
            // (velocity in one direction)
            scroller.LinearVelocity = amount;
        }

        #region EnhancedScroller Callbacks

        /// <summary>
        /// This callback tells the scroller how many slot cells to expect
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
            return 150f;
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
            // get the cell view from the scroller, recycling if possible
            SlotCellView cellView = scroller.GetCellView(slotCellViewPrefab) as SlotCellView;

            // set the data for the cell
            cellView.SetData(_data[dataIndex]);

            // return the cell view to the scroller
            return cellView;
        }

        #endregion
    }
}