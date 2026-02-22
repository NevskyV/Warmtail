using UnityEngine;
using Data;
using Data.Player;
using Interfaces;
using Zenject;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TriInspector;

namespace Systems.Tutorial
{
    [Serializable]
    [CreateAssetMenu(fileName = "EventConfig", menuName = "Configs/EventConfig")]
    public class EventConfig : ScriptableObject
    {
        [SerializeField, ReadOnly] private string _idNode = Guid.NewGuid().ToString();
        public string Id => _idNode;

        [SerializeField] private string _scene;
        [SerializeField] private EventConfig _nextElement;
        [SerializeField] private SequenceElement _element;
        [Inject] private GlobalData _globalData;

        private int _completedTask;

        [Button("Copy")]
        private void Copy()
        {
            GUIUtility.systemCopyBuffer = _idNode;
        }

        public void StartingActivate()
        {
           if (_globalData.Get<SavablePlayerData>().EventsState != _idNode)
            {
                if (_nextElement) _nextElement.StartingActivate();

                if (_scene != SceneManager.GetActiveScene().name) return;
                foreach(ISequenceAction action in _element.Actions)
                {
                    action.Invoke();
                }
            }
            else
            {
                Activate();
            }

        }
        public void Activate()
        {
            if (_scene != SceneManager.GetActiveScene().name) return;
            
            if (_element.Tasks.Count == 0) Invoke();
            else
            {
                foreach(ITask task in _element.Tasks)
                {
                    task.Activate();
                    task.OnComplete += TaskCompleted;
                }
            }
        }

        private void TaskCompleted()
        {
            _completedTask ++;
            if (_completedTask == _element.Tasks.Count)
            {
                Invoke();
            }
        }
        
        public void Invoke()
        {
            foreach(ISequenceAction action in _element.Actions)
            {
                action.Invoke();
            }

            _globalData.Edit<SavablePlayerData>(data => data.EventsState = _nextElement.Id);
            _nextElement.Activate();
        }
    }
}
