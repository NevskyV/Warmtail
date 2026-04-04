using System;
using UnityEngine;

namespace Entities.Puzzle.Modules
{
    [Serializable]
    public class RenewableIceModule : PuzzleModule
    {
        [SerializeField] private RenewableIcePlatform _platform;
        [SerializeField] private IcePassTrigger _successTrigger;

        public override void Activate()
        {
            if (_platform != null)
                _platform.Initialize();
            if (_successTrigger != null)
                _successTrigger.Initialize(this);
        }
    }
}
