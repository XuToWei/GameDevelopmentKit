using UnityEngine;
using UnityEngine.UI;

namespace StateController
{
    [RequireComponent(typeof(UXText))]
    public sealed class StateUXTextForLocalization : BaseSelectableState<LocalizationData>
    {
        private UXText m_UXText;
        
        protected override void OnStateInit()
        {
            m_UXText = GetComponent<UXText>();
        }

        protected override void OnStateChanged(LocalizationData stateData)
        {
            if (stateData.EnableLocalization)
            {
                m_UXText.ignoreLocalization = false;
                if (m_UXText.localizationType == LocalizationHelper.TextLocalizationType.RuntimeUse)
                {
                    m_UXText.localizationID = stateData.LocalizationKey;
                }
                m_UXText.previewID = stateData.LocalizationKey;
            }
            else
            {
                m_UXText.ignoreLocalization = true;
            }
        }
    }
}