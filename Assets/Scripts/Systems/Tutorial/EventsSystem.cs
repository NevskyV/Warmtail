using UnityEngine;
using Data;
using Data.Player;
using System;
using System.Collections.Generic;
using TriInspector;
using Zenject;
using UnityEngine.SceneManagement;
using Interfaces;

namespace Systems.Tutorial
{
    public class EventsSystem : MonoBehaviour
    {
        [SerializeField] private EventConfig _startNode;

        public static Action<EventConfig> OnEventCompleted = delegate {};
        private string _finishString = "finish";

        [Inject] private GlobalData _globalData;
        [Inject] private EventsData _eventsData;


        private void Awake()
        {
            OnEventCompleted += Invoke;
            Debug.Log("ira _eventsData 3 " + _eventsData);
            StartSystem();
        }
        private void OnDestroy()
        {
            OnEventCompleted -= Invoke;
        }

        private void StartSystem()
        {
            if (string.IsNullOrEmpty(_globalData.Get<SavablePlayerData>().EventsState)) {
                _globalData.Edit<SavablePlayerData>(data => data.EventsState = _startNode.IdNode);
            }
           StartingActivate(_startNode);
        }

        
        private void StartingActivate(EventConfig config)
        {
            SetData(config);

            if (_globalData.Get<SavablePlayerData>().EventsState != config.IdNode)
            {
                if (config.NextElement) StartingActivate(config.NextElement);

                if (config.Scene != SceneManager.GetActiveScene().name) return;
                foreach(ISequenceAction action in config.Element.Actions)
                {
                    action.Invoke();
                }
            }
            else
            {
                Debug.Log("ira _eventsData 5 " + config.Element.Tasks.Count);
                Activate(config);
            }
        }

        private void SetData(EventConfig config)
        {
            foreach(ISequenceAction action in config.Element.Actions)
            {
                action.SetEventsData(_eventsData);
            }
            foreach(ITask task in config.Element.Tasks)
            {
                Debug.Log("ira _eventsData 3.5 " + config.Element.Tasks.Count);
                task.SetEventsData(_eventsData);
            }
            Debug.Log("ira _eventsData 4 " + config.Element.Tasks.Count);
        }

        private void Activate(EventConfig config)
        {
            if (config.Scene != SceneManager.GetActiveScene().name) return;
            
            if (config.Element.Tasks.Count == 0) Invoke(config);
            else
            {
                foreach(ITask task in config.Element.Tasks)
                {
                    Debug.Log("ira _eventsData 6 " + config.Element.Tasks.Count);
                    task.Activate();
                    task.OnComplete += config.TaskCompleted;
                }
            }
        }
        
        private void Invoke(EventConfig config)
        {
            foreach(ISequenceAction action in config.Element.Actions)
            {
                action.Invoke();
            }

            if (config.NextElement == null)
            {
                _globalData.Edit<SavablePlayerData>(data => data.EventsState = _finishString);
            }
            else
            {
                _globalData.Edit<SavablePlayerData>(data => data.EventsState = config.NextElement.IdNode);
            
                SetData(config.NextElement);
                Activate(config.NextElement);
            }
        }
    }
}
