using System;
using UnityEngine;

namespace Entities.Puzzle.Modules
{
    [Serializable]
    public class ButtonPressModule : PuzzleModule
    {
        [SerializeField] private ButtonPressTrigger _trigger;

        public override void Activate()
        {
            if (_trigger != null)
                _trigger.Initialize(this);
        }
    }
}
