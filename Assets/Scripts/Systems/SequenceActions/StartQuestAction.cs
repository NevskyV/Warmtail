using Data;
using Data;
using Interfaces;
using UnityEngine;

namespace Systems.SequenceActions
{
    public class StartQuestAction : ISequenceAction
    {
        [SerializeField] private QuestData _quest;
        private EventsData _eventsData;

        public void SetEventsData(EventsData data)
        {
            _eventsData = data;
        }

        
        public void Invoke()
        {
            QuestSystem.StartQuest(_quest);
        }
	}
}