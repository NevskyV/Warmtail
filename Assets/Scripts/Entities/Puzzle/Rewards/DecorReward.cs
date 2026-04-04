using Data;
using Data.Player;
using UnityEngine;
using Zenject;

namespace Entities.Puzzle.Rewards
{
    public class DecorReward : Reward
    {
        [SerializeField] private int _decorItemId;
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
                if (!player.Inventory.ContainsKey(_decorItemId))
                    player.Inventory[_decorItemId] = 0;
                player.Inventory[_decorItemId]++;
            });
        }
    }
}
