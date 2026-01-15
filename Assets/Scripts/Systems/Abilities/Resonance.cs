using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Entities.PlayerScripts;
using Interfaces;
using Systems.Swarm;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;


namespace Systems.Abilities.Concrete
{
    [Serializable]
    public class ResonanceAbility : BaseAbility
    {
        [SerializeField] private float _searchRadius = 12f;
        [SerializeField] private float _interactRadius = 5f;
        [SerializeField] private float _warmthDrain = 1f;
        [SerializeField] private float _drainInterval = 0.5f;
       
        [SerializeField] private Vector2 _returnPosition;
        private IWarmthSystem _warmthSystem;
        private Transform _playerTransform;
        private Player _player;
        private CinemachineCamera _vCam;
        private PlayerInput _input;
        private SwarmController _activeSwarm;
        private Vector2 _moveInput;

        private CancellationTokenSource _tickCts;
        private CancellationTokenSource _warmthCts;
        
        private Rigidbody2D _originalPlayerRb;
        private Rigidbody2D _currentPlayerRb;

        [Inject]
        public void Construct(Player player, IWarmthSystem warmth, PlayerInput input, CinemachineCamera cam)
        {
            _player = player;
            _playerTransform = player.Rigidbody.transform;
            _originalPlayerRb = player.Rigidbody;
            _currentPlayerRb = player.Rigidbody;
            _warmthSystem = warmth;
            _input = input;
            _vCam = cam;

            var moveAction = _input.actions.FindAction("Move");
            if (moveAction != null)
            {
                moveAction.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
                moveAction.canceled += _ => _moveInput = Vector2.zero;
            }

            StartAbility += OnStart;
            EndAbility += OnEnd;
        }

        private void OnStart()
        {
            _activeSwarm = FindNearestSwarm();
            if (_activeSwarm != null)
            {
                _returnPosition = _activeSwarm.transform.position;
            }

            if (_activeSwarm == null)
                return;

            StartResonanceInternal(_activeSwarm.GetComponent<Rigidbody2D>());
            _activeSwarm.SetControlled(true);

            if (_vCam != null)
            {
                _vCam.Follow = _activeSwarm.transform;
                _vCam.LookAt = _activeSwarm.transform;
            }

            // Запуск цикла обработки роя
            _tickCts?.Cancel();
            _tickCts = new CancellationTokenSource();
            ControlSwarm(_tickCts.Token).Forget();

            // Запуск цикла траты тепла раз в _drainInterval
            _warmthCts?.Cancel();
            _warmthCts = new CancellationTokenSource();
            DrainWarmthPeriodically(_warmthCts.Token).Forget();
        }

        private async UniTaskVoid DrainWarmthPeriodically(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    ApplyWarmthDrain();
                    await UniTask.Delay(TimeSpan.FromSeconds(_drainInterval), cancellationToken: token);
                }
            }
            catch (OperationCanceledException) { }
        }

        private void ApplyWarmthDrain()
        {
            int cost = Mathf.FloorToInt(_warmthDrain);

            if (cost <= 0)
                return;

            if (!_warmthSystem.CheckWarmCost(cost))
            {
                EndAbility?.Invoke();
                return;
            }

            _warmthSystem.DecreaseWarmth(cost);
        }

        private async UniTaskVoid ControlSwarm(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested && Enabled)
                {
                    if (_activeSwarm == null)
                        break;

                    ProcessSwarmInteraction();
                    await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: token); // маленький тик для обновления движения
                }
            }
            catch (OperationCanceledException) { }
        }

        private void ProcessSwarmInteraction()
        {
            if (_activeSwarm == null)
                return;

            bool anyNear = false;
            var neighbors = _activeSwarm.GetNeighbors(null);
            Vector2 playerPos = _playerTransform.position;

            foreach (var boid in neighbors)
            {
                float dist = Vector2.Distance(playerPos, boid.transform.position);
                if (dist <= _interactRadius)
                {
                    anyNear = true;
                    boid.InteractWithPhysicsObjects();
                }
            }

            _activeSwarm.SetControlInput(anyNear ? _moveInput : Vector2.zero);
        }

        private void OnEnd()
        {
            var swarmToReturn = _activeSwarm;
            Vector2 returnPos = _returnPosition;
            
            _tickCts?.Cancel();
            _warmthCts?.Cancel();

            if (_activeSwarm != null)
            {
                _activeSwarm.SetControlled(false);
            }

            StopResonanceInternal();

            if (_vCam != null && _playerTransform != null)
            {
                _vCam.Follow = _playerTransform;
                _vCam.LookAt = _playerTransform;
            }

            _moveInput = Vector2.zero;
            _activeSwarm = null;

            if (swarmToReturn != null)
            {
                ReturnSwarmToPosition(swarmToReturn, returnPos, 0.5f).Forget();
            }
        }

        private SwarmController FindNearestSwarm()
        {
            var swarms = UnityEngine.Object.FindObjectsByType<SwarmController>(FindObjectsSortMode.None);
            SwarmController nearest = null;

            float bestDist = float.MaxValue;
            Vector2 p = _playerTransform.position;

            foreach (var s in swarms)
            {
                float d = Vector2.Distance(p, s.transform.position);
                if (d < bestDist && d <= _searchRadius)
                {
                    bestDist = d;
                    nearest = s;
                }
            }

            return nearest;
        }
        
        private async UniTask ReturnSwarmToPosition(SwarmController swarm, Vector2 target, float duration)
        {
            if (swarm == null) return;

            Vector2 start = swarm.transform.position;
            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime / duration;
                swarm.transform.position = Vector2.Lerp(start, target, t);
                await UniTask.Yield();
            }

            swarm.transform.position = target;
        }
        
        private void StartResonanceInternal(Rigidbody2D swarmRb)
        {
            if (swarmRb == null)
                return;
            
            _currentPlayerRb = swarmRb;
            _player.Rigidbody = _currentPlayerRb;
        }
        
        private void StopResonanceInternal()
        {
            if (_originalPlayerRb != null)
            {
                _currentPlayerRb = _originalPlayerRb;
                _player.Rigidbody = _originalPlayerRb;
            }
        }
    }
}
