using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Data;
using Data.Player;
using EasyTextEffects.Editor.MyBoxCopy.Extensions;
using Entities.Sound;
using Interfaces;
using Systems;
using Systems.Abilities.Concrete;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Entities.PlayerScripts
{
    public class Player : MonoBehaviour
    {
        private static readonly int IsSleeping = Animator.StringToHash("IsSleeping");
        [field: SerializeReference] public Rigidbody2D Rigidbody { get; private set;}
        [field: SerializeReference] public ObjectSfx ObjectSfx { get; private set;}
        [field: SerializeReference] public Animator Animator { get; private set;}
        [SerializeField] private bool _sleepAwake;

        private GlobalData _globalData;
        private PlayerConfig _config;
        private DiContainer _container;

        private DashAbility _dashAbility;
        private PlayerMovement _movement;

        private List<IAbility> _disabledAbilities = new();
        private List<IDisposable> _disposables = new();
        private List<Rigidbody2D> _rbs = new();

        [SerializeField] private Rigidbody2D _originalPlayerRb;  
        [SerializeField] private Rigidbody2D _swarmRb; 
        private PlayerInput _input;

        [Inject]
        private void Construct(GlobalData globalData, PlayerConfig config, DiContainer container, PlayerInput input)
        {
            _globalData = globalData;
            _config = config;
            _container = container;
            _input = input;
        }

        private void Start()
        {
            foreach (var ability in _config.Abilities)
            {
                _container.Inject(ability);

                if (ability is IDisposable disposable)
                    _disposables.Add(disposable);

                if (ability.Visual != null)
                {
                    _container.Inject(ability.Visual);
                    if (ability.Visual is IDisposable disposableVisual)
                        _disposables.Add(disposableVisual);
                }
            }

            _movement = (PlayerMovement)_config.Abilities[0];
            if (_config.Abilities.Count > 4)
                _dashAbility = (DashAbility)_config.Abilities[5];

            _rbs = GetComponentsInChildren<Rigidbody2D>().ToList();
            if (_sleepAwake) Sleep();
            else WakeUp();
            _originalPlayerRb = Rigidbody;
        }

        private void FixedUpdate()
        {
            _movement?.FixedTick();
            _dashAbility?.Tick();
        }
        public void StartResonance(Rigidbody2D swarmRb)
        {
            if (swarmRb == null)
            {
                return;
            }
            
            _swarmRb = swarmRb;
            
            Rigidbody = _swarmRb;
            
        }
        public void StopResonance()
        {
            if (_originalPlayerRb != null)
                Rigidbody = _originalPlayerRb;

            _swarmRb = null;
            
        }
        public void DisableAllAbilities()
        {
            foreach (var ability in _config.Abilities)
            {
                if (ability.Enabled)
                {
                    _disabledAbilities.Add(ability);
                    ability.Enabled = false;
                }
            }
        }

        public void EnableLastAbilities()
        {
            foreach (var ability in _disabledAbilities)
            {
                ability.Enabled = true;
            }
            _disabledAbilities.Clear();
        }

        private void OnDestroy()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }
        
        public async void WakeUp()
        {
            _input.SwitchCurrentActionMap("UI");
            DisableAllAbilities();
            Animator.enabled = true;
            Animator.SetBool(IsSleeping, false);
            _rbs.ForEach(x => x.simulated = false);
            await UniTask.Delay(3000);
            
            _input.SwitchCurrentActionMap("Player");
            
            Animator.enabled = false;
            EnableLastAbilities();
            _rbs.ForEach(x => x.simulated = true);
        }
        
        public void Sleep()
        {
            Animator.enabled = true;
            Animator.SetBool(IsSleeping, true);
            _rbs.ForEach(x => x.simulated = false);
            DisableAllAbilities();
        }

        void Update() {
            Debug.Log("Cur map = " + _input.currentActionMap);
        }

        public async void Die()
        {
            var pos = new List<Vector3>();
            var systemPos = _globalData.Get<SavablePlayerData>().RespawnPositions;

            foreach (var p in systemPos)
                pos.Add(p.ToUnity());

            var rbParent = Rigidbody.transform.parent;
            rbParent.position = pos.GetRandom() - Rigidbody.transform.position + Rigidbody.transform.parent.position;
        }
    }
}
