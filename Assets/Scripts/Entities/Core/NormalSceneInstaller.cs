using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Player;
using Entities.House;
using Entities.PlayerScripts;
using Entities.UI;
using Interfaces;
using Systems;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
using Systems.Abilities;
using Systems.Fears;
using Systems.Environment;
using Systems.Swarm;
using TriInspector;
using Unity.Cinemachine;
using UnityEngine.InputSystem.UI;

namespace Entities.Core
{
    public class NormalSceneInstaller : MonoInstaller
    {
        [Title("Player")]
        [SerializeField] private Player _player; 
        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private PlayerConfig _playerConfig;
        [SerializeField] private CinemachineCamera _cam;
        
        [Title("Abilities")]
        [SerializeField] private ComboConfig _comboConfig;
        
        [Title("Visuals")]
        [SerializeField] private DialogueVisuals _dialogueVisuals;
        [SerializeField] private MonologueVisuals _monologueVisuals;
        [SerializeField] private QuestVisuals _questVisuals;
        [SerializeField] private TipsVisuals _tipsVisuals;
        [SerializeField] private MarksVisuals _marksVisuals;
        [SerializeField] private ShoppingVisuals _shoppingVisuals;
        [SerializeField] private MapVisuals _mapVisuals;
        
        [Title("UI")]
        [SerializeField] private InputSystemUIInputModule _uiInput;
        [SerializeField] private UIStateSystem _uiStateSystem;
        
        [Title("Map")]
        [SerializeField] private SwarmController _swarmController;
        [SerializeField] private SurfacingSystem _surfacingSystem;
       
        // ReSharper disable Unity.PerformanceAnalysis
        public override void InstallBindings()
        {
            BuildScene();
            BuildPlayer();
            BuildSystems();
            BuildVisuals();
            BuildAbilities();
            BuildUI();
        }

        private void BuildPlayer()
        {
            Container.Bind<PlayerConfig>().FromInstance(_playerConfig).AsSingle();
            Container.Bind<Player>().FromInstance(_player).AsSingle();
            Container.Bind<PlayerStateController>().FromNewComponentOn(_player.gameObject).AsSingle().NonLazy();
            Container.Bind<PlayerAbilityController>().FromNewComponentOn(_player.gameObject).AsSingle().NonLazy();
            Container.Bind<PlayerInput>().FromInstance(_playerInput).AsSingle();
            Container.Bind<CinemachineCamera>().FromInstance(_cam).AsSingle();
            Container.Bind<PlayerDataProvider>().FromNew().AsSingle();
        }

        private void BuildVisuals()
        {
            Container.Bind<DialogueVisuals>().FromInstance(_dialogueVisuals).AsSingle();
            Container.Bind<MonologueVisuals>().FromInstance(_monologueVisuals).AsSingle();
            Container.Bind<QuestVisuals>().FromInstance(_questVisuals).AsSingle();
            Container.Bind<TipsVisuals>().FromInstance(_tipsVisuals).AsSingle();
            Container.Bind<MarksVisuals>().FromInstance(_marksVisuals).AsSingle();
            Container.Bind<ShoppingVisuals>().FromInstance(_shoppingVisuals).AsSingle();
            Container.Bind<MapVisuals>().FromInstance(_mapVisuals).AsSingle();
        }

        private void BuildSystems()
        {
            Container.Bind<SceneSystem>().FromNew().AsSingle();
            Container.Bind<EventsData>().FromNew().AsSingle();
            Container.Bind<DialogueSystem>().FromNew().AsSingle();
            Container.Bind<ScreenshotSystem>().FromNew().AsSingle();
            Container.Bind<SurfacingSystem>().FromInstance(_surfacingSystem).AsSingle();
            Container.BindInterfacesAndSelfTo<WarmthSystem>().FromNew().AsSingle();
            Container.BindInterfacesAndSelfTo<TemperatureSystem>().FromNew().AsSingle();
            Container.BindInterfacesAndSelfTo<DailySystem>().FromNew().AsSingle();
            Container.Bind<GamepadRumble>().FromNew().AsSingle();
            Container.BindInterfacesAndSelfTo<FearBuffSystem>().FromNew().AsSingle().NonLazy();
            
            QuestSystem questSystem = new();
            Container.Bind<QuestSystem>().FromInstance(questSystem).AsSingle();
            Container.Inject(questSystem);
        }

        private void BuildAbilities()
        {
            Container.Bind<ComboConfig>().FromInstance(_comboConfig).AsSingle();
            Container.Bind<ComboSystem>().FromNew().AsSingle().NonLazy();
            Container.Bind<AbilitiesSystem>().FromNew().AsSingle();
            
            Container.BindInterfacesAndSelfTo<DashAbility>().FromInstance(_playerConfig.Abilities
                .OfType<DashAbility>().First()).AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerMovement>().FromInstance(_playerConfig.Abilities
                .OfType<PlayerMovement>().First()).AsSingle();
            
            Container.Bind<List<IAbility>>()
                .FromInstance(_playerConfig.Abilities)
                .AsSingle();
        }

        private void BuildUI()
        {
            Container.Bind<UIStateSystem>().FromInstance(_uiStateSystem).AsSingle();
            Container.Bind<InputSystemUIInputModule>().FromInstance(_uiInput).AsSingle();
        }

        private void BuildScene()
        {
            Container.Bind<SceneLoader>().FromComponentInHierarchy().AsSingle();
            Container.Bind<SwarmController>().FromInstance(_swarmController).AsSingle();
        }
    }
}
