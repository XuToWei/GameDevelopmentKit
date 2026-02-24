using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    /// <summary>
    /// 拖拽事件穿透
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PointerThroughDrag : MonoBehaviour, IDragHandler
    {
        public void OnDrag(PointerEventData eventData)
        {
            PointerThroughUtility.ExecuteThrough(gameObject, eventData, ExecuteEvents.dragHandler);
        }
    }
}
