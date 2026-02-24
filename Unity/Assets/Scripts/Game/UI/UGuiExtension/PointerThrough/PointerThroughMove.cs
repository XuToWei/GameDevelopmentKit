using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    /// <summary>
    /// 指针移动事件穿透
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PointerThroughMove : MonoBehaviour, IPointerMoveHandler
    {
        public void OnPointerMove(PointerEventData eventData)
        {
            PointerThroughUtility.ExecuteThrough(gameObject, eventData, ExecuteEvents.pointerMoveHandler);
        }
    }
}
