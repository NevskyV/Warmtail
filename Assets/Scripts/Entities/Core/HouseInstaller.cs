using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Player;
using Entities.PlayerScripts;
using Entities.UI;
using Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
using Systems;
using Entities.House;
using Entities.NPC;
using Systems.Abilities;
using Systems.Environment;
using Unity.Cinemachine;
using UnityEngine.InputSystem.UI;

namespace Entities.Core
{
    public class HouseInstaller : MonoInstaller
    {
        [SerializeField] private Player _player;
        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private InputSystemUIInputModule _uiInput;
        [SerializeField] private HouseManager _houseManager;
        [SerializeField] private PlayerConfig _playerConfig;
        [SerializeField] private UIStateSystem _uiStateSystem;
        [SerializeField] private DialogueVisuals _dialogueVisuals;
        [SerializeField] private MonologueVisuals _monologueVisuals;
        [SerializeField] private QuestVisuals _questVisuals;
        [SerializeField] private CinemachineCamera _cam;
        [SerializeField] private SurfacingSystem _surfacingSystem;
        [SerializeField] private TipsVisuals _tipsVisuals;
        [SerializeField] private MarksVisuals _marksVisuals;
        private QuestSystem _questSystem = new();

        public override void InstallBindings()
        {
            Debug.Log("set ui installer HOUSE");
            
            Container.Inject(new WarmthSystem());
            Container.Bind<SurfacingSystem>().FromInstance(_surfacingSystem).AsSingle();
            Container.Bind<Player>().FromInstance(_player).AsSingle();
            Container.Bind<PlayerStateController>().FromNewComponentOn(_player.gameObject).AsSingle().NonLazy();
            Container.Bind<PlayerAbilityController>().FromNewComponentOn(_player.gameObject).AsSingle().NonLazy();
            Container.Bind<PlayerInput>().FromInstance(_playerInput).AsSingle();
            Container.Bind<HouseManager>().FromInstance(_houseManager).AsSingle();
            Container.Bind<PlayerConfig>().FromInstance(_playerConfig).AsSingle();
            Container.Bind<UIStateSystem>().FromInstance(_uiStateSystem).AsSingle();
            Container.Bind<CinemachineCamera>().FromInstance(_cam).AsSingle();
            Container.Bind<DialogueVisuals>().FromInstance(_dialogueVisuals).AsSingle();
            Container.Bind<MonologueVisuals>().FromInstance(_monologueVisuals).AsSingle();
            Container.Bind<QuestVisuals>().FromInstance(_questVisuals).AsSingle();
            Container.Bind<InputSystemUIInputModule>().FromInstance(_uiInput).AsSingle();
            Container.Bind<SceneSystem>().FromNew().AsSingle();
            Container.Bind<PlacementSystem>().FromNew().AsSingle();
            Container.Bind<DialogueSystem>().FromNew().AsSingle();
            Container.Bind<GamepadRumble>().FromNew().AsSingle();
            Container.Bind<ScreenshotSystem>().FromNew().AsSingle();
            Container.Bind<DailySystem>().FromNew().AsSingle();
            Container.Bind<EventsData>().FromNew().AsSingle();
            Container.Bind<MarksVisuals>().FromInstance(_marksVisuals).AsSingle();
            Container.Bind<TipsVisuals>().FromInstance(_tipsVisuals).AsSingle();
            Container.Bind<QuestSystem>().FromInstance(_questSystem).AsSingle();
            
            Container.Inject(_questSystem);

            Container.BindInterfacesAndSelfTo<PlayerMovement>().FromInstance(_playerConfig.Abilities
                .OfType<PlayerMovement>().First()).AsSingle();
            
            Container.Bind<List<IAbility>>()
                .FromInstance(_playerConfig.Abilities)
                .AsSingle();
        }
    }
}
