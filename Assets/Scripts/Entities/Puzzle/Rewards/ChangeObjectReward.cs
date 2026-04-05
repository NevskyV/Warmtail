using Entities.Props;
using UnityEngine;
using Zenject;

namespace Entities.Puzzle.Rewards
{
    public class ChangeObjectReward : Reward
    {
        [SerializeField] private bool _activate;
        [SerializeField] private SavableStateObject _object;
        
        public override void Get()
        {
            _object.ChangeState(_activate);
        }
    }
}