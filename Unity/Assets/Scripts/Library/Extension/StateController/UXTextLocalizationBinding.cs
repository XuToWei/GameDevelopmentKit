using UnityEngine;
using UnityEngine.UI;

namespace StateController
{
    [RequireComponent(typeof(UXText))]
    public sealed class UXTextLocalizationBinding : StateValueBinding<LocalizationValue>
    {
        private UXText m_TargetUXText;

        protected override void InitializeTarget()
        {
            m_TargetUXText = GetComponent<UXText>();
        }

        protected override void ApplyValue(LocalizationValue value)
        {
            if (value.EnableLocalization)
            {
                m_TargetUXText.ignoreLocalization = false;
                if (m_TargetUXText.localizationType == LocalizationHelper.TextLocalizationType.RuntimeUse)
                {
                    m_TargetUXText.localizationID = value.LocalizationKey;
                }
                m_TargetUXText.previewID = value.LocalizationKey;
            }
            else
            {
                m_TargetUXText.ignoreLocalization = true;
            }
        }
    }
}
