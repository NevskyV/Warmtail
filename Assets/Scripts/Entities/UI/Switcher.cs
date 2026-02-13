using System.Collections.Generic;
using Entities.Localization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Entities.UI
{
    public class Switcher : MonoBehaviour
    {
        [SerializeField] private LocalizedText _localizedText;
        [SerializeField] private List<string> _id = new();
        [SerializeField] private Color _activeColor;
        [SerializeField] private Color _deActiveColor;
        [SerializeField] private Transform _switchesParent;
        public UnityEvent<int> Event { get; set; } = new();
        public int CurrentValue { get; set; }
        private List<Image> _images = new();

        private void Start()
        {
            for (int i = 0; i < _switchesParent.childCount; i++)
            {
                _images.Add(_switchesParent.GetChild(i).GetComponent<Image>());
            }
            Switch(CurrentValue);
        }

        public void SwitchNext()
        {
            var newValue = CurrentValue + 1;
            if (newValue == _images.Count) newValue = 0;
            Event.Invoke(newValue);
            Switch(newValue);
        }
        
        public void SwitchPrev()
        {
            var newValue = CurrentValue - 1;
            if (newValue < 0) newValue = _images.Count - 1;
            Event.Invoke(newValue);
            Switch(newValue);
        }
        
        public virtual void Switch(int value)
        {
            _images[CurrentValue].color = _deActiveColor;
            CurrentValue = value;
            _images[CurrentValue].color = _activeColor;
            _localizedText.SetNewKey(_id[value]);
        }
    }
}