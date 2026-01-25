using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Entities.PlayerScripts;
using Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Systems.Abilities
{
    [Serializable]
    public class WarmingAbility : WarmthAbility
    {
        [Header("Normal Warm")]
        [SerializeField] private float _radius = 3f;
        
        [Header("Explosion")]
        [SerializeField] private float _explosionDuration = 0.5f;
        [SerializeField] private int _explosionCost = 50;
        
        private WarmthSystem _warmthSystem;
        private GamepadRumble _rumble;
        private bool _isRunning;
        private int _maxWarmthCost;
        private AbilityTriggerZone<Warmable> _triggerZone;
        
        [Inject]
        public void Construct(Player player,  WarmthSystem warmthSystem, GamepadRumble gamepadRumble)
        {
            _warmthSystem = warmthSystem;
            _rumble = gamepadRumble;
            _maxWarmthCost = WarmthCost;
            _triggerZone = GetOrCreateTriggerZone(player, "WarmingTrigger", _radius);
            _triggerZone.Wake();
            EndAbility += StopWarm;
        }
        
        private AbilityTriggerZone<Warmable> GetOrCreateTriggerZone(Player player, string name, float radius)
        {
            var triggerObj = player.Rigidbody.transform.Find(name)?.gameObject;
            if (triggerObj == null)
            {
                triggerObj = new GameObject(name);
                triggerObj.transform.SetParent(player.Rigidbody.transform);
                triggerObj.transform.localPosition = Vector3.zero;
            }
            
            return new AbilityTriggerZone<Warmable>(triggerObj, radius);
        }
        
        public void StartWarm()
        {
            if (_isRunning) return;
            _isRunning = true;
            ActiveRoutine().Forget();
        }
        
        public void PerformExplosion()
        {
            PerformExplosionInternal().Forget();
        }
        
        private void StopWarm()
        {
            Gamepad.current.ResetHaptics();
            _isRunning = false;
        }
        
        private async UniTaskVoid ActiveRoutine()
        {
            while (Enabled && _isRunning)
            {
                PerformTick();
                await UniTask.Delay(TimeSpan.FromSeconds(Tick));
            }
        }

        private async UniTask PerformExplosionInternal()
        {
            if (!_warmthSystem.CheckWarmCost(_explosionCost))
            { 
                StopWarm();
                return;
            }
            _rumble.EnableRumble();
            _warmthSystem.DecreaseWarmth(_explosionCost);
            float timer = 0;
            
            while (timer < _explosionDuration)
            {
                timer += Time.deltaTime;

                WarmObjectsInRadius(true);
                await UniTask.Yield();
            }
            _rumble.DisableRumble();
            StopWarm();
        }

        private void PerformTick()
        {
            if (WarmObjectsInRadius())
            {
                _rumble.EnableRumble();
                WarmthCost = _maxWarmthCost;
                UsingAbility?.Invoke();
            }
            else
            {
                _rumble.DisableRumble();;
                WarmthCost = 0;
            }
        }
        

        private bool WarmObjectsInRadius(bool complete = false)
        {
            var warmableObjects = _triggerZone.ObjectsInRange.ToList();
            
            for (int i = 0; i < warmableObjects.Count; i++)
            {
                if (warmableObjects[i] == null) continue;
                if(complete)
                    warmableObjects[i].WarmComplete();
                else
                    warmableObjects[i].Warm();
            }
            
            return warmableObjects.Count > 0;
        }
    }
}

