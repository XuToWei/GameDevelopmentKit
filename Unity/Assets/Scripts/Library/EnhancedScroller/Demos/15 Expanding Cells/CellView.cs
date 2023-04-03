using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;
using EnhancedUI;
using System;

namespace EnhancedScrollerDemos.ExpandingCells
{
    /// <summary>
    /// This is the view of our cell which handles how the cell looks.
    /// </summary>
    public class CellView : EnhancedScrollerCellView
    {
        private Tween tween;
        private LayoutElement layoutElement;

        private Data data;

        public Text dataIndexText;
        public Text headerText;
        public Text descriptionText;
        public Action<int, int> initializeTween;
        public Action<int, int, float, float> updateTween;
        public Action<int, int> endTween;

        private void Start()
        {
            tween = GetComponent<Tween>();
            layoutElement = GetComponent<LayoutElement>();
        }

        /// <summary>
        /// This function just takes the Demo data and displays it
        /// </summary>
        /// <param name="data"></param>
        public void SetData(Data data, int dataIndex, float collapsedSize, float expandedSize, Action<int, int> initializeTween, Action<int, int, float, float> updateTween, Action<int, int> endTween)
        {
            this.dataIndex = dataIndex;
            this.initializeTween = initializeTween;
            this.updateTween = updateTween;
            this.endTween = endTween;

            dataIndexText.text = dataIndex.ToString();
            headerText.text = data.headerText;
            descriptionText.text = data.descriptionText;

            descriptionText.enabled = data.isExpanded;

            this.data = data;
        }

        /// <summary>
        /// Called when the cell is selected
        /// </summary>
        public void CellButton_Clicked()
        {
            if (initializeTween != null)
            {
                // start the tweening process by telling the controller
                // to prepare all the cells
                initializeTween(dataIndex, cellIndex);
            }
        }

        /// <summary>
        /// Called from the controller to kick off the cell's tweening
        /// </summary>
        public void BeginTween()
        {
            // hide the description text so that it does not float outside
            // of the cell's view bounds while tweening
            descriptionText.enabled = false;

            if (!data.isExpanded)
            {
                // collapse cell view
                layoutElement.minHeight = data.expandedSize;

                // if this is an immediate tween, just call the TweenCompleted method
                if (data.tweenType == Tween.TweenType.immediate)
                {
                    TweenCompleted();
                    return;
                }

                StartCoroutine(tween.TweenPosition(data.tweenType, data.tweenTimeCollapse, data.expandedSize, data.collapsedSize, TweenUpdated, TweenCompleted));
            }
            else
            {
                // expand cell view
                layoutElement.minHeight = data.collapsedSize;

                // if this is an immediate tween, just call the TweenCompleted method
                if (data.tweenType == Tween.TweenType.immediate)
                {
                    TweenCompleted();
                    return;
                }

                StartCoroutine(tween.TweenPosition(data.tweenType, data.tweenTimeExpand, data.collapsedSize, data.expandedSize, TweenUpdated, TweenCompleted));
            }
        }

        /// <summary>
        /// Called when the cell's size changes
        /// </summary>
        /// <param name="newValue">The new cell size</param>
        /// <param name="delta">The change in the cell's size</param>
        private void TweenUpdated(float newValue, float delta)
        {
            // update the size of the cell view
            layoutElement.minHeight += delta; // newValue;

            if (updateTween != null)
            {
                // call the update tween on the controller
                // in order to update the last padder
                updateTween(dataIndex, cellIndex, newValue, delta);
            }
        }

        /// <summary>
        /// Called when the cell size has finished tweening
        /// </summary>
        private void TweenCompleted()
        {
            if (data.isExpanded)
            {
                // show the description text if the cell is expanded
                descriptionText.enabled = true;
            }

            if (endTween != null)
            {
                // tween is completed, so now we can reload the scroller
                endTween(dataIndex, cellIndex);
            }
        }
    }
}
