using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Player;
using Data;
using Interfaces;
using Systems.Abilities;
using UnityEngine;
using Zenject;

namespace Systems.Fears
{
    public class FearBuffSystem : IInitializable, IDisposable
    {
        private readonly GlobalData _globalData;
        private readonly WarmthSystem _warmthSystem;
        private readonly PlayerMovement _playerMovement;

        private List<FearConfig> _configs;
        private IFearBuff _activeBuff;
        private int _activeFearId = -1;

        [Inject]
        public FearBuffSystem(GlobalData globalData, WarmthSystem warmthSystem, PlayerMovement playerMovement)
        {
            _globalData = globalData;
            _warmthSystem = warmthSystem;
            _playerMovement = playerMovement;
        }

        public void Initialize()
        {
            _configs = Resources.LoadAll<FearConfig>(string.Empty).ToList();
            _globalData.SubscribeTo<SavablePlayerData>(SyncFromData);
            SyncFromData();
        }

        private void SyncFromData()
        {
            var id = _globalData.Get<SavablePlayerData>().ActiveFearId;
            if (id == _activeFearId) return;
            
            _activeBuff?.Remove(_warmthSystem, _playerMovement);
            _activeBuff = null;
            _activeFearId = id;

            if (id < 0) return;

            var config = _configs?.FirstOrDefault(c => c != null && c.Id == id);
            var buff = config?.Buff;

            if (buff == null && id == 10)
            {
                buff = new Entities.Props.OldAgeFear();
            }

            _activeBuff = buff;
            _activeBuff?.Apply(_warmthSystem, _playerMovement);
        }

        public void Dispose()
        {
            _activeBuff?.Remove(_warmthSystem, _playerMovement);
            _activeBuff = null;
        }
    }
}

