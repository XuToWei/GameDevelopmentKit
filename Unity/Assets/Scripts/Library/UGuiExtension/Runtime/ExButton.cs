using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
    public class ExButton : Button
    {
        public static event Action AllButtonOnPointerDownEvent;
        
        [Serializable]
        public class ButtonOnPointerDownEvent : UnityEvent {}
        
        [FormerlySerializedAs("onClick")]
        [SerializeField]
        private ButtonOnPointerDownEvent m_OnPointerDown = new ButtonOnPointerDownEvent();

        public ButtonOnPointerDownEvent onPointerDown
        {
            get => m_OnPointerDown;
            set => m_OnPointerDown = value;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            if (!IsActive() || !IsInteractable())
                return;
            if (AllButtonOnPointerDownEvent != null)
            {
                AllButtonOnPointerDownEvent.Invoke();
            }
            onPointerDown.Invoke();
        }
    }
}

