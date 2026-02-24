using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    /// <summary>
    /// 所有指针事件穿透
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PointerThroughAll : MonoBehaviour,
        IPointerClickHandler,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerMoveHandler,
        IPointerEnterHandler,
        IPointerExitHandler,
        IInitializePotentialDragHandler,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler,
        IDropHandler,
        IScrollHandler
    {
        private GameObject m_EnteredTarget;

        public void OnPointerClick(PointerEventData eventData)
        {
            PointerThroughUtility.ExecuteThrough(gameObject, eventData, ExecuteEvents.pointerClickHandler);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            PointerThroughUtility.ExecuteThrough(gameObject, eventData, ExecuteEvents.pointerDownHandler);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            PointerThroughUtility.ExecuteThrough(gameObject, eventData, ExecuteEvents.pointerUpHandler);
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            PointerThroughUtility.ExecuteThrough(gameObject, eventData, ExecuteEvents.pointerMoveHandler);

            if (m_EnteredTarget != null && !PointerThroughUtility.IsPointerInsideTarget(m_EnteredTarget, eventData))
            {
                ExecuteEvents.ExecuteHierarchy(m_EnteredTarget, eventData, ExecuteEvents.pointerExitHandler);
                m_EnteredTarget = null;
            }
            if (m_EnteredTarget == null)
            {
                OnPointerEnter(eventData);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (m_EnteredTarget != null)
            {
                return;
            }
            if (PointerThroughUtility.TryGetThroughTarget(gameObject, eventData, out var target))
            {
                if (PointerThroughUtility.IsPointerInsideTarget(target, eventData))
                {
                    m_EnteredTarget = target;
                    ExecuteEvents.ExecuteHierarchy(target, eventData, ExecuteEvents.pointerEnterHandler);
                }
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (m_EnteredTarget != null)
            {
                if (!PointerThroughUtility.IsPointerInsideTarget(m_EnteredTarget, eventData))
                {
                    ExecuteEvents.ExecuteHierarchy(m_EnteredTarget, eventData, ExecuteEvents.pointerExitHandler);
                }
                m_EnteredTarget = null;
            }
        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            PointerThroughUtility.ExecuteThrough(gameObject, eventData, ExecuteEvents.initializePotentialDrag);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            PointerThroughUtility.ExecuteThrough(gameObject, eventData, ExecuteEvents.beginDragHandler);
        }

        public void OnDrag(PointerEventData eventData)
        {
            PointerThroughUtility.ExecuteThrough(gameObject, eventData, ExecuteEvents.dragHandler);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            PointerThroughUtility.ExecuteThrough(gameObject, eventData, ExecuteEvents.endDragHandler);
        }

        public void OnDrop(PointerEventData eventData)
        {
            PointerThroughUtility.ExecuteThrough(gameObject, eventData, ExecuteEvents.dropHandler);
        }

        public void OnScroll(PointerEventData eventData)
        {
            PointerThroughUtility.ExecuteThrough(gameObject, eventData, ExecuteEvents.scrollHandler);
        }

        private void OnDisable()
        {
            m_EnteredTarget = null;
        }
    }
}
