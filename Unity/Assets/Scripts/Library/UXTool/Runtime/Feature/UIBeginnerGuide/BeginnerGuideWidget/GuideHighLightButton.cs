using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GuideHighLightButton : Button
{
    private static readonly List<RaycastResult> s_RaycastResults = new List<RaycastResult>();

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && IsActive() && IsInteractable())
        {
            EventSystem.current.RaycastAll(eventData, s_RaycastResults);
            var current = eventData.pointerCurrentRaycast.gameObject;
            //排除自己和自己的父节点
            bool isAfterSelf = false;
            foreach (var t in s_RaycastResults)
            {
                if (!isAfterSelf && current == t.gameObject)
                {
                    isAfterSelf = true;
                    continue;
                }
                if (isAfterSelf && !current.transform.IsChildOf(t.gameObject.transform) && !t.gameObject.transform.IsChildOf(current.transform))
                {
                    //RaycastAll后ugui会自己排序，如果你只想响应透下去的最近的一个响应，这里ExecuteEvents.Execute后直接break就行。
                    ExecuteEvents.ExecuteHierarchy(t.gameObject, eventData, ExecuteEvents.pointerClickHandler);
                    break;
                }
            }
            s_RaycastResults.Clear();
        }
        base.OnPointerClick(eventData);
    }
}