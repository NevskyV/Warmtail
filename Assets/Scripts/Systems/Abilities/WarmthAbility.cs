using System;
using Cysharp.Threading.Tasks;
using Interfaces;
using UnityEngine;
using Zenject;

namespace Systems.Abilities
{
    public abstract class WarmthAbility : IAbility
    {
        [field: SerializeReference] public IAbilityVisual Visual { get; set; }
        [SerializeField] protected int WarmthCost;
        [SerializeField, Range(0f, 5)] protected float Cooldown;
        [field: SerializeReference] public string BaseMethodName { get; private set; }
        public bool Enabled { get; set; }
        public Action StartAbility { get; set; }
        public Action UsingAbility { get; set; }
        public Action EndAbility { get; set; }
        public string MethodName { get; set; }

        [Inject] private WarmthSystem _warmthSystem;
        private bool _drainWarmthRunning;

        public void UseAbility()
        {
            MethodName ??= BaseMethodName;
            
            if (!string.IsNullOrEmpty(MethodName))
                GetType().GetMethod(MethodName)?.Invoke(this, null);
            
            Enabled = true;
            StartAbility?.Invoke();
            
            if (WarmthCost > 0 && Cooldown > 0 && !_drainWarmthRunning)
            {
                DrainWarmth();
            }
        }
        
        public void StopAbility()
        {
            Enabled = false;
            _drainWarmthRunning = false;
            EndAbility?.Invoke();
        }
        
        public async void DrainWarmth()
        {
            if (_drainWarmthRunning) return;
            _drainWarmthRunning = true;
            
            while (Enabled && _drainWarmthRunning && _warmthSystem.CheckWarmCost(WarmthCost))
            {
                _warmthSystem.DecreaseWarmth(WarmthCost);
                await UniTask.Delay(TimeSpan.FromSeconds(Cooldown));
            }
            
            _drainWarmthRunning = false;
            StopAbility();
        }
    }
}