using System;
using Entities.Core;
using Entities.UI;
using Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Systems.Tasks
{
    public class KeyPressTask : ITask
    {
        public bool Completed { get; set; }
        public Action OnComplete { get; set; }
        [SerializeField] private InputActionAsset _reference;
        [SerializeField] private string _actionName;
        [SerializeField] private string _tipsVisuals;
        [TextArea] [SerializeField] private string _description;
        
        private TipsVisuals _tips;

        public void Activate()
        {
            Completed = false;
            SavableObjectsResolver.FindObjectById<TipsVisuals>(_tipsVisuals).ShowTip(_reference[_actionName]);
            _reference[_actionName].started += MarkComplete;
        }

        private void MarkComplete(InputAction.CallbackContext _)
        {
            Completed = true;
            OnComplete?.Invoke();
            _reference[_actionName].started -= MarkComplete;
        }
    }
}