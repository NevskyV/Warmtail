using System;
using Data;
using Data.Player;
using Systems;
using UnityEngine;
using Zenject;

namespace Entities.Puzzle.Modules
{
    [Serializable]
    public class TemperatureTriggerModule : PuzzleModule
    {
        [SerializeField] private TemperatureTriggerZone _zone;

        public override void Activate()
        {
            if (_zone != null)
                _zone.Initialize(this);
        }
    }
}
