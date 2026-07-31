using UnityEngine;

namespace StateController
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public sealed class RectTransformAnchorBinding : StateValueBinding<AnchorValue>
    {
        private RectTransform m_TargetRectTransform;

        protected override void InitializeTarget()
        {
            m_TargetRectTransform = GetComponent<RectTransform>();
        }

        protected override void ApplyValue(AnchorValue value)
        {
            m_TargetRectTransform.anchorMin = value.AnchorMin;
            m_TargetRectTransform.anchorMax = value.AnchorMax;
        }
    }
}
