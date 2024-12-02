using UnityEngine;
using System.Collections;
using EnhancedUI;
using EnhancedUI.EnhancedScroller;

namespace EnhancedScrollerDemos.KeyControlGrid
{
    /// <summary>
    /// This example shows how you can use the up and down arrow keys to control the selected cell of the list.
    /// If the selection goes beyond the bounds of the list, the list is automatically scrolled.
    /// </summary>
	public class Controller : MonoBehaviour, IEnhancedScrollerDelegate
    {

        /// <summary>
        /// Internal representation of our data. Note that the scroller will never see
        /// this, so it separates the data from the layout using MVC principles.
        /// </summary>
        private SmallList<Data> _data;


        /// <summary>
        /// The selected index of the list
        /// </summary>
        private int _selectedIndex = 0;

        /// <summary>
        /// This is our scroller we will be a delegate for
        /// </summary>
        public EnhancedScroller scroller;


        public int numberOfCellsPerRow = 3;

        /// <summary>
        /// This will be the prefab of each cell in our scroller. Note that you can use more
        /// than one kind of cell, but this example just has the one type.
        /// </summary>
        public EnhancedScrollerCellView cellViewPrefab;

        /// <summary>
        /// Be sure to set up your references to the scroller after the Awake function. The
        /// scroller does some internal configuration in its own Awake function. If you need to
        /// do this in the Awake function, you can set up the script order through the Unity editor.
        /// In this case, be sure to set the EnhancedScroller's script before your delegate.
        ///
        /// In this example, we are calling our initializations in the delegate's Start function,
        /// but it could have been done later, perhaps in the Update function.
        /// </summary>
        void Start()
        {
            // set the application frame rate.
            // this improves smoothness on some devices
            Application.targetFrameRate = 60;

            // tell the scroller that this script will be its delegate
            scroller.Delegate = this;

            // load in a large set of data
            LoadData();
        }

        void Update()
        {
            bool selectionChanged = false;

            if (Input.GetKeyDown(KeyCode.UpArrow) && _selectedIndex >= numberOfCellsPerRow)
            {
                // the up arrow was used, so we move the selected index up
                _selectedIndex = Mathf.Clamp(_selectedIndex - numberOfCellsPerRow, 0, _data.Count - 1);
                selectionChanged = true;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) && _selectedIndex < _data.Count - numberOfCellsPerRow)
            {
                // the down arrow was used, so we move the selected index down
                _selectedIndex = Mathf.Clamp(_selectedIndex + numberOfCellsPerRow, 0, _data.Count - 1);
                selectionChanged = true;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                // the down arrow was used, so we move the selected index down
                _selectedIndex = Mathf.Clamp(_selectedIndex - 1, 0, _data.Count - 1);
                selectionChanged = true;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                // the down arrow was used, so we move the selected index down
                _selectedIndex = Mathf.Clamp(_selectedIndex + 1, 0, _data.Count - 1);
                selectionChanged = true;
            }

            if (selectionChanged)
            {
                // the selected index was changed, so we update the underlying data
                for (var i = 0; i < _data.Count; i++)
                {
                    _data[i].isSelected = i == _selectedIndex;
                }

                // refresh the visible cell views to update the selection highlight
                scroller.RefreshActiveCellViews();

                var rowIndex = (_selectedIndex / numberOfCellsPerRow);

                if (rowIndex >= scroller.EndCellViewIndex)
                {
                    // the selected index is at the bottom or beyond, so we need to scroll down.
                    // note that we set the scroll offset to 1 (the end of the scroller)
                    // and also set the cell offset to 1 so that the entire last cell is visible.
                    scroller.JumpToDataIndex(rowIndex, 1.0f, 1.0f);
                }
                else if (rowIndex <= scroller.StartCellViewIndex)
                {
                    // the selected index is at the top or beyond, so we need to scroll up.
                    // note that we set the scroll offset to 0 (the start of the scroller)
                    // and also set the cell offset to 0 so that the entire first cell is visible.
                    scroller.JumpToDataIndex(rowIndex, 0.0f, 0.0f);
                }
            }
        }

        /// <summary>
        /// Populates the data with a lot of records
        /// </summary>
        private void LoadData()
        {
            // set up some simple data
            _data = new SmallList<Data>();
            for (var i = 0; i < 31; i++)
            {
                _data.Add(new Data()
                {
                    someText = i.ToString(),
                    isSelected = i == _selectedIndex
                });
            }

            // tell the scroller to reload now that we have the data
            scroller.ReloadData();
        }

        #region EnhancedScroller Handlers

        /// <summary>
        /// This tells the scroller the number of cells that should have room allocated.
        /// For this example, the count is the number of data elements divided by the number of cells per row (rounded up using Mathf.CeilToInt)
        /// </summary>
        /// <param name="scroller">The scroller that is requesting the data size</param>
        /// <returns>The number of cells</returns>
        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            return Mathf.CeilToInt((float)_data.Count / (float)numberOfCellsPerRow);
        }

        /// <summary>
        /// This tells the scroller what the size of a given cell will be. Cells can be any size and do not have
        /// to be uniform. For vertical scrollers the cell size will be the height. For horizontal scrollers the
        /// cell size will be the width.
        /// </summary>
        /// <param name="scroller">The scroller requesting the cell size</param>
        /// <param name="dataIndex">The index of the data that the scroller is requesting</param>
        /// <returns>The size of the cell</returns>
        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            return 100f;
        }

        /// <summary>
        /// Gets the cell to be displayed. You can have numerous cell types, allowing variety in your list.
        /// Some examples of this would be headers, footers, and other grouping cells.
        /// </summary>
        /// <param name="scroller">The scroller requesting the cell</param>
        /// <param name="dataIndex">The index of the data that the scroller is requesting</param>
        /// <param name="cellIndex">The index of the list. This will likely be different from the dataIndex if the scroller is looping</param>
        /// <returns>The cell for the scroller to use</returns>
        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            // first, we get a cell from the scroller by passing a prefab.
            // if the scroller finds one it can recycle it will do so, otherwise
            // it will create a new cell.
            CellView cellView = scroller.GetCellView(cellViewPrefab) as CellView;

            // data index of the first sub cell
            var di = dataIndex * numberOfCellsPerRow;

            cellView.name = "Cell " + (di).ToString() + " to " + ((di) + numberOfCellsPerRow - 1).ToString();

            // pass in a reference to our data set with the offset for this cell
            cellView.SetData(ref _data, di);

            // return the cell to the scroller
            return cellView;
        }

        #endregion
    }
}
