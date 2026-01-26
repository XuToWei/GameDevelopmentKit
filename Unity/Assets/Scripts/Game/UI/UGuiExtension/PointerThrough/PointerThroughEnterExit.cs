using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    /// <summary>
    /// 指针进入和离开事件穿透
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PointerThroughEnterExit : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
    {
        private GameObject m_EnteredTarget;

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

        public void OnPointerMove(PointerEventData eventData)
        {
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

        private void OnDisable()
        {
            m_EnteredTarget = null;
        }
    }
}
