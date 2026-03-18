using System;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Entities.UI
{
    public class FearCardView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private TMP_Text _activeText;
        [SerializeField] private GameObject _collectedMark;

        [Header("Colors")]
        [SerializeField] private Color _inactiveColor = Color.white;
        [SerializeField] private Color _activeColor = new Color(0.6f, 1f, 0.6f, 1f);
        [SerializeField] private Image _tintTarget;

        private Action _onClick;

        public void Bind(FearConfig config, bool collected, bool active, Action onClick)
        {
            _onClick = onClick;

            if (_icon != null) _icon.sprite = config != null ? config.Icon : null;
            if (_nameText != null) _nameText.text = config != null ? config.Name : string.Empty;
            if (_descriptionText != null) _descriptionText.text = config != null ? config.Description : string.Empty;
            if (_collectedMark != null) _collectedMark.SetActive(collected);

            SetActive(active);

            if (_button != null)
            {
                _button.onClick.RemoveListener(OnClickInternal);
                _button.onClick.AddListener(OnClickInternal);
            }
        }

        public void SetActive(bool active)
        {
            if (_activeText != null) _activeText.gameObject.SetActive(active);
            if (_activeText != null && active) _activeText.text = "активно";

            var target = _tintTarget != null ? _tintTarget : GetComponent<Image>();
            if (target != null) target.color = active ? _activeColor : _inactiveColor;
        }

        private void OnClickInternal()
        {
            _onClick?.Invoke();
        }
    }
}

