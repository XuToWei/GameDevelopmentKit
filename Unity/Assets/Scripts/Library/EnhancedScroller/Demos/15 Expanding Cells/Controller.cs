using UnityEngine;
using System.Collections;
using EnhancedUI;
using EnhancedUI.EnhancedScroller;

namespace EnhancedScrollerDemos.ExpandingCells
{
    /// <summary>
    /// This example shows how you can expand and collapse cells
    /// </summary>
	public class Controller : MonoBehaviour, IEnhancedScrollerDelegate
    {
        /// <summary>
        /// The data to store for our cells
        /// </summary>
        private SmallList<Data> _data;

        /// <summary>
        /// The last padder's active state before our expansion / collapse
        /// </summary>
        private bool _lastPadderActive;

        /// <summary>
        /// The last padder's size before our expansion / collapse
        /// </summary>
        private float _lastPadderSize;

        /// <summary>
        /// The scroller that will be showing the cells
        /// </summary>
        public EnhancedScroller scroller;

        /// <summary>
        /// The cell prefab to use in the scroller
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

            // set some space to look for cells before and after the scroll rect.
            // this is useful for when the cells are expanding, we don't want to see empty space.
            scroller.lookAheadBefore = 1000f;
            scroller.lookAheadAfter = 1000f;

            // load in a large set of data
            LoadData();
        }

        /// <summary>
        /// Populates the data with a lot of records
        /// </summary>
        private void LoadData()
        {
            // set up some simple data
            _data = new SmallList<Data>();
            for (var i = 0; i < 50; i++)
            {
                if (i % 3 == 0)
                {
                    _data.Add(new Data()
                    {
                        headerText = "Multiple Expand",
                        descriptionText = "Expanding this cell will not collapse other cells. This allows you to have multiple cells expanded at once.\n\nClick the cell again to collapse.",
                        isExpanded = false,
                        expandedSize = 300f,
                        collapsedSize = 60f,
                        tweenType = Tween.TweenType.immediate,
                        tweenTimeExpand = 0,
                        tweenTimeCollapse = 0
                    });
                }
                else if ((i + 1) % 3 == 0)
                {
                    _data.Add(new Data()
                    {
                        headerText = "Tween Expand",
                        descriptionText = "This cell will animate its size when clicked.\n\nClick the cell again to collapse.",
                        isExpanded = false,
                        expandedSize = 300f,
                        collapsedSize = 60f,
                        tweenType = Tween.TweenType.easeInOutSine,
                        tweenTimeExpand = 0.5f,
                        tweenTimeCollapse = 0.5f
                    });
                }
                else
                {
                    _data.Add(new Data()
                    {
                        headerText = "Single Expand",
                        descriptionText = "Expanding this cell will collapse other cells.\n\nClick the cell again to collapse.",
                        isExpanded = false,
                        expandedSize = 300f,
                        collapsedSize = 60f,
                        tweenType = Tween.TweenType.immediate,
                        tweenTimeExpand = 0,
                        tweenTimeCollapse = 0
                    });
                }
            }

            // tell the scroller to reload now that we have the data
            scroller.ReloadData();
        }

