using System;
using CodeBind;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game
{
    [CodeBindName("ExButton")]
    public class ExButton : Button
    {
        public static event Action AllButtonOnPointerDownEvent;
        
        [Serializable]
        public class ButtonOnPointerDownEvent : UnityEvent {}
        
        [FormerlySerializedAs("onPointerDown")]
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

