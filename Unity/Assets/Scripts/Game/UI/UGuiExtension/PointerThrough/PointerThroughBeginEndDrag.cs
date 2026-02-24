using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    /// <summary>
    /// 开始和结束拖拽事件穿透
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PointerThroughBeginEndDrag : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        public void OnBeginDrag(PointerEventData eventData)
        {
            PointerThroughUtility.ExecuteThrough(gameObject, eventData, ExecuteEvents.beginDragHandler);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            PointerThroughUtility.ExecuteThrough(gameObject, eventData, ExecuteEvents.endDragHandler);
        }
    }
}
