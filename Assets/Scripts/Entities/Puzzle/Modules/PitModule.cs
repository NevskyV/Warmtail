using System;
using UnityEngine;

namespace Entities.Puzzle.Modules
{
    [Serializable]
    public class PitModule : PuzzleModule
    {
        [SerializeField] private PitTrigger _trigger;

        public override void Activate()
        {
            if (_trigger != null)
                _trigger.Initialize(this);
        }
    }
}
