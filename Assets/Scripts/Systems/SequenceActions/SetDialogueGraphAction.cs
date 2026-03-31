using Data;
using Entities.Core;
using Data;
using Entities.NPC;
using Interfaces;
using UnityEngine;

namespace Systems.SequenceActions
{
    public class SetDialogueGraphAction : ISequenceAction
    {
        [SerializeField] private RuntimeDialogueGraph _graph;
        [SerializeField] private string _npcId;
        private EventsData _eventsData;

        public void SetEventsData(EventsData data)
        {
            Debug.Log ("ira Dialogue " + _npcId);
            _eventsData = data;
        }

        
        public void Invoke()
        {
            Debug.Log ("ira Dialogue move ");
            if (!_eventsData.SceneObjects.ContainsKey(_npcId)) return ;
            _eventsData.SceneObjects[_npcId].GetComponent<SpeakableCharacter>().Graph = _graph;
        }
    }
}