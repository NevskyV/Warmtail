using System;
using Data;
using UnityEngine;
using Interfaces;
using Data.Player;

namespace Systems.SequenceActions
{
    public class FearSpawnAction : ISequenceAction
    {
        private EventsData _eventsData;

        private GlobalData _globalData;
        [SerializeField] private bool isInFearWorld; //in SceneSystem

        public void SetEventsData(EventsData data)
        {
            _eventsData = data;
            _globalData = data.GlobalData;
        }


        public void Invoke()
        {
            _globalData.Edit<SavablePlayerData>(data => data.IsInFearWorld = isInFearWorld);
        }
    }
}