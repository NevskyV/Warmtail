using System;
using Interfaces;
using Data;
using Systems.Tutorial;
using UnityEngine;
using Data;
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
        private EventsData _eventsData;

        public void SetEventsData(EventsData data)
        {
            _eventsData = data;
        }


        public void Activate()
        {
            TutorialSystem.TaskActivated.Add(this);

            if (_start) QuestSystem.OnQuestStarted.AddListener(MarkComplete);
            else QuestSystem.OnQuestEnded.AddListener(MarkComplete);
        }

        private void MarkComplete(QuestData data, bool start)
        {
            if (data != _questData || start != _start) return;
            if (!_home && SceneManager.GetActiveScene().name != "Gameplay" && SceneManager.GetActiveScene().name != "GameplayIra") return;
            if (_home && SceneManager.GetActiveScene().name != "Home" && SceneManager.GetActiveScene().name != "HomeIra") return;

            Completed = true;
            OnComplete?.Invoke();
            if (_start) QuestSystem.OnQuestStarted.RemoveListener(MarkComplete);
            else QuestSystem.OnQuestEnded.RemoveListener(MarkComplete);
        }
    }
}