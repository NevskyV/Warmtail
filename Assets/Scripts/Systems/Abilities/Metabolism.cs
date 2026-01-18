using System;
using Cysharp.Threading.Tasks;
using Interfaces;
using UnityEngine;
using Zenject;

namespace Systems.Abilities
{
    [Serializable]
    public class MetabolismAbility : WarmthAbility
    {
        [SerializeField] private float _cooldownDuration = 1f;

        private bool _canActivate = true;
        private bool _isRunning;
        
        [SerializeField] private int _heatDrainPerSecond = 8;
        [SerializeField] private float _speedMultiplier = 2f;

        private IPlayerDataProvider _playerDataProvider;
        private IWarmthSystem _warmthSystem;
        private int _baseSpeed;

        [Inject]
        public void Construct(IPlayerDataProvider playerDataProvider, IWarmthSystem warmthSystem)
        {
            _playerDataProvider = playerDataProvider;
            _warmthSystem = warmthSystem;
            
            EndAbility += OnEnd;
        }

        public void ActivateMetabolism()
        {
            OnStart();
        }

        private void OnStart()
        { 
            if (!_canActivate) return;

            _canActivate = false;
            _isRunning = true;
            
            Debug.Log("MetabolismAbility.OnStart()");
            _baseSpeed = _playerDataProvider.GetSpeed();
            ApplySpeedModifier();
            DrainRoutine().Forget();
        }

        private void ApplySpeedModifier()
        {
            int newSpeed = Mathf.RoundToInt(_baseSpeed * _speedMultiplier);
            _playerDataProvider.SetSpeed(newSpeed);
        }

        private void RestoreSpeed()
        {
            _playerDataProvider.SetSpeed(_baseSpeed);
        }

        private async UniTaskVoid DrainRoutine()
        {
            Debug.Log("MetabolismAbility.DrainRoutine()");
            while (Enabled && _isRunning)
            {
                Debug.Log("MetabolismAbility.DrainRoutine(DDDD)");
                DrainWarmth();
                await UniTask.Delay(1000);
            }
        }

        private void DrainWarmth()
        {
            _warmthSystem.DecreaseWarmth(_heatDrainPerSecond);
        }

        private async void OnEnd()
        {
            _isRunning = false;
            RestoreSpeed();

            await UniTask.Delay(TimeSpan.FromSeconds(_cooldownDuration));
            _canActivate = true;
        }
    }
}
