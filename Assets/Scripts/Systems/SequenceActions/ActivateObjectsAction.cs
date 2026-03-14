using System.Collections.Generic;
using Entities.Core;
using Data;
using Interfaces;
using UnityEngine;

namespace Systems.SequenceActions
{
    public class ActivateObjectsAction : ISequenceAction
    {
        [SerializeField] private bool _active;
        [SerializeField] private List<string> _objectIds;
        private EventsData _eventsData;

        public void SetEventsData(EventsData data)
        {
            _eventsData = data;
            Debug.Log("ira 3 " + _active);
        }

            
        public void Invoke()
        {
            _objectIds.ForEach(x =>
            {
                Debug.Log("ira 4 " + _active + _eventsData);
                Debug.Log("ira 5 " + _active + _eventsData.SceneObjects);
                var obj = _eventsData.SceneObjects[x];
                if (obj != null)
                {
                    obj.SetActive(_active);
                }
                else
                {
                    Debug.LogWarning($"ActivateObjectsAction: Object with id '{x}' not found");
                }
            });
        }
    }
}