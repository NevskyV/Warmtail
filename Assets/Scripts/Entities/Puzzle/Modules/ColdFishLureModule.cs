using System;
using Data;
using Data.Player;
using Entities.Creatures;
using UnityEngine;
using Zenject;

namespace Entities.Puzzle.Modules
{
    [Serializable]
    public class ColdFishLureModule : PuzzleModule
    {
        [SerializeField] private ColdFishLure _fish;

        public override void Activate()
        {
            if (_fish != null)
                _fish.Initialize(this);
        }
    }
}
