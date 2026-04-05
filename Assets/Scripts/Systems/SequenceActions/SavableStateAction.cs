using System.Collections.Generic;
using Entities.Core;
using Entities.Props;
using Interfaces;
using UnityEngine;
using Data;
using UnityEngine.SceneManagement;

namespace Systems.SequenceActions
{
    public class SavableStateAction : ISequenceAction
    {
        [SerializeField] private bool _active;
        [SerializeField] private bool _onLoad;
        [SerializeField] private List<string> _objectIds;
        [SerializeField] private bool _homeScene;
        private EventsData _eventsData;

        public void SetEventsData(EventsData data)
        {
            _eventsData = data;
        }

        
        public void Invoke()
        {
            if ((_homeScene && (SceneManager.GetActiveScene().name == "HomeIra" || SceneManager.GetActiveScene().name == "Home")) ||
                ( !_homeScene && (SceneManager.GetActiveScene().name == "Gameplay" || SceneManager.GetActiveScene().name == "GameplayIra") ) )
            {
                _objectIds.ForEach(x => {
                    if (_eventsData.SceneObjects.ContainsKey(x))
                    {
                        if(!_onLoad) _eventsData.SceneObjects[x].GetComponent<SavableStateObject>().ChangeState(_active);
                        else _eventsData.SceneObjects[x].GetComponent<SavableStateObject>().ChangeStateOnLoad(_active);
                    }
                });
            }
        }
    }
}