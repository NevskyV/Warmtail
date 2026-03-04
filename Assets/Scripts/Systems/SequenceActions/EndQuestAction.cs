using Data;
using Interfaces;
using Data;
using UnityEngine;

namespace Systems.SequenceActions
{
    public class EndQuestAction : ISequenceAction
    {
       [SerializeField] private QuestData _quest;
        private EventsData _eventsData;

        public void SetEventsData(EventsData data)
        {
            _eventsData = data;
        }

       
       public void Invoke()
       {
           QuestSystem.EndQuest(_quest);
       }
    }
}