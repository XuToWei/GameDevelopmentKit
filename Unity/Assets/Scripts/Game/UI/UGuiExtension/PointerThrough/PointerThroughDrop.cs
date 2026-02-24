using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    /// <summary>
    /// 放置事件穿透
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PointerThroughDrop : MonoBehaviour, IDropHandler
    {
        public void OnDrop(PointerEventData eventData)
        {
            PointerThroughUtility.ExecuteThrough(gameObject, eventData, ExecuteEvents.dropHandler);
        }
    }
}
