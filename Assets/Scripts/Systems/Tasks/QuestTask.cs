using System;
using Interfaces;
using Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Systems.Tasks
{
    public class QuestTask : ITask
    {
        public bool Completed { get; set; }
        public Action OnComplete { get; set; }
        [SerializeField] private QuestData _questData;
        [SerializeField] private bool _start;
        [SerializeField] private bool _home = false;
        [TextArea] [SerializeField] private string description;

        public void Activate()
        {
            if (!_home && SceneManager.GetActiveScene().name != "Gameplay" && SceneManager.GetActiveScene().name != "GameplayIra") return;
            if (_home && SceneManager.GetActiveScene().name != "Home" && SceneManager.GetActiveScene().name != "HomeIra") return;

            if (_start) QuestSystem.OnQuestStarted.AddListener(MarkComplete);
            else QuestSystem.OnQuestEnded.AddListener(MarkComplete);
        }

        private void MarkComplete(QuestData data, bool start)
        {
            if (data != _questData || start != _start) return;
            Completed = true;
            OnComplete?.Invoke();
            if (_start) QuestSystem.OnQuestStarted.RemoveListener(MarkComplete);
            else QuestSystem.OnQuestEnded.RemoveListener(MarkComplete);
        }
    }
}