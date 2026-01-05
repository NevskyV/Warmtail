using UnityEngine;
using Interfaces;
using System;
using Data;
using Entities.Triggers;
using Entities.Core;
using Entities.Probs;
using UnityEngine.Events;

namespace Systems.Tasks
{
    public class InteractTask : ITask
    {
        public bool Completed { get; set; }
        public Action OnComplete { get; set; }
        private UnityEventTrigger _trigger;
        [SerializeField] private string _triggerId;

        public void Activate()
        {
            _trigger = SavableObjectsResolver.FindObjectById<UnityEventTrigger>(_triggerId);
            _trigger._event.AddListener(MarkComplete);
        }

        private void MarkComplete()
        {
            Completed = true;
            OnComplete?.Invoke();
            if (_trigger) _trigger._event.RemoveListener(MarkComplete);
        }
    }
}