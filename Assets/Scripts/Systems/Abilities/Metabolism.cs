using System;
using Cysharp.Threading.Tasks;
using Interfaces;
using UnityEngine;
using Zenject;

namespace Systems.Abilities.Concrete
{
    [Serializable]
    public class MetabolismAbility : BaseAbility
    {
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
            
            StartAbility += OnStart;
            EndAbility += OnEnd;
        }

        private void OnStart()
        { 
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
            while (Enabled)
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

        private void OnEnd()
        {
            RestoreSpeed();
        }
    }
}
