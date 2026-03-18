using System;
using Entities.Core;
using Entities.Props;
using Interfaces;
using Data;
using UnityEngine;

namespace Systems.Tasks
{
    public class ActiveStateTask : ITask
    {
        public bool Completed { get; set; }
        public Action OnComplete { get; set; }

        [SerializeField] private string _objId;
        [SerializeField] private bool _needToBe; 
        
        private ActiveStateObject _obj;
        private EventsData _eventsData;

        public void SetEventsData(EventsData data)
        {
            _eventsData = data;
        }


        public void Activate()
        {
            _obj = _eventsData.SceneObjects[_objId].GetComponent<ActiveStateObject>();

            _obj.OnActiveChanged += MarkComplete;
        }

        private void MarkComplete(bool b)
        {
            if (b != _needToBe) return;
            
            Completed = true;
            OnComplete?.Invoke();

            _obj.OnActiveChanged -= MarkComplete;
        }
    }
}