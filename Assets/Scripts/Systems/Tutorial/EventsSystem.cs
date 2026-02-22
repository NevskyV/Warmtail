using UnityEngine;
using Data;
using Data.Player;
using System;
using System.Collections.Generic;
using TriInspector;
using Zenject;

namespace Systems.Tutorial
{
    public class EventsSystem : MonoBehaviour
    {
        [SerializeField] private EventConfig _startNode;
        [SerializeField] private EventConfig[] _eventConfigs;

        [Inject] private GlobalData _globalData;
        [Inject]
        private void Construct(DiContainer container)
        {
            foreach(EventConfig e in _eventConfigs)
                container.Inject(e);
        }

        private void Start()
        {
            StartSystem();
        }

        private void StartSystem()
        {
            if (string.IsNullOrEmpty(_globalData.Get<SavablePlayerData>().EventsState)) {
                _globalData.Edit<SavablePlayerData>(data => data.EventsState = _startNode.Id);
            }
           _startNode.StartingActivate();
        }
    }
}
