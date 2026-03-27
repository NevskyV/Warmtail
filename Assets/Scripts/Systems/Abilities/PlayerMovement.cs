using System;
using Data;
using Data.Player;
using Entities.PlayerScripts;
using Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Systems.Abilities
{
    [Serializable]
    public class PlayerMovement : IAbility, IFixedTickable
    {
        public bool Enabled { get; set; } = true;
        public Action StartAbility { get; set; }
        public Action UsingAbility { get; set; }
        public Action EndAbility { get; set; }
        [field: SerializeReference] public IAbilityVisual Visual { get; set; }

        [Header("Movement Settings")]
        public float MoveForce = 100f;

        [SerializeField] private float _moreForge = 100f;
        [SerializeField] private float _drag = 5f;

        [Header("Acceleration")]
        [SerializeField] private float _accelerationTime = 0.5f;
        [SerializeField] private AnimationCurve _accelerationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private float _accelerationTimer;

        private Vector2 _moveInput;
        private GlobalData _globalData;
        private Player _player;

        [Inject]
        public void Construct(Player player, PlayerInput playerInput, GlobalData data)
        {
            Enabled = true;
            _globalData = data;
            _player = player;

            var rb = _player.Rigidbody;
            rb.angularDamping = _drag;
            rb.linearDamping = _drag;

            if (playerInput != null && playerInput.actions != null && playerInput.actions["Move"] != null)
            {
                var moveAction = playerInput.actions["Move"];
                moveAction.started += _ => { if (Enabled) StartAbility?.Invoke(); };
                moveAction.performed += OnMove;
                moveAction.canceled += OnMoveCanceled;
            }
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            if (Enabled)
            {
                _moveInput = context.ReadValue<Vector2>();
            }
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            _moveInput = Vector2.zero;
            _accelerationTimer = 0f;
            EndAbility?.Invoke();
        }

        public void FixedTick()
        {
            var rb = _player.Rigidbody;
            if (!rb || !Enabled) return;

            if (_moveInput.magnitude > 0.1f)
            {
                UsingAbility?.Invoke();

                _accelerationTimer += Time.fixedDeltaTime;
                float normalizedTime = Mathf.Clamp01(_accelerationTimer / _accelerationTime);
                float accelerationMultiplier = _accelerationCurve.Evaluate(normalizedTime);

                Vector2 force = _moveInput * (MoveForce * accelerationMultiplier);
                rb.AddForce(force * _moreForge, ForceMode2D.Force);

                float targetAngle = Mathf.Atan2(_moveInput.y, _moveInput.x) * Mathf.Rad2Deg;
                float newAngle = Mathf.LerpAngle(rb.rotation, targetAngle, 1.5f * Time.fixedDeltaTime);
                rb.MoveRotation(newAngle);
            }
            else
            {
                _accelerationTimer = 0f;
            }
        }
    }
}
