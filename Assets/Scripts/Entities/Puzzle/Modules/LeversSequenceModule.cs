using System;
using System.Collections.Generic;
using UnityEngine;

namespace Entities.Puzzle.Modules
{
    [Serializable]
    public class LeversSequenceModule : PuzzleModule
    {
        [SerializeField] private List<SequenceLever> _leversInOrder = new();

        private int _nextExpected;

        public override void Activate()
        {
            _nextExpected = 0;
            for (int i = 0; i < _leversInOrder.Count; i++)
            {
                if (_leversInOrder[i] == null) continue;
                _leversInOrder[i].Initialize(i, this);
            }
        }

        public void ReportActivated(int index)
        {
            if (index != _nextExpected)
            {
                _nextExpected = 0;
                return;
            }
            _nextExpected++;
            if (_nextExpected >= _leversInOrder.Count)
                Solve();
        }

        public void ReportDeactivated()
        {
            _nextExpected = 0;
        }
    }
}
