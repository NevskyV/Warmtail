using System;
using Cysharp.Threading.Tasks;
using Entities.PlayerScripts;
using Interfaces;
using Systems.Abilities;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Systems.Abilities.Concrete
{
    [Serializable]
    public class WarmingAbility : BaseAbility
    {
        [Header("Normal Warm")]
        [SerializeField] private float _radius = 3f;
        [SerializeField] private int _cost = 5;
        
        [Header("Explosion")]
        [SerializeField] private float _explosionMaxRadius = 10f;
        [SerializeField] private float _explosionDuration = 0.5f;
        [SerializeField] private int _explosionCost = 50;

        private Transform _playerTransform;
        private IWarmthSystem _warmthSystem;
        private PlayerInput _playerInput;
        private bool _isRunning;
        private bool _canActivate;
        
        private WarmableTriggerZone _triggerZone;
        private CompositeDisposable _disposables = new();

        [Inject]
        public void Construct(Player player, PlayerInput playerInput, IWarmthSystem warmth)
        {
            _playerTransform = player.Rigidbody.transform;
            _warmthSystem = warmth;
            _playerInput = playerInput;
            
            // Найти или создать триггер-зону
            _triggerZone = GetOrCreateTriggerZone(player, "WarmingTrigger", _radius);
            
            // Подписаться на вход объектов в триггер
            _triggerZone.OnObjectEnter
                .Subscribe(_ => OnWarmableEntered())
                .AddTo(_disposables);

            StartAbility += StartWarm;
            EndAbility += StopWarm;
        }
        
        private WarmableTriggerZone GetOrCreateTriggerZone(Player player, string name, float radius)
        {
            var triggerObj = player.Rigidbody.transform.Find(name)?.gameObject;
            if (triggerObj == null)
            {
                triggerObj = new GameObject(name);
                triggerObj.transform.SetParent(player.Rigidbody.transform);
                triggerObj.transform.localPosition = Vector3.zero;
                triggerObj.AddComponent<CircleCollider2D>();
            }
            
            var zone = triggerObj.GetComponent<WarmableTriggerZone>();
            if (zone == null)
                zone = triggerObj.AddComponent<WarmableTriggerZone>();
            
            zone.SetRadius(radius);
            zone.SetActive(false); // Изначально выключен
            return zone;
        }
        
        private void OnWarmableEntered()
        {
            if (_isRunning)
                UsingAbility?.Invoke(); // Вызывать сразу при входе в радиус
        }

        private void StartWarm()
        {
            _triggerZone.SetActive(true); // Включить триггер
            ActiveRoutine().Forget();
            _canActivate = false;
        }
        
        private async void StopWarm()
        {
            _triggerZone.SetActive(false); // Выключить триггер
            _isRunning = false;
            await UniTask.Delay(1000);
            _canActivate = true;
        }
        
        private async UniTaskVoid ActiveRoutine()
        {
            if (!_canActivate) return;
            _isRunning = true;
            if (IsComboActive && _secondaryComboType == typeof(MetabolismAbility))
            {
                await PerformExplosion();
            }
            else
            {
                Debug.Log("warm");
                while (Enabled && _isRunning)
                {
                    PerformTick();
                    await UniTask.Delay(500);
                }
            }
        }

        private async UniTask PerformExplosion()
        {
            if (!_warmthSystem.CheckWarmCost(_explosionCost))
            { 
                StopWarm();
                return;
            }
            _warmthSystem.DecreaseWarmth(_explosionCost);
            float timer = 0;
            
            while (timer < _explosionDuration)
            {
                timer += Time.deltaTime;
                float currentRadius = Mathf.Lerp(0, _explosionMaxRadius, timer / _explosionDuration);
                
                var warmableObjects = FindWarmableObjects(_playerTransform.position, currentRadius);
                foreach (var warmable in warmableObjects)
                {
                    warmable.WarmComplete();
                }
                await UniTask.Yield();
            }

            StopWarm();
        }

        private void PerformTick()
        {
            if (!_warmthSystem.CheckWarmCost(_cost))
            {
                StopWarm();
                return;
            }

            if (WarmObjectsInRadius())
            {
                _warmthSystem.DecreaseWarmth(_cost);
                UsingAbility?.Invoke();
            }
        }

        // Используется только для взрыва (combo) с динамическим радиусом
        private System.Collections.Generic.List<Warmable> FindWarmableObjects(Vector2 position, float radius)
        {
            var hits = Physics2D.OverlapCircleAll(position, radius);
            var warmableObjects = new System.Collections.Generic.List<Warmable>();
            
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<Warmable>(out var warmable))
                {
                    warmableObjects.Add(warmable);
                }
            }
            
            return warmableObjects;
        }

        private bool WarmObjectsInRadius()
        {
            var warmableObjects = _triggerZone.ObjectsInRange;
            
            foreach (var warmable in warmableObjects)
            {
                warmable.Warm();
            }
            
            return warmableObjects.Count > 0;
        }
    }
}

