using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Data;
using DG.Tweening;
using EasyTextEffects.Editor.MyBoxCopy.Extensions;
using ModestTree;
using Systems;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Entities.PlayerScripts
{
    public class PlayerStateController : MonoBehaviour
    {
        private static readonly int IsSleeping = Animator.StringToHash("IsSleeping");
        private static readonly int HugIndex = Animator.StringToHash("Hug");

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
            BonesActive(false);
            _player.Animator.SetBool(IsSleeping, false);
            
            await UniTask.Delay(TimeSpan.FromSeconds(_player.Animator.GetCurrentAnimatorClipInfo(0)[0].clip.length - 0.1f));
            if (_player.Animator.GetBool(IsSleeping)) return;
            
            _input.SwitchCurrentActionMap("Player");
            _abilityController.EnableLastAbilities();
            BonesActive(true);
        }
        
        public void Sleep()
        {
            BonesActive(false);
            _player.Animator.SetBool(IsSleeping, true);
            
            _abilityController.DisableAllAbilities();
        }

        public async void Hug(Transform character)
        {
            BonesActive(false);

            _player.Rigidbody.transform.DORotate(
                Quaternion.FromToRotation(_player.Rigidbody.transform.position, character.position).eulerAngles, 0.5f);
            _player.Animator.SetTrigger(HugIndex);
            await UniTask.Delay(TimeSpan.FromSeconds(_player.Animator.GetCurrentAnimatorClipInfo(0)[0].clip.length - 0.1f) * 2);
            
            BonesActive(true);
        }

        private void BonesActive(bool active)
        {
            if(_rbs.Count == 0) _rbs = _player.GetComponentsInChildren<Rigidbody2D>().ToList();
            _rbs.ForEach(x =>
            {
                x.bodyType = active? RigidbodyType2D.Dynamic:RigidbodyType2D.Static;
                x.simulated = active;
            });
            _player.Animator.enabled = !active;
        }

        public void Die()
        {
            _sceneSystem.Die();
        }
    }
}
