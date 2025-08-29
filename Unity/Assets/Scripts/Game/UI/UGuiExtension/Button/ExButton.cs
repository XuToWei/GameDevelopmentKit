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
    [CodeBindName("ExButton")]
    public class ExButton : Button
    {
        public static event Action AllButtonOnPointerDownEvent;

        [Serializable]
        public class ButtonOnPointerDownEvent : UnityEvent {}

        [SerializeField]
        private ButtonOnPointerDownEvent m_OnPointerDown = new ButtonOnPointerDownEvent();

        [IgnorePropertyDeclaration]
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

