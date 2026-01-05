using System;
using Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Systems.Tasks
{
    public class KeyPressTask : ITask
    {
        public bool Completed { get; set; }
        public Action OnComplete { get; set; }
        [SerializeField] private InputActionAsset _asset;
        [SerializeReference] private string _action;

        public void Activate()
        {
            _asset[_action].started += MarkComplete;
        }

        private void MarkComplete(InputAction.CallbackContext _)
        {
            Completed = true;
            OnComplete?.Invoke();
            _asset[_action].started -= MarkComplete;
        }
    }
}