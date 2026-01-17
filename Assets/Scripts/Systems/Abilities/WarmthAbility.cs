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
        [SerializeField] private int _warmthCost;
        [SerializeField, Range(0f, 2)] protected float _drainTick;
        [SerializeField, Range(0f, 5)] protected float _cooldown;
        [field: SerializeReference] public string BaseMethodName { get; private set; }
        public bool Enabled { get; set; }
        public Action StartAbility { get; set; }
        public Action UsingAbility { get; set; }
        public Action EndAbility { get; set; }
        public string MethodName { get; set; }

        [Inject] private WarmthSystem _warmthSystem;

        public void UseAbility()
        {
            MethodName ??= BaseMethodName;
            GetType().GetMethod(MethodName)?.Invoke(this, null);
            Enabled = true;
            StartAbility?.Invoke();
            DrainWarmth();
        }
        
        public void StopAbility()
        {
            Enabled = false;
            EndAbility?.Invoke();
        }
        
        public async void DrainWarmth()
        {
            while (Enabled)
            {
                _warmthSystem.DecreaseWarmth(_warmthCost);
                await UniTask.Delay(TimeSpan.FromSeconds(_cooldown));
            }
        }
    }
}