using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EnhancedUI;
using EnhancedUI.EnhancedScroller;

namespace EnhancedScrollerDemos.Chat
{
    /// <summary>
    /// Simple example of one way a chat scroller could look
    /// </summary>
    public class Chat : MonoBehaviour, IEnhancedScrollerDelegate
    {
        /// <summary>
        /// Internal representation of our data. Note that the scroller will never see
        /// this, so it separates the data from the layout using MVC principles.
        /// </summary>
        private List<Data> _data;

        /// <summary>
        /// This stores the total size of all the cells,
        /// plus the scroller's top and bottom padding.
        /// This will be used to calculate the spacer required
        /// </summary>
        private float _totalCellSize = 0;

        /// <summary>
        /// Stores the scroller's position before jumping to the new chat cell
        /// </summary>
        private float _oldScrollPosition = 0;

        /// <summary>
        /// This is our scroller we will be a delegate for
        /// </summary>
        public EnhancedScroller scroller;

        /// <summary>
        /// The input field for texts from us
        /// </summary>
        public UnityEngine.UI.InputField myInputField;

        /// <summary>
        /// The input field for simulated texts from another person
        /// </summary>
        public UnityEngine.UI.InputField otherInputField;

        /// <summary>
        /// This will be the prefab of our chat cell
        /// </summary>
        public EnhancedScrollerCellView myTextCellViewPrefab;

        /// <summary>
        /// This will be the prefab of another person's chat cell
        /// </summary>
        public EnhancedScrollerCellView otherTextCellViewPrefab;

        /// <summary>
        /// This will be the prefab of our first cell to push the other cells to the bottom
        /// </summary>
        public EnhancedScrollerCellView spacerCellViewPrefab;

        /// <summary>
        /// The estimated width of each character. Note that this is just an estimate
        /// since most fonts are not mono-spaced.
        /// </summary>
        public int characterWidth = 8;

        /// <summary>
        /// The height of each character.
        /// </summary>
        public int characterHeight = 26;

        void Start()
        {
            // set the application frame rate.
            // this improves smoothness on some devices
            Application.targetFrameRate = 60;

            // tell the scroller that this script will be its delegate
            scroller.Delegate = this;

            // set up a single data item containing the spacer
            // this pushes the cells down to the bottom
            _data = new List<Data>();
            _data.Add(new Data() { cellType = Data.CellType.Spacer });

            // call resize scroller to calculate and set up the scroll
            ResizeScroller();

            // focus on the chat input field
            myInputField.ActivateInputField();
        }

