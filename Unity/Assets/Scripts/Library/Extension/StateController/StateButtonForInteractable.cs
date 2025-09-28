using UnityEngine;
using UnityEngine.UI;

namespace StateController
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Button))]
    public class StateButtonForInteractable : BaseSelectableState<bool>
    {
        private Button m_Button;
        
        protected override void OnStateInit()
        {
            m_Button = GetComponent<Button>();
        }

        protected override void OnStateChanged(bool interactable)
        {
            m_Button.interactable = interactable;
        }
    }
}
