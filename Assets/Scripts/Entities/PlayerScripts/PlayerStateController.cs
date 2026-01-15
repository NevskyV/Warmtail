using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Data;
using Data.Player;
using EasyTextEffects.Editor.MyBoxCopy.Extensions;
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
        private GlobalData _globalData;
        private PlayerInput _input;
        private List<Rigidbody2D> _rbs = new();
        
        [Inject]
        private void Construct(GlobalData globalData, PlayerInput input, Player player, PlayerAbilityController abilityController)
        {
            _globalData = globalData;
            _input = input;
            _player = player;
            _abilityController = abilityController;
        }

        public void Initialize(bool sleepAwake)
        {
            if (_player == null)
            {
                Debug.LogError("PlayerStateController: Player is not injected!");
                return;
            }
            
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
            var pos = new List<Vector3>();
            var systemPos = _globalData.Get<SavablePlayerData>().RespawnPositions;

            foreach (var p in systemPos)
                pos.Add(p.ToUnity());

            var rbParent = _player.Rigidbody.transform.parent;
            rbParent.position = pos.GetRandom() - _player.Rigidbody.transform.position + _player.Rigidbody.transform.parent.position;
        }
    }
}
