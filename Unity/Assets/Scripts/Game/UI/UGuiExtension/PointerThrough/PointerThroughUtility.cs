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
            var raycast = eventData.pointerCurrentRaycast.gameObject;
            if (raycast == null)
            {
                raycast = current;
            }
            bool isAfterSelf = false;
            foreach (var result in s_RaycastResults)
            {
                var resultGameObject = result.gameObject;
                if (!isAfterSelf && raycast == resultGameObject)
                {
                    isAfterSelf = true;
                    continue;
                }
                if (isAfterSelf)
                {
                    var resultTransform = resultGameObject.transform;
                    var raycastTransform = raycast.transform;
                    if (!raycastTransform.IsChildOf(resultTransform) && !resultTransform.IsChildOf(raycastTransform))
                    {
                        target = resultGameObject;
                        break;
                    }
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

        /// <summary>
        /// 检查指针是否在目标RectTransform区域内
        /// </summary>
        public static bool IsPointerInsideTarget(GameObject target, PointerEventData eventData)
        {
            if (target == null)
            {
                return false;
            }
            var rectTransform = target.GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                return false;
            }
            return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, eventData.position, eventData.enterEventCamera);
        }
    }
}
