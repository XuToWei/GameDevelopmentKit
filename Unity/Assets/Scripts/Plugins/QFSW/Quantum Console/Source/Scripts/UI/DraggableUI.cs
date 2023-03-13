using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace QFSW.QC.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class DraggableUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform _dragRoot = null;
        [SerializeField] private QuantumConsole _quantumConsole = null;
        [SerializeField] private bool _lockInScreen = true;

        [SerializeField] private UnityEvent _onBeginDrag = null;
        [SerializeField] private UnityEvent _onDrag = null;
        [SerializeField] private UnityEvent _onEndDrag = null;

        private Vector2 _lastPos;
        private bool _isDragging = false;

        public void OnPointerDown(PointerEventData eventData)
        {
            _isDragging =
                _quantumConsole &&
                _quantumConsole.KeyConfig &&
                _quantumConsole.KeyConfig.DragConsoleKey.IsHeld();

            if (_isDragging)
            {
                _onBeginDrag.Invoke();
                _lastPos = eventData.position;
            }
        }

        public void LateUpdate()
        {
            if (_isDragging)
            {
                Transform root = _dragRoot;
                if (!root) { root = transform as RectTransform; }

                Vector2 pos = InputHelper.GetMousePosition();
                Vector2 delta = pos - _lastPos;
                _lastPos = pos;

                if (_lockInScreen)
                {
                    Vector2 resolution = new Vector2(Screen.width, Screen.height);
                    if (pos.x <= 0 || pos.x >= resolution.x) { delta.x = 0; }
                    if (pos.y <= 0 || pos.y >= resolution.y) { delta.y = 0; }
                }

                root.Translate(delta);
                _onDrag.Invoke();
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_isDragging)
            {
                _isDragging = false;
                _onEndDrag.Invoke();
            }
        }
    }
}
