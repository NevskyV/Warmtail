using Data;
using Interfaces;
using Data;
using Systems.Abilities;
using UnityEngine;

namespace Systems.SequenceActions
{
    public class AddAbilityAction : ISequenceAction
    {
        [SerializeField] private AbilityType _type;
        private EventsData _eventsData;

        public void SetEventsData(EventsData data)
        {
            _eventsData = data;
        }

        
        public void Invoke()
        {
            AbilitiesSystem.Instance.AddAbility(_type);
        }
    }
}