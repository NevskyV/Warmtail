using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Player;
using Interfaces;
using Systems.Abilities;
using Zenject;

namespace Systems.Fears
{
    public class FearSystem
    {
        private readonly List<FearConfig> _configs = new();
        private IFearBuff _activeBuff;

        private GlobalData _globalData;
        private WarmthSystem _warmthSystem;
        private PlayerMovement _playerMovement;

        public int ActiveFearId { get; private set; } = -1;

        [Inject]
        private void Construct(GlobalData globalData, WarmthSystem warmthSystem, PlayerMovement playerMovement)
        {
            _globalData = globalData;
            _warmthSystem = warmthSystem;
            _playerMovement = playerMovement;
        }

        public void RegisterConfigs(IReadOnlyList<FearConfig> configs)
        {
            _configs.Clear();
            if (configs != null)
            {
                _configs.AddRange(configs.Where(config => config != null));
            }

            ApplySavedFear();
        }

        public void SetActiveFear(FearConfig config)
        {
            if (config == null) return;

            _globalData.Edit<SavablePlayerData>(data =>
            {
                data.FearIds.Clear();
                data.FearIds.Add(config.Id);
            });

            ApplyFear(config);
        }

        private void ApplySavedFear()
        {
            var data = _globalData.Get<SavablePlayerData>();
            if (data.FearIds == null || data.FearIds.Count == 0)
            {
                ActiveFearId = -1;
                return;
            }

            var savedId = data.FearIds[data.FearIds.Count - 1];
            var config = _configs.FirstOrDefault(x => x.Id == savedId);
            if (config != null)
            {
                ApplyFear(config);
            }
            else
            {
                ActiveFearId = savedId;
            }
        }

        private void ApplyFear(FearConfig config)
        {
            _activeBuff?.Remove(_warmthSystem, _playerMovement);
            _activeBuff = config.Buff;
            ActiveFearId = config.Id;
            _activeBuff?.Apply(_warmthSystem, _playerMovement);
        }
    }
}
