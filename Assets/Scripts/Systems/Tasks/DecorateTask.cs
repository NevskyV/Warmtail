using Interfaces;
using System;

namespace Systems.Tasks
{
    public class DecorateTask : ITask
    {
        public bool Completed { get; set; }
        public Action OnComplete { get; set; }

        public void Activate()
        {
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