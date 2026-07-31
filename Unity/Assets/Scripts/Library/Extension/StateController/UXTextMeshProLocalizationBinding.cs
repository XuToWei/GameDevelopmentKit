using UnityEngine;
using UnityEngine.UI;

namespace StateController
{
    [RequireComponent(typeof(UXTextMeshPro))]
    public class UXTextMeshProLocalizationBinding : StateValueBinding<LocalizationValue>
    {
        private UXTextMeshPro m_TargetUXTextMeshPro;

        protected override void InitializeTarget()
        {
            m_TargetUXTextMeshPro = GetComponent<UXTextMeshPro>();
        }

        protected override void ApplyValue(LocalizationValue value)
        {
            if (value.EnableLocalization)
            {
                m_TargetUXTextMeshPro.ignoreLocalization = false;
                if (m_TargetUXTextMeshPro.localizationType == LocalizationHelper.TextLocalizationType.RuntimeUse)
                {
                    m_TargetUXTextMeshPro.localizationID = value.LocalizationKey;
                }
                m_TargetUXTextMeshPro.previewID = value.LocalizationKey;
            }
            else
            {
                m_TargetUXTextMeshPro.ignoreLocalization = true;
            }
        }
    }
}
