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
        
        private void Start()
        {
            _puzzleCompletion = new(_modules.Count);
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
            _puzzleCompletion[_modules.IndexOf(module)] = true;
            if (_puzzleCompletion.TrueForAll(x => x))
            {
                Reward();
            }
        }
        
        private void Reward()
        {
            _reward.Get();
            ChangeState(false);
        }
    }
}