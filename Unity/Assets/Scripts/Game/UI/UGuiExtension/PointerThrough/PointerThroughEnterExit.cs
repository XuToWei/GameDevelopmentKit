using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    /// <summary>
    /// 指针进入和离开事件穿透
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PointerThroughEnterExit : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            PointerThroughUtility.ExecuteThrough(gameObject, eventData, ExecuteEvents.pointerEnterHandler);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            PointerThroughUtility.ExecuteThrough(gameObject, eventData, ExecuteEvents.pointerExitHandler);
        }
    }
}
