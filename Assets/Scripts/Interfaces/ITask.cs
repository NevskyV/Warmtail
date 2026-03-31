using System;
using Data;

namespace Interfaces
{
    public interface ITask
    {
        public bool Completed { get; set; }
        public Action OnComplete { get; set; }
        public void Activate();
        public void SetEventsData(EventsData data);
    }
}