using System;
using DG.Tweening;
using Systems;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Entities.UI.SDF
{
    [ExecuteAlways]
    public class SdfSlider : MonoBehaviour
    {
        [SerializeField, Range(0, 1f)] private float _value;
        public float Value
        {
            get => _value;
            set
            {
                _value = Mathf.Clamp(value, 0f, 1f);
                _gamepadRumble.ShortRumble();
                OnValueChange?.Invoke(_value);
            }
        }

        [SerializeField, Range(0,10f)] private float _speed;
        [SerializeField, Range(0,10f)] private float _onSelectScale = 1.2f;
        [SerializeField] private RectTransform _backRect;
        [SerializeField] private RectTransform _fillRect;
        [SerializeField] private RectTransform _handleRect;
        [SerializeField] private Image _sdfImage;
        [Inject] private GamepadRumble _gamepadRumble;
        public Action<float> OnValueChange;
        
        private void Update()
        {
            _fillRect.anchorMax = new Vector2(Mathf.Lerp(_fillRect.anchorMax.x, _value *
                _backRect.anchorMax.x, Time.deltaTime * _speed), _fillRect.anchorMax.y);
            _handleRect.localPosition = new Vector2(Mathf.Lerp(_handleRect.localPosition.x, _value *
                _backRect.rect.width - _backRect.rect.width / 2 - _handleRect.rect.width/8, Time.deltaTime * _speed),_handleRect.localPosition.y);
        }
    }
}