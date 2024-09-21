using UnityEngine;
using UnityEngine.UI;

namespace StateController
{
    [RequireComponent(typeof(UXTextMeshPro))]
    public class StateUXTextMeshProForLocalization : BaseSelectableState<LocalizationData>
    {
        private UXTextMeshPro m_UXTextMeshPro;
        
        protected override void OnStateInit()
        {
            m_UXTextMeshPro = GetComponent<UXTextMeshPro>();
        }

        protected override void OnStateChanged(LocalizationData stateData)
        {
            if (stateData.EnableLocalization)
            {
                m_UXTextMeshPro.ignoreLocalization = false;
                if (m_UXTextMeshPro.localizationType == LocalizationHelper.TextLocalizationType.RuntimeUse)
                {
                    m_UXTextMeshPro.localizationID = stateData.LocalizationKey;
                }
                m_UXTextMeshPro.previewID = stateData.LocalizationKey;
            }
            else
            {
                m_UXTextMeshPro.ignoreLocalization = true;
            }
        }
    }
}
