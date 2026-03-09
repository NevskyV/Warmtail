using Interfaces;
using UnityEngine;
using Entities.Core;
using Data;
using Entities.NPC;

namespace Systems.SequenceActions
{
    public class SetPositionAction : ISequenceAction
    {
        [SerializeField] private string _id;
        [SerializeField] private Vector2 _newPosition;
        private EventsData _eventsData;

        public void SetEventsData(EventsData data)
        {
            _eventsData = data;
        }


        public void Invoke()
        {
            _eventsData.SceneObjects[_id].GetComponent<SpeakableCharacter>().transform.position = _newPosition;
        }
    }
}