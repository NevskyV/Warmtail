using System;
using Cysharp.Threading.Tasks;
using Data;
using Data.Player;
using UnityEngine;
using Zenject;

namespace Systems.Abilities.Concrete
{
    [Serializable]
    public class MetabolismAbility : BaseAbility
    {
        [SerializeField] private int _heatDrainPerSecond = 8;
        [SerializeField] private float _speedMultiplier = 2f;

        private GlobalData _globalData;
        private WarmthSystem _warmthSystem;
        private int _baseSpeed;

        [Inject]
        public void Construct(GlobalData globalData, WarmthSystem warmthSystem)
        {
            _globalData = globalData;
            _warmthSystem = warmthSystem;
            
            StartAbility += OnStart;
            EndAbility += OnEnd;
        }

        private void OnStart()
        { 
            Debug.Log("MetabolismAbility.OnStart()");
            _baseSpeed = _globalData.Get<RuntimePlayerData>().Speed;
            _globalData.Edit<RuntimePlayerData>(d => d.Speed = Mathf.RoundToInt(_baseSpeed * _speedMultiplier));
            DrainRoutine().Forget();
        }

        private async UniTaskVoid DrainRoutine()
        {
            Debug.Log("MetabolismAbility.DrainRoutine()");
            while (Enabled)
            {
                Debug.Log("MetabolismAbility.DrainRoutine(DDDD)");
                _warmthSystem.DecreaseWarmth(_heatDrainPerSecond);
                await UniTask.Delay(1000);
            }
        }

        private void OnEnd()
        {
            _globalData.Edit<RuntimePlayerData>(d => d.Speed = _baseSpeed);
        }
    }
}
