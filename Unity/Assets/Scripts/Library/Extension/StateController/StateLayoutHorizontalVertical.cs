using UnityEngine;

namespace StateController
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public sealed class StateRectTransformForAnchor : BaseSelectableState<AnchorData>
    {
        private RectTransform m_RectTransform;
        
        protected override void OnStateInit()
        {
            m_RectTransform = GetComponent<RectTransform>();
        }

        protected override void OnStateChanged(AnchorData stateData)
        {
            m_RectTransform.anchorMin = stateData.AnchorMin;
            m_RectTransform.anchorMax = stateData.AnchorMax;
        }
    }
}
