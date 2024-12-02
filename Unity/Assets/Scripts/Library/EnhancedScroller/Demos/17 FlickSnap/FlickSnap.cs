using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using EnhancedUI.EnhancedScroller;

namespace EnhancedScrollerDemos.FlickSnap
{
    /// <summary>
    /// This class handles the snapping of the scroller.
    /// It uses the Unity interfaces for drag handling.
    /// </summary>
    public class FlickSnap : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        /// <summary>
        /// Link to the EnhancedScroller component
        /// </summary>
        public EnhancedScroller scroller;

        /// <summary>
        /// The tween transition type while snapping
        /// </summary>
        public EnhancedScroller.TweenType snapTweenType;

        /// <summary>
        /// The time it takes to tween to the new position
        /// </summary>
        public float snapTweenTime;

        /// <summary>
        /// The number of elements in the scroller.
        /// This is set by the controller
        /// </summary>
        public int MaxDataElements { get; set; }

        /// <summary>
        /// Whether the scroller is being dragged
        /// </summary>
        private bool _isDragging = false;

        /// <summary>
        /// The start position of the drag. Used to calculate the delta when the dragging stops.
        /// </summary>
        private Vector2 _dragStartPosition = Vector2.zero;

        /// <summary>
        /// The index of the scroller when the dragging starts
        /// </summary>
        private int _currentIndex = -1;

        /// <summary>
        /// Called when the dragging starts on the scroller. This is a Unity method handled by the IBeginDragHandler interface.
        /// </summary>
        /// <param name="data"></param>
        public void OnBeginDrag(PointerEventData data)
        {
            // if the scroller is tweening, then don't capture a new drag.
            // let the tweening complete first.
            if (scroller.IsTweening)
            {
                return;
            }

            // set the dragging as true
            _isDragging = true;

            // capture the start position so we can calculate the delta when the dragging stops
            _dragStartPosition = data.position;

            // capture the current scroller index
            _currentIndex = scroller.StartDataIndex;
        }

        /// <summary>
        /// Called when the dragging stops on the scroller. This is a Unity method handled by the IEndDragHandler interface.
        /// </summary>
        /// <param name="data"></param>
        public void OnEndDrag(PointerEventData data)
        {
            // only if we are dragging
            if (_isDragging)
            {
                // turn off the dragging toggle state
                _isDragging = false;

                // calculate the delta position based on the current pointer position and the starting position
                var delta = data.position - _dragStartPosition;

                // calculate the jump to index. for this example it will either be one element above or below the index
                // captured at the start of the drag. Initialize to -1 (meaning no index to jump to)
                var jumpToIndex = -1;

                if (delta.y < 0)
                {
                    // if the change is less than zero, then we jump one element up (backward)
                    jumpToIndex = _currentIndex - 1;
                }
                else if (delta.y > 0)
                {
                    // if the change is greater than zero, then we jump one element down (forward)
                    jumpToIndex = _currentIndex + 1;
                }

                // if there was a change
                if (jumpToIndex != -1)
                {
                    // jump to the new index using the tween settings in the inspector, clamping to the scroller's data count
                    scroller.JumpToDataIndex(Mathf.Clamp(jumpToIndex, 0, MaxDataElements - 1), tweenType: snapTweenType, tweenTime: snapTweenTime);
                }
            }
        }
    }
}
