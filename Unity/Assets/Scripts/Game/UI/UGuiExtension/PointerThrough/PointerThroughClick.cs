using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    /// <summary>
    /// 点击事件穿透
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PointerThroughClick : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            PointerThroughUtility.ExecuteThrough(gameObject, eventData, ExecuteEvents.pointerClickHandler);
        }
    }
}
