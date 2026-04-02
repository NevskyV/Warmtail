using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Data;
using Data.Player;
using Entities.PlayerScripts;
using Systems.Abilities;
using UnityEngine;
using Zenject;

namespace Systems
{
    public class TemperatureSystem : IDisposable
    {
        public const float Neutral = 50f;
        public const float Min = 0f;
        public const float Max = 100f;

        private float _driftSpeed = 1f;
        private float _hotThreshold = 65f;
        private float _criticalCold = 5f;
        private float _criticalHot = 95f;

        private static readonly AnimationCurve SpeedCurve = new(
            new Keyframe(0f, 0.85f),
            new Keyframe(0.5f, 1f),
            new Keyframe(1f, 1.3f)
        );

        private const float DriftInterval = 0.1f;

        private GlobalData _globalData;
        private SceneSystem _sceneSystem;
        private PlayerConfig _playerConfig;
        private Transform _playerBody;
        private bool _isCritical;
        private float _baseMoveForce;
        private CancellationTokenSource _cts;

        [Inject]
        private void Construct(GlobalData globalData, SceneSystem sceneSystem, PlayerConfig playerConfig, Player player)
        {
            _globalData = globalData;
            _sceneSystem = sceneSystem;
            _playerConfig = playerConfig;
            _playerBody = player.Rigidbody.transform;

            if (_playerConfig.Abilities[0] is PlayerMovement mv)
                _baseMoveForce = mv.MoveForce;

            _cts = new CancellationTokenSource();
            DriftLoop(_cts.Token).Forget();
        }

        public void Dispose()
        {
            RestoreSpeed();
            _cts?.Cancel();
            _cts?.Dispose();
        }

        public void Modify(float delta)
        {
            float newTemp = 0f;
            _globalData.Edit<RuntimePlayerData>(data =>
            {
                data.Temperature = Mathf.Clamp(data.Temperature + delta, Min, Max);
                newTemp = data.Temperature;

                var hotRange = Max - _hotThreshold;
                data.HeatAttraction = newTemp > _hotThreshold
                    ? (newTemp - _hotThreshold) / hotRange
                    : 0f;
            });

            ApplySpeed(newTemp);
            CheckCritical(newTemp);
            
            NormalizeTemp();
        }

        private async void NormalizeTemp()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            await UniTask.Delay(TimeSpan.FromSeconds(2));
            _cts = new CancellationTokenSource();
            DriftLoop(_cts.Token).Forget();
        }

        private void ApplySpeed(float temp)
        {
            if (_baseMoveForce <= 0f) return;
            if (_playerConfig.Abilities[0] is not PlayerMovement movement) return;
            movement.MoveForce = _baseMoveForce * SpeedCurve.Evaluate(temp / Max);
        }

        private void RestoreSpeed()
        {
            if (_baseMoveForce <= 0f) return;
            if (_playerConfig.Abilities[0] is not PlayerMovement movement) return;
            movement.MoveForce = _baseMoveForce;
        }

        private void CheckCritical(float temp)
        {
            if (_isCritical) return;
            if (temp > _criticalCold && temp < _criticalHot) return;

            _isCritical = true;
            _globalData.Edit<RuntimePlayerData>(data => data.Temperature = Neutral);
            RestoreSpeed();
            _sceneSystem.DieAtNearest(_playerBody.position);
            _isCritical = false;
        }

        private async UniTaskVoid DriftLoop(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(DriftInterval), cancellationToken: ct);

                var temp = _globalData.Get<RuntimePlayerData>().Temperature;

                if (Mathf.Abs(temp - Neutral) < 0.05f)
                {
                    RestoreSpeed();
                    continue;
                }

                var step = _driftSpeed;
                Modify(temp < Neutral ? step : -step);
            }
        }
    }
}
