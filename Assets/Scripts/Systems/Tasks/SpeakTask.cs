using System;
using Interfaces;
using UniRx;
using Data;
using UniRx.Triggers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Systems.Tasks
{
    public class SpeakTask : ITask
    {
        public bool Completed { get; set; }
        public Action OnComplete { get; set; }
        [SerializeField] private string _id;

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