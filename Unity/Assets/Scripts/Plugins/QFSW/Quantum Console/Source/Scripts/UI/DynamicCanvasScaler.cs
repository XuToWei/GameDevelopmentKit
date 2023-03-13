using UnityEngine;
using UnityEngine.UI;

namespace QFSW.QC.UI
{
    [ExecuteInEditMode]
    public class DynamicCanvasScaler : MonoBehaviour
    {
        public float RectMagnification
        {
            get => _rectMagnification;
            set
            {
                if (value > 0)
                {
                    _rectMagnification = value;
                }
            }
        }
        [Range(0.5f, 2f)]
        [SerializeField] private float _rectMagnification = 1f;

        public float ZoomMagnification
        {
            get => _zoomMagnification;
            set
            {
                if (value > 0)
                {
                    _zoomMagnification = value;
                }
            }
        }
        [Range(0.5f, 2f)]
        [SerializeField] private float _zoomMagnification = 1f;

        [SerializeField] private CanvasScaler _scaler = null;
        [SerializeField] private RectTransform _uiRoot = null;
        [SerializeField] private Vector2 _referenceResolution = new Vector2(1920, 1080);

        private float RootScaler => _rectMagnification / _zoomMagnification;

        private float _lastScaler;

        private void OnEnable()
        {
            _lastScaler = RootScaler;
        }

        private void Update()
        {
            if (_scaler && _uiRoot)
            {
                if (RootScaler != _lastScaler)
                {
                    Rect rootRect = new Rect(_uiRoot.offsetMin.x / _lastScaler, _uiRoot.offsetMin.y / _lastScaler, _uiRoot.offsetMax.x / _lastScaler, _uiRoot.offsetMax.y / _lastScaler);
                    _lastScaler = RootScaler;

                    _scaler.referenceResolution = _referenceResolution / _zoomMagnification;
                    _uiRoot.offsetMin = new Vector2(rootRect.x, rootRect.y) * RootScaler;
                    _uiRoot.offsetMax = new Vector2(rootRect.width, rootRect.height) * RootScaler;

#if UNITY_EDITOR
                    UnityEditor.EditorUtility.SetDirty(_uiRoot);
                    UnityEditor.EditorUtility.SetDirty(_scaler);
#endif
                }
            }
        }
    }
}
