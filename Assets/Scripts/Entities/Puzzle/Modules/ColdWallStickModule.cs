using System;
using UnityEngine;

namespace Entities.Puzzle.Modules
{
    [Serializable]
    public class ColdWallStickModule : PuzzleModule
    {
        [SerializeField] private ColdStickyWall _wall;
        [SerializeField] private ColdWallExitTrigger _exitTrigger;

        public override void Activate()
        {
            if (_wall != null)
                _wall.Initialize(this);
            if (_exitTrigger != null)
                _exitTrigger.Initialize(this);
        }
    }
}
