using System.Collections.Generic;
using Entities.Props;
using Entities.Puzzle.Modules;
using Entities.Puzzle.Rewards;
using UnityEngine;
using Zenject;

namespace Entities.Puzzle
{
    public class PuzzleCore : SavableStateObject
    {
        [SerializeReference] private List<PuzzleModule> _modules = new();
        [SerializeReference] private Reward _reward;
        private List<bool> _puzzleCompletion;
        
        [Inject] private DiContainer _diContainer;
        
        private bool _isComplete;

        private void Start()
        {
            _puzzleCompletion = new List<bool>(new bool[_modules.Count]);
            foreach (var module in _modules)
            {
                _diContainer.Inject(module);
                module.Activate();
                module.OnSolve += CheckCompletion;
            }
            _diContainer.Inject(_reward);
        }
        
        private void CheckCompletion(PuzzleModule module)
        {
            if (_isComplete) return;

            int index = _modules.IndexOf(module);
            if (index < 0) return;

            _puzzleCompletion[index] = true;
            if (_puzzleCompletion.TrueForAll(x => x))
                Reward();
        }
        
        private void Reward()
        {
            if (_isComplete) return;
            _isComplete = true;

            foreach (var module in _modules)
                module.OnSolve -= CheckCompletion;

            _reward.Get();
            ChangeState(false);
        }
    }
}