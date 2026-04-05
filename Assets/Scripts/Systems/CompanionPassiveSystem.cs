using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Data;
using Data.Player;
using Entities.PlayerScripts;
using Entities.Props;
using Interfaces;
using Systems.Abilities;
using UnityEngine;
using Zenject;

namespace Systems
{
    public class CompanionPassiveSystem : IDisposable
    {
        private const float CrabCooldown = 300f;
        private const float SeaRabbitRadius = 5f;
        private const float SeaRabbitInterval = 3f;

        private readonly HashSet<CompanionType> _activeCompanions = new();

        private GlobalData _globalData;
        private TemperatureSystem _temperatureSystem;
        private WarmthSystem _warmthSystem;
        private PlayerMovement _playerMovement;
        private Transform _playerTransform;
        private float _baseMoveForce;

        private bool _crabOnCooldown;

        [Inject]
        private void Construct(GlobalData globalData, TemperatureSystem temperatureSystem,
            WarmthSystem warmthSystem, PlayerConfig playerConfig, Player player)
        {
            _globalData = globalData;
            _temperatureSystem = temperatureSystem;
            _warmthSystem = warmthSystem;
            _playerTransform = player.Rigidbody.transform;

            _playerMovement = playerConfig.Abilities.OfType<PlayerMovement>().FirstOrDefault();
            if (_playerMovement != null)
                _baseMoveForce = _playerMovement.MoveForce;
        }

        public void Register(CompanionType type)
        {
            if (_activeCompanions.Contains(type)) return;
            _activeCompanions.Add(type);
            Apply(type);
        }

        public void Unregister(CompanionType type)
        {
            if (!_activeCompanions.Remove(type)) return;
            Remove(type);
        }

        private void Apply(CompanionType type)
        {
            switch (type)
            {
                case CompanionType.Fish:
                    ApplyFish();
                    break;
                case CompanionType.Jellyfish:
                    ApplyJellyfish();
                    break;
                case CompanionType.Crab:
                    ApplyCrab();
                    break;
                case CompanionType.SeaRabbit:
                    ApplySeaRabbitLoop().Forget();
                    break;
            }
        }

        private void Remove(CompanionType type)
        {
            if (type == CompanionType.Fish && _playerMovement != null)
                _playerMovement.MoveForce = _baseMoveForce;
        }

        private void ApplyFish()
        {
            if (_playerMovement == null) return;
            _playerMovement.MoveForce = _baseMoveForce * 1.2f;
        }

        private void ApplyJellyfish()
        {
            _globalData.Edit<SavablePlayerData>(data => data.Stars += 1);
        }

        private void ApplyCrab()
        {
            _temperatureSystem.OnBeforeCritical += OnCrabIntercept;
        }

        private bool OnCrabIntercept()
        {
            if (_crabOnCooldown) return false;
            _crabOnCooldown = true;
            CrabCooldownTimer().Forget();
            return true;
        }

        private async UniTaskVoid CrabCooldownTimer()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(CrabCooldown));
            _crabOnCooldown = false;
        }

        private async UniTaskVoid ApplySeaRabbitLoop()
        {
            while (_activeCompanions.Contains(CompanionType.SeaRabbit))
            {
                WarmNearbyShells();
                await UniTask.Delay(TimeSpan.FromSeconds(SeaRabbitInterval));
            }
        }

        private void WarmNearbyShells()
        {
            if (_playerTransform == null) return;
            var hits = Physics2D.OverlapCircleAll(_playerTransform.position, SeaRabbitRadius);
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<Warmable>(out var warmable))
                    warmable.Warm();
            }
        }

        public void Dispose()
        {
            if (_temperatureSystem != null)
                _temperatureSystem.OnBeforeCritical -= OnCrabIntercept;
        }
    }
}
