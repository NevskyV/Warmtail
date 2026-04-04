using System;
using UnityEngine;

namespace Entities.Puzzle.Modules
{
    [Serializable]
    public class TimedLeverModule : PuzzleModule
    {
        [SerializeField] private TimedDashLever _lever;

        public override void Activate()
        {
            if (_lever != null)
                _lever.Initialize(this);
        }
    }
}
