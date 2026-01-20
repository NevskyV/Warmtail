using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Systems.Abilities
{
    [Serializable]
    public class MetabolismAbility : WarmthAbility
    {
        private bool _canActivate = true;
        private bool _isRunning;
        [SerializeField] private float _speedMultiplier = 2f;

        private PlayerDataProvider _playerDataProvider;
        private int _baseSpeed;

        [Inject]
        public void Construct(PlayerDataProvider playerDataProvider)
        {
            _playerDataProvider = playerDataProvider;
            EndAbility += OnEnd;
        }

        private void ApplyNewSpeed()
        { 
            if (!_canActivate) return;

            _canActivate = false;
            _isRunning = true;
            
            Debug.Log("MetabolismAbility.OnStart()");
            _baseSpeed = int.Parse(_playerDataProvider.GetProperty("Speed"));
            int newSpeed = Mathf.RoundToInt(_baseSpeed * _speedMultiplier);
            _playerDataProvider.SetProperty("Speed",newSpeed.ToString());
        }

        private void RestoreSpeed()
        {
            _playerDataProvider.SetProperty("Speed",_baseSpeed.ToString());
        }

        private async void OnEnd()
        {
            _isRunning = false;
            RestoreSpeed();

            await UniTask.Delay(TimeSpan.FromSeconds(Cooldown));
            _canActivate = true;
        }
    }
}