        /// <summary>
        /// This method is called by the cell view when the cell is clicked.
        /// It will set all the expansion properties of the cells, reload the scroller,
        /// set the position correctly, and kick off the tweening of the cell size
        /// </summary>
        /// <param name="dataIndex">The data index of the cell</param>
        /// <param name="cellViewIndex">The cell view index of the cell</param>
        private void InitializeTween(int dataIndex, int cellViewIndex)
        {
            // toggle the cell's expansion
            _data[dataIndex].isExpanded = !_data[dataIndex].isExpanded;

            // set all the other cells' expansion properties
            for (var i = 0; i < _data.Count; i++)
            {
                // if not this cell
                if (i != dataIndex)
                {
                    // if the clicked cell is a single cell or if this iteration is a single cell
                    // collapse other cells
                    if (((dataIndex + 2) % 3 == 0) || ((i + 2) % 3 == 0))
                    {
                        _data[i].isExpanded = false;
                    }
                }
            }

            // get the cell's position (using the cell view index in case of looping)
            var cellPosition = scroller.GetScrollPositionForCellViewIndex(cellViewIndex, EnhancedScroller.CellViewPositionEnum.Before);

            // get the offset of the cell from the top of the scroll rect
            var tweenCellOffset = cellPosition - scroller.ScrollPosition;

            // turn off loop jumping so that the scroller will not try to jump to a new location as the cell is expanding / collapsing
            scroller.IgnoreLoopJump(true);

            // reload the scroller to accommodate the new cell sizes
            scroller.ReloadData();

            // get the new position of the cell (using the cell view index in case of looping)
            cellPosition = scroller.GetScrollPositionForCellViewIndex(cellViewIndex, EnhancedScroller.CellViewPositionEnum.Before);

            // set the scroller's position to focus on the cell, using the offset calculated above
            scroller.SetScrollPositionImmediately(cellPosition - tweenCellOffset);

            // turn loop jumping back on
            scroller.IgnoreLoopJump(false);

            // if this cell has an immediate tween type, then we can just exit the
            // method and not worry about adjusting padder sizes or calling
            // the tweening on the cell
            if (_data[dataIndex].tweenType == Tween.TweenType.immediate)
            {
                return;
            }

            // cache the last padder's active state and size for after the tween
            _lastPadderActive = scroller.LastPadder.IsActive();
            _lastPadderSize = scroller.LastPadder.minHeight;

            // manually set the last padder's size so that we can tween the cell
            // size without distorting all the cells' sizes
            if (_data[dataIndex].isExpanded)
            {
                scroller.LastPadder.minHeight += _data[dataIndex].SizeDifference;
            }
            else
            {
                scroller.LastPadder.minHeight -= _data[dataIndex].SizeDifference;
            }

            // make sure the last padder is active so that we can tween its size
            scroller.LastPadder.gameObject.SetActive(true);

            // grab the cell that was clicked so that we can start tweening.
            // note that we cannot just pass in the cell to this method since we
            // are calling ReloadData, which destroys that cell. Grabbing it
            // here is the only way to get an active cell after the reload.
            var cellViewTween = scroller.GetCellViewAtDataIndex(dataIndex) as CellView;

            // start the cell's tweening process
            cellViewTween.BeginTween();
        }

        /// <summary>
        /// This method is called by the cell that is changing size. As the cell changes size,
        /// we need to adjust the last padder's size to accommodate. Otherwise all the cell's
        /// would distort in size as the tween occurs.
        /// </summary>
        /// <param name="dataIndex">The data index of the cell that is tweening</param>
        /// <param name="cellViewIndex">The cell view index of the cell that is tweening</param>
        /// <param name="newValue">The new value of the cell's size</param>
        /// <param name="delta">The change in the cell's size</param>
        private void TweenUpdated(int dataIndex, int cellViewIndex, float newValue, float delta)
        {
            // set the last padder's height by adjusting it by the cell's size delta
            // to offset the change
            scroller.LastPadder.minHeight -= delta;
        }

        /// <summary>
        /// This method is called when the cell has stopped changing size. We just
        /// need to set the last padder's properties back
        /// </summary>
        /// <param name="dataIndex">The data index of the cell that is tweening</param>
        /// <param name="cellViewIndex">The cell view index of the cell that is tweening</param>
        private void TweenEnd(int dataIndex, int cellViewIndex)
        {
            // set the last padder's active state back to what we captured before the tween
            scroller.LastPadder.gameObject.SetActive(_lastPadderActive);

            // set the last padder's size back to what we captured before the tween
            scroller.LastPadder.minHeight = _lastPadderSize;
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
            // pass the expanded size if the cell is expanded, else pass the collapsed size
            return _data[dataIndex].Size;
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

            cellView.name = "Cell " + dataIndex.ToString();

            // pass in a reference to our data 
            cellView.SetData(_data[dataIndex], dataIndex, _data[dataIndex].collapsedSize, _data[dataIndex].expandedSize, InitializeTween, TweenUpdated, TweenEnd);

            // return the cell to the scroller
            return cellView;
        }

        #endregion
    }
}
