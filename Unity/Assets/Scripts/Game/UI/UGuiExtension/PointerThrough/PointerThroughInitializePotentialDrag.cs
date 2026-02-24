using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    /// <summary>
    /// 初始化潜在拖拽事件穿透
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PointerThroughInitializePotentialDrag : MonoBehaviour, IInitializePotentialDragHandler
    {
        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            PointerThroughUtility.ExecuteThrough(gameObject, eventData, ExecuteEvents.initializePotentialDrag);
        }
    }
}
