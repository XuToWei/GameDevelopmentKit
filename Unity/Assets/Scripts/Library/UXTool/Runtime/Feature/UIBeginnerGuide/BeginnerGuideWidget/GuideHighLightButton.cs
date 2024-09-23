using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GuideHighLightButton : Button
{
    private List<RaycastResult> m_RaycastResults;
    private bool m_ClickPassThrough;
    /// <summary>
    /// 是否点击穿透
    /// </summary>
    public bool clickPassThrough
    {
        set
        {
            if (value && m_RaycastResults == null)
            {
                m_RaycastResults = new List<RaycastResult>();
            }
            m_ClickPassThrough = value;
        }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (m_ClickPassThrough && eventData.button == PointerEventData.InputButton.Left && IsActive() && IsInteractable())
        {
            EventSystem.current.RaycastAll(eventData, m_RaycastResults);
            var current = eventData.pointerCurrentRaycast.gameObject;
            //排除自己和自己的父节点
            foreach (var t in m_RaycastResults)
            {
                if (current != t.gameObject && current.transform.parent != t.gameObject.transform)
                {
                    //RaycastAll后ugui会自己排序，如果你只想响应透下去的最近的一个响应，这里ExecuteEvents.Execute后直接break就行。
                    ExecuteEvents.ExecuteHierarchy(t.gameObject, eventData, ExecuteEvents.pointerClickHandler);
                    break;
                }
            }
            m_RaycastResults.Clear();
        }
        base.OnPointerClick(eventData);
    }
}