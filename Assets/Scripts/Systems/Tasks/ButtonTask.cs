using System;
using Entities.Core;
using Interfaces;
using UnityEngine;
using UnityEngine.UI;
using Data;

namespace Systems.Tasks
{
    public class ButtonTask : ITask
    {
        [SerializeField] private string _buttonId;
        public bool Completed { get; set; }
        public Action OnComplete { get; set; }
        
        private Button _button;
        private EventsData _eventsData;

        public void SetEventsData(EventsData data)
        {
            _eventsData = data;
            Debug.Log("ira 1" + _eventsData);
            Debug.Log("ira 2" + _eventsData.SceneObjects);
        }

        
        public void Activate()
        {
            _button = _eventsData.SceneObjects[_buttonId].GetComponent<Button>();
            if (_button != null)
            {
                _button.onClick.AddListener(MarkComplete);
            }
            else
            {
                Debug.LogWarning($"ButtonTask: Button with id '{_buttonId}' not found");
            }
        }

        private void MarkComplete()
        {
            Completed = true;
            OnComplete?.Invoke();
            _button.onClick.RemoveListener(MarkComplete);
        }
    }
}