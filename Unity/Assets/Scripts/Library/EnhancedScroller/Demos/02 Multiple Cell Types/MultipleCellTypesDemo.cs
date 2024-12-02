using UnityEngine;
using System.Collections;
using EnhancedUI;
using EnhancedUI.EnhancedScroller;

namespace EnhancedScrollerDemos.MultipleCellTypesDemo
{
    public class MultipleCellTypesDemo : MonoBehaviour, IEnhancedScrollerDelegate
    {
        /// <summary>
        /// Internal representation of our data. Note that the scroller will never see
        /// this, so it separates the data from the layout using MVC principles.
        /// </summary>
        private SmallList<Data> _data;

        /// <summary>
        /// This is our scroller we will be a delegate for
        /// </summary>
        public EnhancedScroller scroller;

        public EnhancedScrollerCellView headerCellViewPrefab;
        public EnhancedScrollerCellView rowCellViewPrefab;
        public EnhancedScrollerCellView footerCellViewPrefab;

        /// <summary>
        /// The base path to the resources folder where sprites are located
        /// </summary>
        public string resourcePath;

        void Start()
        {
            // set the application frame rate.
            // this improves smoothness on some devices
            Application.targetFrameRate = 60;

            // tell the scroller that this script will be its delegate
            scroller.Delegate = this;

            LoadData();
        }

        /// <summary>
        /// Populates the data with a lot of records
        /// </summary>
        private void LoadData()
        {
            // create some data
            // note we are using different data class fields for the header, row, and footer rows. This works due to polymorphism.

            _data = new SmallList<Data>();

            _data.Add(new HeaderData() { category = "Platinum Players" });
            _data.Add(new RowData() { userName = "John Smith", userAvatarSpritePath = resourcePath + "/avatar_male", userHighScore = 21323199 });
            _data.Add(new RowData() { userName = "Jane Doe", userAvatarSpritePath = resourcePath + "/avatar_female", userHighScore = 20793219 });
            _data.Add(new RowData() { userName = "Julie Prost", userAvatarSpritePath = resourcePath + "/avatar_female", userHighScore = 19932132 });
            _data.Add(new FooterData());

            _data.Add(new HeaderData() { category = "Gold Players" });
            _data.Add(new RowData() { userName = "Jim Bob", userAvatarSpritePath = resourcePath + "/avatar_male", userHighScore = 1002132 });
            _data.Add(new RowData() { userName = "Susan Anthony", userAvatarSpritePath = resourcePath + "/avatar_female", userHighScore = 991234 });
            _data.Add(new FooterData());

            _data.Add(new HeaderData() { category = "Silver Players" });
            _data.Add(new RowData() { userName = "Gary Richards", userAvatarSpritePath = resourcePath + "/avatar_male", userHighScore = 905723 });
            _data.Add(new RowData() { userName = "John Doe", userAvatarSpritePath = resourcePath + "/avatar_male", userHighScore = 702318 });
            _data.Add(new RowData() { userName = "Lisa Ford", userAvatarSpritePath = resourcePath + "/avatar_female", userHighScore = 697767 });
            _data.Add(new RowData() { userName = "Jacob Morris", userAvatarSpritePath = resourcePath + "/avatar_male", userHighScore = 409393 });
            _data.Add(new RowData() { userName = "Carolyn Shephard", userAvatarSpritePath = resourcePath + "/avatar_female", userHighScore = 104352 });
            _data.Add(new RowData() { userName = "Guy Wilson", userAvatarSpritePath = resourcePath + "/avatar_male", userHighScore = 88321 });
            _data.Add(new RowData() { userName = "Jackie Jones", userAvatarSpritePath = resourcePath + "/avatar_female", userHighScore = 20826 });
            _data.Add(new RowData() { userName = "Sally Brewer", userAvatarSpritePath = resourcePath + "/avatar_female", userHighScore = 17389 });
            _data.Add(new RowData() { userName = "Joe West", userAvatarSpritePath = resourcePath + "/avatar_male", userHighScore = 2918 });
            _data.Add(new FooterData());

            // tell the scroller to reload now that we have the data
            scroller.ReloadData();
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
            // we will determine the cell height based on what kind of row it is

            if (_data[dataIndex] is HeaderData)
            {
                // header views
                return 70f;
            }
            else if (_data[dataIndex] is RowData)
            {
                // row views
                return 100f;
            }
            else
            {
                // footer views
                return 90f;
            }
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
            CellView cellView;

            // determin what cell view to get based on the type of the data row

            if (_data[dataIndex] is HeaderData)
            {
                // get a header cell prefab from the scroller, recycling old cells if possible
                cellView = scroller.GetCellView(headerCellViewPrefab) as CellViewHeader;

                // optional for clarity: set the cell's name to something to indicate this is a header row
                cellView.name = "[Header] " + (_data[dataIndex] as HeaderData).category;
            }
            else if (_data[dataIndex] is RowData)
            {
                // get a row cell prefab from the scroller, recycling old cells if possible
                cellView = scroller.GetCellView(rowCellViewPrefab) as CellViewRow;

                // optional for clarity: set the cell's name to something to indicate this is a row
                cellView.name = "[Row] " + (_data[dataIndex] as RowData).userName;
            }
            else
            {
                // get a footer cell prefab from the scroller, recycling old cells if possible
                cellView = scroller.GetCellView(footerCellViewPrefab) as CellViewFooter;

                // optional for clarity: set the cell's name to something to indicate this is a footer row
                cellView.name = "[Footer]";
            }

            // set the cell view's data. We can do this because we declared a single SetData function
            // in the CellView base class, saving us from having to call this for each cell type
            cellView.SetData(_data[dataIndex]);

            // return the cellView to the scroller
            return cellView;
        }

        #endregion
    }
}
