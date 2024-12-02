using UnityEngine;
using System.Collections.Generic;
using EnhancedUI;
using EnhancedUI.EnhancedScroller;

namespace EnhancedScrollerDemos.ViewDrivenCellSizes
{
    /// <summary>
    /// This demo shows how you can use the calculated size of the cell view to drive the scroller's cell sizes.
    /// This can be good for cases where you do not know how large each cell will need to be until the contents are
    /// populated. An example of this would be text cells containing unknown information.
    /// </summary>
    public class Controller : MonoBehaviour, IEnhancedScrollerDelegate
    {
        private List<Data> _data;

        /// <summary>
        /// This member tells the scroller that we need
        /// the cell views to figure out how much space to use.
        /// This is only set to true on the first pass to reduce
        /// processing required.
        /// </summary>
        private bool _calculateLayout;

        public EnhancedScroller scroller;
        public EnhancedScrollerCellView cellViewPrefab;

        void Start()
        {
            // set the application frame rate.
            // this improves smoothness on some devices
            Application.targetFrameRate = 60;

            scroller.Delegate = this;
            LoadData();
        }

        /// <summary>
        /// Populates the data with some random Lorum Ipsum text
        /// </summary>
        private void LoadData()
        {
            _data = new List<Data>();

            // populate the scroller with some text
            for (var i = 0; i < 7; i++)
            {
                _data.Add(new Data() { cellSize = 0, someText = (i * 11 + 0).ToString() + " Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam augue enim, scelerisque ac diam nec, efficitur aliquam orci. Vivamus laoreet, libero ut aliquet convallis, dolor elit auctor purus, eget dapibus elit libero at lacus. Aliquam imperdiet sem ultricies ultrices vestibulum. Proin feugiat et dui sit amet ultrices. Quisque porta lacus justo, non ornare nulla eleifend at. Nunc malesuada eget neque sit amet viverra. Donec et lectus ac lorem elementum porttitor. Praesent urna felis, dapibus eu nunc varius, varius tincidunt ante. Vestibulum vitae nulla malesuada, consequat justo eu, dapibus elit. Nulla tristique enim et convallis facilisis." });
                _data.Add(new Data() { cellSize = 0, someText = (i * 11 + 1).ToString() + " Nunc convallis, ipsum a porta viverra, tortor velit feugiat est, eget consectetur ex metus vel diam." });
                _data.Add(new Data() { cellSize = 0, someText = (i * 11 + 2).ToString() + " Phasellus laoreet vitae lectus sit amet venenatis. Duis scelerisque ultricies tincidunt. Cras ullamcorper lectus sed risus porttitor, id viverra urna venenatis. Maecenas in odio sed mi tempus porta et a justo. Nullam non ullamcorper est. Nam rhoncus nulla quis commodo aliquam. Maecenas pulvinar est sed ex iaculis, eu pretium tellus placerat. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Praesent in ipsum faucibus, fringilla lectus id, congue est. " });
                _data.Add(new Data() { cellSize = 0, someText = (i * 11 + 3).ToString() + " Fusce ex lectus." });
                _data.Add(new Data() { cellSize = 0, someText = (i * 11 + 4).ToString() + " Fusce mollis elementum sem euismod malesuada. Aenean et convallis turpis. Suspendisse potenti." });
                _data.Add(new Data() { cellSize = 0, someText = (i * 11 + 5).ToString() + " Fusce nec sapien orci. Pellentesque mollis ligula vitae interdum imperdiet. Aenean ultricies velit at turpis luctus, nec lacinia ligula malesuada. Nulla facilisi. Donec at nisi lorem. Aenean vestibulum velit velit, sed eleifend dui sodales in. Nunc vulputate, nulla non facilisis hendrerit, neque dolor lacinia orci, et fermentum nunc quam vel purus. Donec gravida massa non ullamcorper consectetur. Sed pellentesque leo ac ornare egestas. " });
                _data.Add(new Data() { cellSize = 0, someText = (i * 11 + 6).ToString() + " Curabitur non dignissim turpis, vel viverra elit. Cras in sem rhoncus, gravida velit ut, consectetur erat. Proin ac aliquet nulla. Mauris quis augue nisi. Sed purus magna, mollis sed massa ac, scelerisque lobortis leo. Nullam at facilisis ex. Nullam ut accumsan orci. Integer vitae dictum felis, quis tristique sem. Suspendisse potenti. Curabitur bibendum eleifend eros at porta. Ut malesuada consectetur arcu nec lacinia. " });
                _data.Add(new Data() { cellSize = 0, someText = (i * 11 + 7).ToString() + " Pellentesque pulvinar ac arcu fermentum interdum. Pellentesque gravida faucibus ipsum at blandit. Vestibulum pharetra erat sit amet feugiat sodales. Nunc et dui viverra tellus efficitur egestas. Sed ex mauris, eleifend in nisi sed, consequat tincidunt elit. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Proin vel bibendum enim. Etiam feugiat nulla ac dui commodo, eget vehicula est scelerisque. In metus neque, congue a justo ac, consequat lacinia neque. Vivamus non velit vitae ex dictum pharetra. Aliquam blandit nisi eget libero feugiat porta. " });
                _data.Add(new Data() { cellSize = 0, someText = (i * 11 + 8).ToString() + " Proin bibendum ligula a pulvinar convallis. Mauris tincidunt tempor ipsum id viverra. Vivamus congue ipsum venenatis tellus semper, vel venenatis mauris finibus. Vivamus a nisl in lacus fermentum varius. Mauris bibendum magna placerat risus interdum, vitae facilisis nulla pellentesque. Curabitur vehicula odio quis magna pulvinar, et lacinia ante bibendum. Morbi laoreet eleifend ante, quis luctus augue luctus sit amet. Sed consectetur enim et orci posuere euismod. Curabitur sollicitudin metus eu nisl dictum suscipit. " });
                _data.Add(new Data() { cellSize = 0, someText = (i * 11 + 9).ToString() + " Sed gravida augue ligula, tempus auctor ante rutrum sit amet. Vestibulum finibus magna ut viverra rhoncus. Vestibulum rutrum eu nibh interdum imperdiet. Curabitur ac nunc a turpis ultricies dictum. Phasellus in molestie eros. Morbi porta imperdiet odio sed pharetra. Cras blandit tincidunt ultricies. " });
                _data.Add(new Data() { cellSize = 0, someText = (i * 11 + 10).ToString() + " Integer pellentesque viverra orci, sollicitudin luctus dui rhoncus sed. Duis placerat at felis vel placerat. Mauris massa urna, scelerisque vitae posuere vitae, ultrices in nibh. Mauris posuere hendrerit viverra. In lacinia urna nibh, ut lobortis lectus finibus et. Aliquam arcu dolor, suscipit eget massa id, eleifend dapibus est. Quisque eget bibendum urna. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed condimentum pulvinar ornare. Aliquam venenatis eget nunc et euismod. " });
            }

            ResizeScroller();
        }

