using Data.Player;
using Entities.Sound;
using Systems;
using Systems.Abilities.Concrete;
using UnityEngine;
using Zenject;

namespace Entities.PlayerScripts
{
    public class Player : MonoBehaviour
    {
        [field: SerializeReference] public Rigidbody2D Rigidbody { get; set;}
        [field: SerializeReference] public ObjectSfx ObjectSfx { get; private set;}
        [field: SerializeReference] public Animator Animator { get; private set;}
        [SerializeField] private bool _sleepAwake;
        
        private PlayerStateController _stateController;
        private PlayerAbilityController _abilityController;
        
        private bool _isInitialized;

        [Inject]
        private void Construct(PlayerStateController stateController, PlayerAbilityController abilityController)
        {
            _stateController = stateController;
            _abilityController = abilityController;
        }

        private void Start()
        {
            if (_isInitialized)
                return;
                
            Initialize();
        }

        private void Initialize()
        {
            _isInitialized = true;
            
            if (_abilityController == null || _stateController == null)
            {
                Debug.LogError("Player controllers are not injected properly!");
                return;
            }
            _abilityController.Initialize();
            _stateController.Initialize(_sleepAwake);
        }
    }
}
