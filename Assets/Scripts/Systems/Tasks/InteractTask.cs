using UnityEngine;
using Interfaces;
using Data;
using System;
using Entities.Triggers;
using Entities.Core;

namespace Systems.Tasks
{
    public class InteractTask : ITask
    {
        public bool Completed { get; set; }
        public Action OnComplete { get; set; }
        private UnityEventTrigger _trigger;
        [SerializeField] private string _triggerId; 
        private EventsData _eventsData;

        public void SetEventsData(EventsData data)
        {
            _eventsData = data;
        }


        public void Activate()
        {
            _trigger = _eventsData.SceneObjects[_triggerId].GetComponent<UnityEventTrigger>();
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