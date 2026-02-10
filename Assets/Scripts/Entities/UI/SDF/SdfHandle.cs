using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using Zenject;

namespace Entities.UI.SDF
{
    public class SdfHandle : Selectable
    {
        [SerializeField, Range(0,10f)] private float _onSelectScale = 1.2f;
        [SerializeField, Range(0,10f)] private float _moveFactor = 1f;
        [SerializeField] private SdfSlider _slider;
        private RectTransform _handleRect;
        private RectTransform _sliderRect;
        private Vector2 _normalSize;
        private InputSystemUIInputModule _uiInputModule;
        private bool _isSelected;

        [Inject]
        public void Construct(InputSystemUIInputModule uiInputModule)
        {
            _uiInputModule = uiInputModule;
            _sliderRect = _slider.GetComponent<RectTransform>();
        }
            
        protected override void Start()
        {
            _handleRect = GetComponent<RectTransform>();
            _normalSize = new Vector2(_handleRect.rect.width, _handleRect.rect.height);
        }
        
        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            _isSelected = true;
            _handleRect.DOSizeDelta(_normalSize * _onSelectScale, 0.5f);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            _isSelected = false;
            _handleRect.DOSizeDelta(_normalSize, 0.5f);
        }

        private void Update()
        {
            if (_isSelected && _uiInputModule.leftClick.action.IsPressed())
            {
                _slider.Value  = (_uiInputModule.point.action.ReadValue<Vector2>().x - _sliderRect.position.x
                                     ) / _sliderRect.sizeDelta.x;
            }
        }

        public override void OnMove(AxisEventData eventData)
        {
            base.OnMove(eventData);
            _slider.Value += _uiInputModule.move.action.ReadValue<Vector2>().x * _moveFactor;
        }
        
        
    }
}