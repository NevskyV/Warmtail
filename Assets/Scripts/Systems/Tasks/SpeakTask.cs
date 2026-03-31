using System;
using Interfaces;
using Data;
using UnityEngine;
using Entities.NPC;

namespace Systems.Tasks
{
    public class SpeakTask : ITask
    {
        public bool Completed { get; set; }
        public Action OnComplete { get; set; }
        [SerializeField] private string _id; 
        private string _unityId; 
        private EventsData _eventsData;

        public void SetEventsData(EventsData data)
        {
            _eventsData = data;
        }


        public void Activate()
        {
            _unityId = _eventsData.SceneObjects[_id].GetComponent<SpeakableCharacter>().Id;
            DialogueSystem.OnEndedDialogue += MarkComplete;
        }

        private void MarkComplete(string id)
        {
            Debug.Log("ira speak " + id + "  need id=" + _unityId);
            if (_unityId != id) return;
            Completed = true;
            OnComplete?.Invoke();
            DialogueSystem.OnEndedDialogue -= MarkComplete;
        }
    }
}