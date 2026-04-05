using Data;
using Data.Player;
using UnityEngine;
using Zenject;

namespace Entities.Puzzle.Rewards
{
    public class StarShardReward : Reward
    {
        [SerializeField] private int _amount = 1;
        private GlobalData _globalData;

        [Inject]
        public void Construct(GlobalData globalData)
        {
            _globalData = globalData;
        }

        public override void Get()
        {
            _globalData.Edit<SavablePlayerData>(player =>
            {
                player.Shells += _amount;
            });
        }
    }
}