        /// <summary>
        /// This function adds a new record, resizing the scroller and calculating the sizes of all cells
        /// </summary>
        public void AddNewRow()
        {
            // first, clear out the cells in the scroller so the new text transforms will be reset
            scroller.ClearAll();

            // reset the scroller's position so that it is not outside of the new bounds
            scroller.ScrollPosition = 0;

            // second, reset the data's cell view sizes
            foreach (var item in _data)
            {
                item.cellSize = 0;
            }

            // now we can add the data row
            _data.Add(new Data() { cellSize = 0, someText = _data.Count.ToString() + " New Row Added!" });

            ResizeScroller();

            // optional: jump to the end of the scroller to see the new content
            scroller.JumpToDataIndex(_data.Count - 1, 1f, 1f);
        }

        /// <summary>
        /// This function will expand the scroller to accommodate the cells, reload the data to calculate the cell sizes,
        /// reset the scroller's size back, then reload the data once more to display the cells.
        /// </summary>
        private void ResizeScroller()
        {
            // capture the scroller dimensions so that we can reset them when we are done
            var rectTransform = scroller.GetComponent<RectTransform>();
            var size = rectTransform.sizeDelta;

            // set the dimensions to the largest size possible to acommodate all the cells
            rectTransform.sizeDelta = new Vector2(size.x, float.MaxValue);

            // First Pass: reload the scroller so that it can populate the text UI elements in the cell view.
            // The content size fitter will determine how big the cells need to be on subsequent passes.
            _calculateLayout = true;
            scroller.ReloadData();

            // reset the scroller size back to what it was originally
            rectTransform.sizeDelta = size;

            // Second Pass: reload the data once more with the newly set cell view sizes and scroller content size
            _calculateLayout = false;
            scroller.ReloadData();
        }

        #region EnhancedScroller Handlers

        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            return _data.Count;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            // we pull the size of the cell from the model.
            // First pass (frame countdown 2): this size will be zero as set in the LoadData function
            // Second pass (frame countdown 1): this size will be set to the content size fitter in the cell view
            // Third pass (frmae countdown 0): this set value will be pulled here from the scroller
            return _data[dataIndex].cellSize;
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            CellView cellView = scroller.GetCellView(cellViewPrefab) as CellView;

            // tell the cell view to calculate its layout on the first pass,
            // otherwise just use the size set in the data.
            cellView.SetData(_data[dataIndex], _calculateLayout);

            return cellView;
        }


        #endregion
    }
}
