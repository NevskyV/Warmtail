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
        [TextArea] [SerializeField] private string description;

        public void Activate()
        {
            Completed = false;
            _asset[_action].started += MarkComplete;
        }

        private void MarkComplete(InputAction.CallbackContext _)
        {
            Debug.Log(_action);
            Completed = true;
            OnComplete?.Invoke();
            _asset[_action].started -= MarkComplete;
        }
    }
}