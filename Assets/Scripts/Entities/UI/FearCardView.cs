using System;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Entities.UI
{
    public class FearCardView : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private TMP_Text _statusText;
        [SerializeField] private Image _background;
        [SerializeField] private Color _activeColor = Color.white;
        [SerializeField] private Color _inactiveColor = Color.gray;
        [SerializeField] private Button _button;

        private FearConfig _config;
        private Action<FearConfig> _onClick;

        public void Initialize(FearConfig config, Action<FearConfig> onClick)
        {
            _config = config;
            _onClick = onClick;

            if (_icon) _icon.sprite = config.Icon;
            if (_nameText) _nameText.text = config.Name;
            if (_descriptionText) _descriptionText.text = config.Description;

            SetActive(false);
            if (_button)
            {
                _button.onClick.RemoveListener(HandleClick);
                _button.onClick.AddListener(HandleClick);
            }
        }

        public void SetActive(bool active)
        {
            if (_statusText) _statusText.text = active ? "активно" : string.Empty;
            if (_background) _background.color = active ? _activeColor : _inactiveColor;
        }

        private void HandleClick()
        {
            _onClick?.Invoke(_config);
        }
    }
}
