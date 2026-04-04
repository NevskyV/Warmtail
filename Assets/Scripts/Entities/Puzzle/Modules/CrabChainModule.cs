using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Entities.Creatures;
using Interfaces;
using UnityEngine;

namespace Entities.Puzzle.Modules
{
    [Serializable]
    public class CrabChainModule : PuzzleModule
    {
        [SerializeField] private List<ChainCrab> _crabsInOrder = new();
        [SerializeField] private float _chainDelaySeconds = 0.5f;

        public override void Activate()
        {
            if (_crabsInOrder.Count == 0) return;
            _crabsInOrder[0].Initialize(0, this);
        }

        public void ReportCrabWarmed(int index)
        {
            int next = index + 1;
            if (next >= _crabsInOrder.Count)
            {
                Solve();
                return;
            }
            PropagateChain(next).Forget();
        }

        private async UniTaskVoid PropagateChain(int index)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_chainDelaySeconds));
            if (_crabsInOrder[index] == null) return;
            _crabsInOrder[index].Initialize(index, this);
            _crabsInOrder[index].TriggerChain();
        }
    }
}
