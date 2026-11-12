using System;
using CodeBind;
using ReplaceComponent;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game
{
    [ReplaceComponent(typeof(Button))]
    [BindingTargetToken("ExButton")]
    public class ExButton : Button
    {
        public static event Action AllButtonOnPointerDownEvent;

        [Serializable]
        public class ButtonOnPointerDownEvent : UnityEvent {}

        [SerializeField]
        private ButtonOnPointerDownEvent m_OnPointerDown = new ButtonOnPointerDownEvent();

#pragma warning disable ET0502
        public ButtonOnPointerDownEvent onPointerDown
        {
            get => m_OnPointerDown;
            set => m_OnPointerDown = value;
        }
#pragma warning restore ET0502

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

