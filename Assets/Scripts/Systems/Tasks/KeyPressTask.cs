using System;
using Entities.Core;
using Data;
using Entities.UI;
using Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Systems.Tasks
{
    public class KeyPressTask : ITask
    {
        public bool Completed { get; set; }
        public Action OnComplete { get; set; }
        [SerializeField] private InputActionAsset _reference;
        [SerializeField] private string _actionName;
        [SerializeField] private string _tipsVisuals;
        [TextArea] [SerializeField] private string _description;
        private EventsData _eventsData;

        public void SetEventsData(EventsData data)
        {
            _eventsData = data;
        }

        
        private TipsVisuals _tips;

        public void Activate()
        {
            Completed = false;
            _eventsData.SceneObjects[_tipsVisuals].GetComponent<TipsVisuals>().ShowTip(_reference[_actionName]);
            _reference[_actionName].started += MarkComplete;
        }

        private void MarkComplete(InputAction.CallbackContext _)
        {
            Completed = true;
            OnComplete?.Invoke();
            _reference[_actionName].started -= MarkComplete;
        }
    }
}