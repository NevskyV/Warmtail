using System;
using UnityEngine;
using UnityEngine.UI;

namespace Entities.UI.SDF
{
    [ExecuteAlways]
    public class SdfSlider : Selectable
    {
        [SerializeField, Range(0,1f)] private float _value;
        [SerializeField, Range(0,10f)] private float _speed;
        [SerializeField] private RectTransform _backRect;
        [SerializeField] private RectTransform _fillRect;
        [SerializeField] private RectTransform _handleRect;
        [SerializeField] private Image _sdfImage;

        private void Update()
        {
            _fillRect.anchorMax = new Vector2(Mathf.Lerp(_fillRect.anchorMax.x, _value *
                _backRect.anchorMax.x, Time.deltaTime * _speed), _fillRect.anchorMax.y);
            _handleRect.localPosition = new Vector2(Mathf.Lerp(_handleRect.localPosition.x, _value *
                _backRect.rect.width - _backRect.rect.width / 2 - _handleRect.rect.width/8, Time.deltaTime * _speed),_handleRect.localPosition.y);
        }
    }
}