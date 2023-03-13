using UnityEngine;
using UnityEngine.EventSystems;

namespace QFSW.QC.UI
{
    [DisallowMultipleComponent]
    public class ResizableUI : MonoBehaviour, IDragHandler
    {
        [SerializeField] private RectTransform _resizeRoot = null;
        [SerializeField] private Canvas _resizeCanvas = null;

        [SerializeField] private bool _lockInScreen = true;
        [SerializeField] private Vector2 _minSize = new Vector2();

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 minBounds = (_resizeRoot.offsetMin + _minSize) * _resizeCanvas.scaleFactor;
            Vector2 maxBounds = _lockInScreen
                ? new Vector2(Screen.width, Screen.height)
                : new Vector2(Mathf.Infinity, Mathf.Infinity);

            Vector2 delta = eventData.delta;
            Vector2 posCurrent = eventData.position;
            Vector2 posLast = posCurrent - delta;

            Vector2 posCurrentBounded = new Vector2(
                Mathf.Clamp(posCurrent.x, minBounds.x, maxBounds.x),
                Mathf.Clamp(posCurrent.y, minBounds.y, maxBounds.y)
            );

            Vector2 posLastBounded = new Vector2(
                Mathf.Clamp(posLast.x, minBounds.x, maxBounds.x),
                Mathf.Clamp(posLast.y, minBounds.y, maxBounds.y)
            );

            Vector2 deltaBounded = posCurrentBounded - posLastBounded;

            _resizeRoot.offsetMax += deltaBounded / _resizeCanvas.scaleFactor;
        }
    }
}
