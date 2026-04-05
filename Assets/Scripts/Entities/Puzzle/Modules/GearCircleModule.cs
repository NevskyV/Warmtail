using System;
using UnityEngine;

namespace Entities.Puzzle.Modules
{
    [Serializable]
    public class GearCircleModule : PuzzleModule
    {
        [SerializeField] private RotatingGear _gear;

        public override void Activate()
        {
            if (_gear != null)
                _gear.Initialize(this);
        }
    }
}
