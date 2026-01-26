using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    /// <summary>
    /// UI指针事件穿透工具类
    /// </summary>
    public static class PointerThroughUtility
    {
        private static readonly List<RaycastResult> s_RaycastResults = new List<RaycastResult>();

        /// <summary>
        /// 尝试获取穿透目标
        /// </summary>
        public static bool TryGetThroughTarget(GameObject current, PointerEventData eventData, out GameObject target)
        {
            target = null;
            if (EventSystem.current == null)
            {
                return false;
            }
            s_RaycastResults.Clear();
            EventSystem.current.RaycastAll(eventData, s_RaycastResults);
            var raycastTarget = eventData.pointerCurrentRaycast.gameObject;
            if (raycastTarget == null)
            {
                raycastTarget = current;
            }
            bool isAfterSelf = false;
            foreach (var result in s_RaycastResults)
            {
                if (!isAfterSelf && raycastTarget == result.gameObject)
                {
                    isAfterSelf = true;
                    continue;
                }
                if (isAfterSelf && !raycastTarget.transform.IsChildOf(result.gameObject.transform) && !result.gameObject.transform.IsChildOf(raycastTarget.transform))
                {
                    target = result.gameObject;
                    break;
                }
            }
            s_RaycastResults.Clear();
            return target != null;
        }

        /// <summary>
        /// 执行穿透事件
        /// </summary>
        public static void ExecuteThrough<T>(GameObject current, PointerEventData eventData, ExecuteEvents.EventFunction<T> functor) where T : IEventSystemHandler
        {
            if (TryGetThroughTarget(current, eventData, out var target))
            {
                ExecuteEvents.ExecuteHierarchy(target, eventData, functor);
            }
        }
    }
}
