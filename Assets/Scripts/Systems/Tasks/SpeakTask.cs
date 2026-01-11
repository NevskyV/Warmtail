using System;
using Interfaces;
using UnityEngine;

namespace Systems.Tasks
{
    public class SpeakTask : ITask
    {
        public bool Completed { get; set; }
        public Action OnComplete { get; set; }
        [SerializeField] private string _id;
        [TextArea] [SerializeField] private string description;

        public void Activate()
        {
            DialogueSystem.OnEndedDialogue += MarkComplete;
        }

        private void MarkComplete(string id)
        {
            if (_id != id) return;
            Completed = true;
            OnComplete?.Invoke();
            DialogueSystem.OnEndedDialogue -= MarkComplete;
        }
    }
}