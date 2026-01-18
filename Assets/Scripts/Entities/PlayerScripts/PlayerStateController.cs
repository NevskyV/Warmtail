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
            _player.Animator.enabled = true;
            _player.Animator.SetBool(IsSleeping, false);
            _rbs.ForEach(x => x.simulated = false);
            await UniTask.Delay(3000);
            
            _input.SwitchCurrentActionMap("Player");
            
            _player.Animator.enabled = false;
            _abilityController.EnableLastAbilities();
            _rbs.ForEach(x => x.simulated = true);
        }
        
        public void Sleep()
        {
            _player.Animator.enabled = true;
            _player.Animator.SetBool(IsSleeping, true);
            _rbs.ForEach(x => x.simulated = false);
            _abilityController.DisableAllAbilities();
        }

        public void Die()
        {
            _sceneSystem.Die();
        }
    }
}
