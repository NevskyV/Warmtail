using System;
using Data.Player;
using Entities.PlayerScripts;
using Interfaces;
using Systems.Environment;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
using Cysharp.Threading.Tasks;


namespace Systems.Abilities.Concrete
{
    [Serializable]
    public class DashAbility : BaseAbility, ITickable
    {
        [SerializeField] private int _dashCost = 15;
        [SerializeField] private float _destroyRadius = 1.5f;
        [SerializeField] private float _dashCooldownDuration = 1f;
        [SerializeField] private int _normalSpeed = 60;
        [SerializeField] private int _dashSpeed = 100;
        [SerializeField] private int _surfacingCost = 15;
        private float _lastDashTime = -Mathf.Infinity;
        private bool _canDash = true;
        private UniTask _dashTask;
        private bool _dashLoopRunning;
       

        private PlayerConfig _playerConfig;
        private Rigidbody2D _playerRb;
        private SurfacingSystem _surfacingSystem;
        private WarmthSystem _warmthSystem;

        private Vector2 _moveInput;
        private float _layerInput;
        private bool IsFree => IsComboActive && _secondaryComboType == typeof(MetabolismAbility);
        [Inject]
        public void Construct(PlayerConfig playerConfig, Player player, WarmthSystem warmth, SurfacingSystem surfacing,
            PlayerInput input, DiContainer container)
        {

            _playerRb = player.Rigidbody;
            _surfacingSystem = surfacing;
            _warmthSystem = warmth;
            _playerConfig = playerConfig;

            input.actions["Move"].performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
            input.actions["Move"].canceled += _ => _moveInput = Vector2.zero;
            
            input.actions["Surfacing"].performed += ctx => _layerInput = ctx.ReadValue<float>();
            input.actions["Surfacing"].canceled += _ => _layerInput = 0;

            UsingAbility += Tick;
        }
        

        public void Tick()
        {
            if (!Enabled) return;

            if (Mathf.Abs(_layerInput) > 0.1f)
            {
                int dir = (int)Mathf.Sign(_layerInput);

                if (_surfacingSystem.TryChangeLayer(dir))
                {
                    if (ShouldApplyCost())
                        _warmthSystem.DecreaseWarmth(_surfacingCost);

                    _layerInput = 0;
                }
            }

            if (_moveInput.magnitude > 0.1f)
            {
                if (!_dashLoopRunning && _canDash)
                {
                    _canDash = false;  
                    _dashLoopRunning = true;
                    _dashTask = DashLoop();
                }
            }
        }

        
        private async UniTask DashLoop()
        {
            try
            {
                while (Enabled && _dashLoopRunning && _moveInput.magnitude > 0.1f)
                {
                    Dash();

                    if (ShouldApplyCost())
                        _warmthSystem.DecreaseWarmth(_dashCost);

                    await UniTask.Delay(500);
                }
            }
            finally
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_dashCooldownDuration));
                _canDash = true;
                _dashLoopRunning = false;
                ((PlayerMovement)_playerConfig.Abilities[0]).MoveForce = _normalSpeed;
            }
        }



        private void Dash()
        {
            DestroyObstaclesInRadius(_destroyRadius);
            ((PlayerMovement)_playerConfig.Abilities[0]).MoveForce = _dashSpeed;
        }

        private bool ShouldApplyCost()
        {
            return !IsFree;
        }

        private void DestroyObstaclesInRadius(float radius)
        {
            var hits = Physics2D.OverlapCircleAll(_playerRb.position, radius);
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<IDestroyable>(out var dest))
                    dest.DestroyObject();
            }
        }
    }
    
}

