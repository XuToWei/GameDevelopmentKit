using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace QFSW.QC.UI
{
    [ExecuteInEditMode]
    public class ZoomUIController : MonoBehaviour
    {
        [SerializeField] private float _zoomIncrement = 0.1f;
        [SerializeField] private float _minZoom = 0.1f;
        [SerializeField] private float _maxZoom = 2f;

        [SerializeField] private Button _zoomDownBtn = null;
        [SerializeField] private Button _zoomUpBtn = null;

        [SerializeField] private DynamicCanvasScaler _scaler = null;
        [SerializeField] private QuantumConsole _quantumConsole = null;
        [SerializeField] private TextMeshProUGUI _text = null;

        private float _lastZoom = -1;

        private float ClampAndSnapZoom(float zoom)
        {
            float clampedZoom = Mathf.Min(_maxZoom, Mathf.Max(_minZoom, zoom));
            float snappedZoom = Mathf.Round(clampedZoom / _zoomIncrement) * _zoomIncrement;
            return snappedZoom;
        }

        public void ZoomUp()
        {
            _scaler.ZoomMagnification = ClampAndSnapZoom(_scaler.ZoomMagnification + _zoomIncrement);
        }

        public void ZoomDown()
        {
            _scaler.ZoomMagnification = ClampAndSnapZoom(_scaler.ZoomMagnification - _zoomIncrement);
        }

        private void Update()
        {
            if (_quantumConsole && _quantumConsole.KeyConfig)
            {
                if (_quantumConsole.KeyConfig.ZoomInKey.IsPressed()) { ZoomUp(); }
                if (_quantumConsole.KeyConfig.ZoomOutKey.IsPressed()) { ZoomDown(); }
            }
        }

        private void LateUpdate()
        {
            if (_scaler && _text)
            {
                float zoom = _scaler.ZoomMagnification;
                if (zoom != _lastZoom)
                {
                    _lastZoom = zoom;

                    int percentage = Mathf.RoundToInt(100 * zoom);
                    _text.text = $"{percentage}%";
                }
            }

            if (_zoomDownBtn)
            {
                _zoomDownBtn.interactable = _lastZoom > _minZoom;
            }

            if (_zoomUpBtn)
            {
                _zoomUpBtn.interactable = _lastZoom < _maxZoom;
            }
        }
    }
}
