using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    /// <summary>
    /// 滚动事件穿透
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PointerThroughScroll : MonoBehaviour, IScrollHandler
    {
        public void OnScroll(PointerEventData eventData)
        {
            PointerThroughUtility.ExecuteThrough(gameObject, eventData, ExecuteEvents.scrollHandler);
        }
    }
}
