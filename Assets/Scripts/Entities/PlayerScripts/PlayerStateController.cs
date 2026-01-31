using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Data;
using Systems;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Entities.PlayerScripts
{
    public class PlayerStateController : MonoBehaviour
    {
        private static readonly int IsSleeping = Animator.StringToHash("IsSleeping");
        
        private Player _player;
        private PlayerAbilityController _abilityController;
        private PlayerInput _input;
        private List<Rigidbody2D> _rbs = new();
        private SceneSystem _sceneSystem;
        
        [Inject]
        private void Construct(PlayerInput input, Player player, PlayerAbilityController abilityController, SceneSystem sceneSystem)
        {
            _input = input;
            _player = player;
            _sceneSystem = sceneSystem;
            _abilityController = abilityController;
        }

        public void Initialize(bool sleepAwake)
        {
            if (_player == null)
            {
                Debug.LogError("PlayerStateController: Player is not injected!");
                return;
            }
            _sceneSystem.Spawn();
            _rbs = _player.GetComponentsInChildren<Rigidbody2D>().ToList();
            if (sleepAwake) Sleep();
            else WakeUp();
        }
        
        public async void WakeUp()
        {
            _input.SwitchCurrentActionMap("UI");
            _abilityController.DisableAllAbilities();
            _rbs.ForEach(x =>
            {
                x.bodyType = RigidbodyType2D.Static;
                x.simulated = false;
            });
            
            _player.Animator.enabled = true;
            _player.Animator.SetBool(IsSleeping, false);
            
            await UniTask.Delay(TimeSpan.FromSeconds(_player.Animator.GetCurrentAnimatorClipInfo(0)[0].clip.length - 0.1f));
            
            _input.SwitchCurrentActionMap("Player");
            
            _player.Animator.enabled = false;
            _abilityController.EnableLastAbilities();
            _rbs.ForEach(x =>
            {
                x.bodyType = RigidbodyType2D.Dynamic;
                x.simulated = true;
            });
        }
        
        public void Sleep()
        {
            if(_rbs.Count == 0) _rbs = _player.GetComponentsInChildren<Rigidbody2D>().ToList();
            _rbs.ForEach(x =>
            {
                x.bodyType = RigidbodyType2D.Static;
                x.simulated = false;
            });
            
            _player.Animator.enabled = true;
            _player.Animator.SetBool(IsSleeping, true);
            
            _abilityController.DisableAllAbilities();
        }

        public void Die()
        {
            _sceneSystem.Die();
        }
    }
}
