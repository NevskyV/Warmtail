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

        private PlayerConfig _config;
        private PlayerStateController _stateController;
        private PlayerAbilityController _abilityController;
        
        private DashAbility _dashAbility;
        private PlayerMovement _movement;
        
        private bool _isInitialized;

        [Inject]
        private void Construct(PlayerConfig config, PlayerStateController stateController, PlayerAbilityController abilityController)
        {
            _config = config;
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
            
            // 1. Сначала инжектим все abilities
            _abilityController.Initialize();
            
            // 2. Затем инициализируем ссылки на movement и dash
            if (_config != null && _config.Abilities != null && _config.Abilities.Count > 0)
            {
                _movement = (PlayerMovement)_config.Abilities[0];
                if (_config.Abilities.Count > 5)
                    _dashAbility = (DashAbility)_config.Abilities[5];
            }
            
            // 3. ТОЛЬКО ПОТОМ вызываем Sleep/WakeUp (они могут включать/выключать abilities)
            _stateController.Initialize(_sleepAwake);
        }

        private void FixedUpdate()
        {
            _movement?.FixedTick();
            _dashAbility?.FixedTick();
        }
    }
}
