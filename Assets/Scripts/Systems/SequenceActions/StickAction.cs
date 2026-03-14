using System;
using Data;
using Interfaces;

namespace Systems.SequenceActions
{
    public class StickAction : ISequenceAction
    {
        public static Action OnStickTaked;
        private EventsData _eventsData;

        public void SetEventsData(EventsData data)
        {
            _eventsData = data;
        }


        public void Invoke()
        {
            OnStickTaked?.Invoke();
        }
    }
}