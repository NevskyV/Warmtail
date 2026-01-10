using System;
using Interfaces;
using Data;
using UnityEngine;

namespace Systems.Tasks
{
    public class QuestTask : ITask
    {
        public bool Completed { get; set; }
        public Action OnComplete { get; set; }
        [SerializeField] private QuestData _questData;
        [SerializeField] private bool _start;
        [TextArea] [SerializeField] private string description;

        public void Activate()
        {
            if (_start) QuestSystem.OnQuestStarted += MarkComplete;
            else QuestSystem.OnQuestEnded += MarkComplete;
        }

        private void MarkComplete(QuestData data)
        {
            if (data != _questData) return;
            Completed = true;
            OnComplete?.Invoke();
            QuestSystem.OnQuestEnded -= MarkComplete;
        }
    }
}