        /// <summary>
        /// Called every frame to check for return key presses.
        /// A return key press will send the chat
        /// </summary>
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (myInputField.isFocused)
                {
                    SendButton_Click();
                }
                else if (otherInputField.isFocused)
                {
                    OtherSendButton_Click();
                }
            }
        }

        /// <summary>
        /// This function adds a new record, resizing the scroller and calculating the sizes of all cells
        /// </summary>
        public void AddNewRow(Data.CellType cellType, string text)
        {
            // first, clear out the cells in the scroller so the new text transforms will be reset
            scroller.ClearAll();

            _oldScrollPosition = scroller.ScrollPosition;

            // reset the scroller's position so that it is not outside of the new bounds
            scroller.ScrollPosition = 0;


            // calculate the space needed for the text in the cell

            // get the estimated total width of the text (estimated because the font is assumed to be mono-spaced)
            float totalTextWidth = (float)text.Length * (float)characterWidth;

            // get the number of rows the text will take up by dividing the total width by the widht of the cell
            int numRows = Mathf.CeilToInt(totalTextWidth / scroller.GetComponent<RectTransform>().sizeDelta.x) + 1;

            // get the cell size by multiplying the rows times the character height
            var cellSize = numRows * (float)characterHeight;

            // now we can add the data row
            _data.Add(new Data()
            {
                cellType = cellType,
                cellSize = cellSize,
                someText = text
            });

            ResizeScroller();

            // jump to the end of the scroller to see the new content.
            // once the jump is completed, reset the spacer's size
            scroller.JumpToDataIndex(_data.Count - 1, 1f, 1f, tweenType: EnhancedScroller.TweenType.easeInOutSine, tweenTime: 0.5f, jumpComplete: ResetSpacer);
        }

        /// <summary>
        /// This function will expand the scroller to accommodate the cells, reload the data to calculate the cell sizes,
        /// reset the scroller's size back, then reload the data once more to display the cells.
        /// </summary>
        private void ResizeScroller()
        {
            // capture the scroll rect size.
            // this will be used at the end of this method to determine the final scroll position
            var scrollRectSize = scroller.ScrollRectSize;

            // capture the scroller's position so we can smoothly scroll from it to the new cell
            var offset = _oldScrollPosition - scroller.ScrollSize;

            // capture the scroller dimensions so that we can reset them when we are done
            var rectTransform = scroller.GetComponent<RectTransform>();
            var size = rectTransform.sizeDelta;

            // set the dimensions to the largest size possible to acommodate all the cells
            rectTransform.sizeDelta = new Vector2(size.x, float.MaxValue);

            // calculate the total size required by all cells. This will be used when we determine
            // where to end up at after we reload the data on the second pass.
            _totalCellSize = scroller.padding.top + scroller.padding.bottom;
            for (var i = 1; i < _data.Count; i++)
            {
                _totalCellSize += _data[i].cellSize + (i < _data.Count - 1 ? scroller.spacing : 0);
            }

            // set the spacer to the entire scroller size.
            // this is necessary because we need some space to actually do a jump
            _data[0].cellSize = scrollRectSize;

            // reset the scroller size back to what it was originally
            rectTransform.sizeDelta = size;

            // reload the data with the newly set cell view sizes and scroller content size.
            //_calculateLayout = false;
            scroller.ReloadData();

            // set the scroll position to the previous cell (plus the offset of where the scroller currently is) so that we can jump to the new cell.
            scroller.ScrollPosition = (_totalCellSize - _data[_data.Count - 1].cellSize) + offset;
        }

        /// <summary>
        /// This method is called when the new cell has been jumpped to.
        /// It will reset the spacer's cell size to the remainder of the scroller's size minus the
        /// total cell size calculated in ResizeScroller. Finally, it will reload the
        /// scroller to set the new cell sizes.
        /// </summary>
        private void ResetSpacer()
        {
            // reset the spacer's cell size to the scroller's size minus the rest of the cell sizes
            // (or zero if the spacer is no longer needed)
            _data[0].cellSize = Mathf.Max(scroller.ScrollRectSize - _totalCellSize, 0);

            // reload the data to set the new cell size
            scroller.ReloadData(1.0f);
        }

        #region UI Handlers

        /// <summary>
        /// Button handler sending message
        /// </summary>
        public void SendButton_Click()
        {
            // add a chat row from us
            AddNewRow(Data.CellType.MyText, myInputField.text);

            // clear the input field and focus on it
            myInputField.text = "";
            myInputField.ActivateInputField();
        }

        /// <summary>
        /// Button handler sending other person's message
        /// </summary>
        public void OtherSendButton_Click()
        {
            // add a chat row from another person
            AddNewRow(Data.CellType.OtherText, otherInputField.text);

            // clear the input field and focus on it
            otherInputField.text = "";
            otherInputField.ActivateInputField();
        }

        #endregion

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
        /// Gets the cell view size for each cell
        /// </summary>
        /// <param name="scroller"></param>
        /// <param name="dataIndex"></param>
        /// <returns></returns>
        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            // return the cell size for each cell
            return _data[dataIndex].cellSize;
        }

        /// <summary>
        /// Reuse the appropriate cell
        /// </summary>
        /// <param name="scroller"></param>
        /// <param name="dataIndex"></param>
        /// <param name="cellIndex"></param>
        /// <returns></returns>
        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            CellView cellView;

            if (dataIndex == 0)
            {
                // this is the first spacer cell
                cellView = scroller.GetCellView(spacerCellViewPrefab) as CellView;
                cellView.name = "Spacer";
            }
            else
            {
                // this is a chat cell

                if (_data[dataIndex].cellType == Data.CellType.MyText)
                {
                    // this is one of our chat cells
                    cellView = scroller.GetCellView(myTextCellViewPrefab) as CellView;
                }
                else
                {
                    // this is a chat cell from another person
                    cellView = scroller.GetCellView(otherTextCellViewPrefab) as CellView;
                }

                // set the cell's game object name. Not necessary, but nice for debugging.
                cellView.name = "Cell " + dataIndex.ToString();

                // initialize the cell's data so that it can configure its view.
                cellView.SetData(_data[dataIndex]);
            }

            return cellView;
        }

        #endregion
    }
}
