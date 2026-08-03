using UnityEngine;
using UnityEngine.UI;

namespace StateController
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Button))]
    public class ButtonInteractableBinding : StateValueBinding<bool>
    {
        private Button m_TargetButton;

        protected override void InitializeTarget()
        {
            m_TargetButton = GetComponent<Button>();
        }

        protected override void ApplyValue(bool value)
        {
            m_TargetButton.interactable = value;
        }
    }
}
