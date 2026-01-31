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
        [SerializeField] protected int MaxWarmthCost;
        [SerializeField, Range(0f, 2)] protected float Tick;
        [SerializeField, Range(0f, 5)] protected float Cooldown;
        [field: SerializeReference] public string BaseMethodName { get; private set; }
        public bool Enabled { get; set; }
        public bool InUse { get; set; }
        public Action StartAbility { get; set; }
        public Action UsingAbility { get; set; }
        public Action EndAbility { get; set; }
        public string MethodName { get; set; }
        public bool CooldownRunning => _cooldownRunning;
        private WarmthSystem _warmthSystem;
        private bool _drainWarmthRunning;
        private bool _cooldownRunning;
        protected int WarmthCost;

        [Inject]
        private void Construct(WarmthSystem warmthSystem)
        {
            _warmthSystem = warmthSystem;
            WarmthCost = MaxWarmthCost;
        }

        public void UseAbility()
        {
            if (_warmthSystem.CheckWarmCost(WarmthCost)  && !_drainWarmthRunning && !_cooldownRunning)
            {
                Enabled = true;
                StartAbility?.Invoke();
                
                if (string.IsNullOrEmpty(MethodName))
                    MethodName = BaseMethodName;
                GetType().GetMethod(MethodName)?.Invoke(this, null);
                DrainWarmth();
            }
        }
        
        public void StopAbility()
        {
            Enabled = false;
            _drainWarmthRunning = false;
            EndAbility?.Invoke();
            CooldownTimer();
        }
        
        private async void DrainWarmth()
        {
            if (_drainWarmthRunning) return;
            _drainWarmthRunning = true;
            
            while (Enabled && _drainWarmthRunning && _warmthSystem.CheckWarmCost(WarmthCost))
            {
                _warmthSystem.DecreaseWarmth(WarmthCost);
                await UniTask.Delay(TimeSpan.FromSeconds(Tick));
            }
            
            _drainWarmthRunning = false;
            StopAbility();
        }

        private async void CooldownTimer()
        {
            _cooldownRunning = true;
            await UniTask.Delay(TimeSpan.FromSeconds(Cooldown));
            _cooldownRunning = false;
        }
    }
}