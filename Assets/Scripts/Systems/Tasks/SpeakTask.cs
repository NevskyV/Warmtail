using System;
using Interfaces;
using Data;
using UnityEngine;

namespace Systems.Tasks
{
    public class SpeakTask : ITask
    {
        public bool Completed { get; set; }
        public Action OnComplete { get; set; }
        [SerializeField] private string _id; 
        private EventsData _eventsData;

        public void SetEventsData(EventsData data)
        {
            _eventsData = data;
        }


        public void Activate()
        {
            DialogueSystem.OnEndedDialogue += MarkComplete;
        }

        private void MarkComplete(string id)
        {
            if (_id != id) return;
            Completed = true;
            OnComplete?.Invoke();
            DialogueSystem.OnEndedDialogue -= MarkComplete;
        }
    }
}