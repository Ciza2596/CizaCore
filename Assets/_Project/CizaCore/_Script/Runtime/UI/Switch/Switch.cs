using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CizaCore.UI
{
    public class Switch : MonoBehaviour
    {
        [SerializeField]
        private bool _isDefaultTurnOn;

        [SerializeField]
        private float _moveSpeed = 450;

        [Space]
        [SerializeField]
        private UnityEvent<bool> _onIsOnChanged;

        [Space]
        [SerializeField]
        private ColorSettings _colorSettings;

        [SerializeField]
        private MonoSettings _monoSettings;

        private float _offX;
        private float _targetX;

        public event Action<bool> OnIsOnChanged;

        public bool IsOn => _monoSettings.Toggle.isOn;

        public void TurnOn() =>
            _monoSettings.Toggle.isOn = true;

        public void TurnOff() =>
            _monoSettings.Toggle.isOn = false;

        private void Awake()
        {
            _monoSettings.Toggle.onValueChanged.AddListener(OnValueChanged);
            _offX = _monoSettings.Handler.localPosition.x;

            if (_isDefaultTurnOn)
                TurnOn();
            else
                TurnOff();
        }

        private void Update()
        {
            var direction = _targetX > 0 ? 1 : -1;
            var handlerLocalPosition = _monoSettings.Handler.localPosition;
            if (direction > 0 && handlerLocalPosition.x >= _targetX || (direction < 0 && handlerLocalPosition.x <= _targetX))
                return;

            _monoSettings.Handler.localPosition = handlerLocalPosition + new Vector3(Time.deltaTime * direction * _moveSpeed, 0);
        }

        private void OnValueChanged(bool isOn)
        {
            if (isOn)
            {
                _targetX = -_offX;
                SetImagesColor(_colorSettings.TurnOnColor);
            }
            else
            {
                _targetX = _offX;
                SetImagesColor(_colorSettings.TurnOffColor);
            }

            OnIsOnChanged?.Invoke(isOn);
            _onIsOnChanged?.Invoke(isOn);
        }

        private void SetImagesColor(Color color)
        {
            if (!_colorSettings.IsChangeColor)
                return;

            foreach (var image in _monoSettings.Backgrounds)
                image.color = color;
        }

        [Serializable]
        private class ColorSettings
        {
            [SerializeField]
            private bool _isChangeColor = true;

            [Space]
            [SerializeField]
            private Color _turnOnColor = Color.green;

            [SerializeField]
            private Color _turnOffColor = Color.gray;

            public bool IsChangeColor => _isChangeColor;

            public Color TurnOnColor => _turnOnColor;
            public Color TurnOffColor => _turnOffColor;
        }

        [Serializable]
        private class MonoSettings
        {
            [SerializeField]
            private Toggle _toggle;

            [Space]
            [SerializeField]
            private RectTransform _handler;

            [SerializeField]
            private Image[] _backgrounds;

            public Toggle Toggle => _toggle;

            public RectTransform Handler => _handler;

            public Image[] Backgrounds => _backgrounds != null ? _backgrounds.ToArray() : Array.Empty<Image>();
        }
    }
}