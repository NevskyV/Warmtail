using System;
using Entities.Core;
using Entities.Props;
using Interfaces;
using Data;
using UnityEngine;

namespace Systems.Tasks
{
    public class SavabaleStateTask : ITask
    {
        public bool Completed { get; set; }
        public Action OnComplete { get; set; }

        [SerializeField] private string _objId;
        [SerializeField] private bool _needToBe; 
        
        private SavableStateObject _obj;
        private EventsData _eventsData;

        public void SetEventsData(EventsData data)
        {
            _eventsData = data;
        }


        public void Activate()
        {
            _obj = _eventsData.SceneObjects[_objId].GetComponent<SavableStateObject>();
            Debug.Log("iraaaa1");
            _obj.OnStateChanged += b =>
            {
                Debug.Log("iraaaa2");
                if(b == _needToBe) MarkComplete();
            };
        }

        private void MarkComplete()
        {
            Completed = true;
            OnComplete?.Invoke();
            _obj.OnStateChanged -= b =>
            {
                if(b == _needToBe) MarkComplete();
            };
        }
    }
}