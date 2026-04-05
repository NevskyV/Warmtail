using Data;
using Data.Player;
using UnityEngine;
using Zenject;

namespace Entities.Puzzle.Rewards
{
    public class StarShardDestroyReward : Reward
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
                player.Shells = UnityEngine.Mathf.Max(0, player.Shells - _amount);
            });
        }
    }
}
