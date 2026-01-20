using System;
using Entities.Core;
using Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace Systems.Tasks
{
    public class ButtonTask : ITask
    {
        [SerializeField] private string _buttonId;
        public bool Completed { get; set; }
        public Action OnComplete { get; set; }
        
        private Button _button;

        public void Activate()
        {
            _button = SavableObjectsResolver.FindObjectById<Button>(_buttonId);
            if (_button != null)
            {
                _button.onClick.AddListener(MarkComplete);
            }
            else
            {
                Debug.LogWarning($"ButtonTask: Button with id '{_buttonId}' not found");
            }
        }

        private void MarkComplete()
        {
            Completed = true;
            OnComplete?.Invoke();
            _button.onClick.RemoveListener(MarkComplete);
        }
    }
}