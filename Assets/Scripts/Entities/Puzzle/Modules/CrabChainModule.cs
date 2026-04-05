using System;
using System.Collections.Generic;
using UnityEngine;

namespace Entities.Puzzle.Modules
{
    [Serializable]
    public class CrabChainModule : PuzzleModule
    {
        [SerializeField] private List<ChainCrab> _crabsInOrder = new();

        private int _warmedCount;

        public override void Activate()
        {
            if (_crabsInOrder.Count == 0) return;
            _warmedCount = 0;
            for (int i = 0; i < _crabsInOrder.Count; i++)
            {
                if (_crabsInOrder[i] == null) continue;
                bool playerActivatable = i == 0;
                _crabsInOrder[i].Initialize(i, this, playerActivatable);
            }
        }

        public void ReportCrabWarmed(int index)
        {
            _warmedCount++;
            if (_warmedCount >= _crabsInOrder.Count)
                Solve();
        }

        public override void Reset()
        {
            base.Reset();
            _warmedCount = 0;
        }
    }
}
