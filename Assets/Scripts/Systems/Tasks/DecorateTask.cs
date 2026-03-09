using Data;
using Interfaces;
using System;

namespace Systems.Tasks
{
    public class DecorateTask : ITask
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
            PlacementSystem.OnApplyed += MarkComplete;
        }

        private void MarkComplete()
        {
            Completed = true;
            OnComplete?.Invoke();
            PlacementSystem.OnApplyed -= MarkComplete;
        }
    }
}