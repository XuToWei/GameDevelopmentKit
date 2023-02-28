using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EnhancedUI;
using EnhancedUI.EnhancedScroller;

namespace EnhancedScrollerDemos.NestedScrollers
{
    /// <summary>
    /// This example scene shows one way you could set up nested scrollers. 
    /// Each MasterCellView in the master scroller contains an EnhancedScroller which in turn contains DetailCellViews.
    ///
    /// Events are passed from the detail cells up to the master scroll rect so that scrolling can be done naturally 
    /// in both the horizontal and vertical directions.The detail ScrollRectEx is an extension of Unity's ScrollRect 
    /// that allows this event pass through.
    /// </summary>
    public class Controller : MonoBehaviour, IEnhancedScrollerDelegate
    {
        /// <summary>
        /// Internal representation of our data. Note that the scroller will never see
        /// this, so it separates the data from the layout using MVC principles.
        /// </summary>
        private List<MasterData> _data;

        /// <summary>
        /// This is our scroller we will be a delegate for. The master scroller contains mastercellviews which in turn
        /// contain EnhancedScrollers
        /// </summary>
        public EnhancedScroller masterScroller;

        /// <summary>
        /// This will be the prefab of each cell in our scroller. Note that you can use more
        /// than one kind of cell, but this example just has the one type.
        /// </summary>
        public EnhancedScrollerCellView masterCellViewPrefab;

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
            masterScroller.Delegate = this;

            // load in a large set of data
            LoadData();
        }

        /// <summary>
        /// Populates the data with a lot of records
        /// </summary>
        private void LoadData()
        {
            // set up some simple data. This will be a two-dimensional array,
            // specifically a list within a list.

            _data = new List<MasterData>();
            for (var i = 0; i < 1000; i++)
            {
                var masterData = new MasterData()
                {
                    normalizedScrollPosition = 0,
                    childData = new List<DetailData>()
                };

                _data.Add(masterData);

                for (var j = 0; j < 20; j++)
                    masterData.childData.Add(new DetailData() { someText = i.ToString() + "," + j.ToString() });
            }

            // tell the scroller to reload now that we have the data
            masterScroller.ReloadData();
        }

        #region EnhancedScroller Handlers

        /// <summary>
        /// This tells the scroller the number of cells that should have room allocated. This should be the length of your data array.
        /// </summary>
        /// <param name="scroller">The scroller that is requesting the data size</param>
        /// <returns>The number of cells</returns>
        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            // in this example, we just pass the number of our data elements
            return _data.Count;
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
            // in this example, our master cells are 100 pixels tall
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
            MasterCellView masterCellView = scroller.GetCellView(masterCellViewPrefab) as MasterCellView;

            // set the name of the game object to the cell's data index.
            // this is optional, but it helps up debug the objects in 
            // the scene hierarchy.
            masterCellView.name = "Master Cell " + dataIndex.ToString();

            // in this example, we just pass the data to our cell's view which will update its UI
            masterCellView.SetData(_data[dataIndex]);

            // return the cell to the scroller
            return masterCellView;
        }

        #endregion
    }
}
