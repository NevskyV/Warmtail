using Data;
using Data.Player;
using UnityEngine;
using Zenject;

namespace Entities.Puzzle.Rewards
{
    public class ShellReward : Reward
    {
        [SerializeField] private int _amount = 50;
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