using System;
using Entities.Props;
using Data;
using Interfaces;
using UnityEngine;

namespace Systems.Tasks
{
    public class ShellsTask : ITask
    {
        public bool Completed { get; set; }
        public Action OnComplete { get; set; }
        private EventsData _eventsData;

        public void SetEventsData(EventsData data)
        {
            _eventsData = data;
        }


        public void Activate()
        {
            Completed = false;
            Shell.OnShellsChanged += MarkComplete;
        }

        private void MarkComplete()
        {
            Completed = true;
            OnComplete?.Invoke();
            Shell.OnShellsChanged -= MarkComplete;
        }
    }
}