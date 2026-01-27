using System;
using Cysharp.Threading.Tasks;
using Data.Player;
using Entities.PlayerScripts;
using Interfaces;
using Systems.Environment;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Systems.Abilities
{
    [Serializable]
    public class DashAbility : WarmthAbility, IFixedTickable
    {
        [SerializeField] private float _destroyRadius = 1.5f;
        [SerializeField] private float _dashCooldownDuration = 1f;
        [SerializeField] private int _normalSpeed = 60;
        [SerializeField] private int _dashSpeed = 100;
        private float _lastDashTime = -Mathf.Infinity;
        private UniTask _dashTask;
        private bool _dashLoopRunning;

        private PlayerConfig _playerConfig;
        private Rigidbody2D _playerRb;
        private SurfacingSystem _surfacingSystem;
        private GamepadRumble _rumble;
        private CinemachineBasicMultiChannelPerlin _camNoise;

        private Vector2 _moveInput;
        private float _layerInput;
        private bool _applyCost;
        [Inject]
        public void Construct(PlayerConfig playerConfig, Player player, CinemachineCamera cam, SurfacingSystem surfacing,
            PlayerInput input, DiContainer container, GamepadRumble gamepadRumble)
        {

            _playerRb = player.Rigidbody;
            _surfacingSystem = surfacing;
            _playerConfig = playerConfig;
            _rumble = gamepadRumble;
            _camNoise = cam.GetComponent<CinemachineBasicMultiChannelPerlin>();

            input.actions["Move"].performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
            input.actions["Move"].canceled += _ => _moveInput = Vector2.zero;
            
            input.actions["Surfacing"].performed += ctx => _layerInput = ctx.ReadValue<float>();
            input.actions["Surfacing"].canceled += _ => _layerInput = 0;
        }

        public void ActivateDash()
        {
            _applyCost = true;
            WarmthCost = 0;
            StartAbility?.Invoke();
        }

        public void ActivateDashNoCost()
        {
            _applyCost =  false;
            WarmthCost = 0;
            StartAbility?.Invoke();
        }

        public void FixedTick()
        {
            if (!Enabled) return;
            
            if (Mathf.Abs(_layerInput) > 0.1f)
            {
                _rumble.ShortRumble();
                int dir = (int)Mathf.Sign(_layerInput);

                if (_surfacingSystem.TryChangeLayer(dir))
                {
                    WarmthCost = MaxWarmthCost;

                    _layerInput = 0;
                }
            }

            if (_moveInput.magnitude > 0.1f)
            {
                if (!_dashLoopRunning)
                {
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
                    _camNoise.enabled = true;
                    WarmthCost = _applyCost? MaxWarmthCost : 0;
                    Dash();
                    await UniTask.Delay(500);
                }
            }
            finally
            {
                _camNoise.enabled = false;
                _dashLoopRunning = false;
                _rumble.DisableRumble();
                ((PlayerMovement)_playerConfig.Abilities[0]).MoveForce = _normalSpeed;
                await UniTask.Delay(TimeSpan.FromSeconds(_dashCooldownDuration));
            }
        }

        private void Dash()
        {
            _rumble.EnableRumble();
            DestroyObstaclesInRadius(_destroyRadius);
            ((PlayerMovement)_playerConfig.Abilities[0]).MoveForce = _dashSpeed;
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

