using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Player;
using Entities.PlayerScripts;
using Entities.UI;
using Interfaces;
using Systems;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
using Systems.Abilities;
using Systems.Environment;
using Systems.Swarm;
using Unity.Cinemachine;

namespace Entities.Core
{
    public class NormalSceneInstaller : MonoInstaller
    {
        [SerializeField] private DialogueVisuals _dialogueVisuals;
        [SerializeField] private MonologueVisuals _monologueVisuals;
        [SerializeField] private Player _player; 
        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private PlayerConfig _playerConfig;
        [SerializeField] private PopupVisuals _popupVisuals;
        [SerializeField] private UIStateSystem _uiStateSystem;
        [SerializeField] private CinemachineCamera _cam;
        [SerializeField] private SurfacingSystem _surfacingSystem;
        [SerializeField] private SwarmController _swarmController;
        [SerializeField] private FreezeVisuals _freezeVisuals;
        [SerializeField] private QuestVisuals _questVisuals;
        [SerializeField] private ComboConfig _comboConfig;
        private QuestSystem _questSystem = new();
       
        public override void InstallBindings()
        {
            Debug.Log("set ui installer NORMAL");
            Container.Bind<SwarmController>().FromInstance(_swarmController).AsSingle();
            Container.Bind<SurfacingSystem>().FromInstance(_surfacingSystem).AsSingle();
            Container.Bind<PlayerConfig>().FromInstance(_playerConfig).AsSingle();
            Container.Bind<Player>().FromInstance(_player).AsSingle();
            Container.Bind<PlayerStateController>().FromNewComponentOn(_player.gameObject).AsSingle().NonLazy();
            Container.Bind<PlayerAbilityController>().FromNewComponentOn(_player.gameObject).AsSingle().NonLazy();
            Container.Bind<PlayerInput>().FromInstance(_playerInput).AsSingle();
            Container.Bind<DialogueVisuals>().FromInstance(_dialogueVisuals).AsSingle();
            Container.Bind<MonologueVisuals>().FromInstance(_monologueVisuals).AsSingle();
            Container.Bind<PopupVisuals>().FromInstance(_popupVisuals).AsSingle();
            Container.Bind<UIStateSystem>().FromInstance(_uiStateSystem).AsSingle();
            Container.Bind<CinemachineCamera>().FromInstance(_cam).AsSingle();
            Container.Bind<FreezeVisuals>().FromInstance(_freezeVisuals).AsSingle();
            Container.Bind<QuestVisuals>().FromInstance(_questVisuals).AsSingle();
            Container.Bind<ComboConfig>().FromInstance(_comboConfig).AsSingle();
            
            Container.Bind<DialogueSystem>().FromNew().AsSingle();
            Container.BindInterfacesAndSelfTo<WarmthSystem>().FromNew().AsSingle();
            Container.Bind<IPlayerDataProvider>().To<PlayerDataProvider>().FromNew().AsSingle();
            Container.BindInterfacesAndSelfTo<DailySystem>().FromNew().AsSingle();
            Container.Bind<QuestSystem>().FromInstance(_questSystem).AsSingle();
            Container.Inject(_questSystem);
            
            Container.BindInterfacesAndSelfTo<DashAbility>().FromInstance(_playerConfig.Abilities
                .OfType<DashAbility>().First()).AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerMovement>().FromInstance(_playerConfig.Abilities
                .OfType<PlayerMovement>().First()).AsSingle();
            
            Container.Bind<List<IAbility>>()
                .FromInstance(_playerConfig.Abilities)
                .AsSingle();
            
            Container.BindInterfacesAndSelfTo<AbilitiesSystem>().AsSingle().NonLazy();
            Container.Bind<ComboSystem>().FromNew().AsSingle().NonLazy();
            
            Container.Bind<SceneLoader>().FromComponentInHierarchy().AsSingle();
        }
    }
}
