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
        [SerializeField, Range(0f, 1f)] protected float _drainPercentPerTick = 0.25f;
        [SerializeField, Range(0f, 2f)] protected float Tick;
        [SerializeField, Range(0f, 5f)] protected float Cooldown;
        [field: SerializeReference] public string BaseMethodName { get; private set; }
        public bool Enabled { get; set; }
        public bool InUse { get; set; }
        public Action StartAbility { get; set; }
        public Action UsingAbility { get; set; }
        public Action EndAbility { get; set; }
        public string MethodName { get; set; }
        public bool CooldownRunning => _cooldownRunning;

        private const float DrainInterval = 0.5f;

        private WarmthSystem _warmthSystem;
        private bool _drainWarmthRunning;
        private bool _cooldownRunning;

        protected float DrainPercent;

        [Inject]
        private void Construct(WarmthSystem warmthSystem)
        {
            _warmthSystem = warmthSystem;
            DrainPercent = _drainPercentPerTick;
        }

        public async void UseAbility()
        {
            if ((_warmthSystem.HasCells || DrainPercent == 0f) && !_drainWarmthRunning && !_cooldownRunning)
            {
                Enabled = true;
                StartAbility?.Invoke();

                if (string.IsNullOrEmpty(MethodName))
                    MethodName = BaseMethodName;

                DrainWarmth();
                await UniTask.Delay(TimeSpan.FromSeconds(Tick));
                GetType().GetMethod(MethodName)?.Invoke(this, null);
            }
        }

        public void StopAbility()
        {
            if (!Enabled) return;
            Enabled = false;
            _drainWarmthRunning = false;
            _warmthSystem.ConsumeCurrentCell();
            EndAbility?.Invoke();
            CooldownTimer();
        }

        private async void DrainWarmth()
        {
            if (_drainWarmthRunning) return;
            _drainWarmthRunning = true;

            while (Enabled && _drainWarmthRunning)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(DrainInterval));
                if (!Enabled || !_drainWarmthRunning) break;
                if (!_warmthSystem.DrainCurrentCell(DrainPercent)) break;
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
