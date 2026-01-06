using System.Collections.Generic;
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
using Systems.Environment;
using Unity.Cinemachine;
using Systems.Environment;

namespace Entities.Core
{
    public class HouseInstaller : MonoInstaller
    {
        [SerializeField] private Player _player;
        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private HouseManager _houseManager;
        [SerializeField] private PlayerConfig _playerConfig;
        [SerializeField] private UIStateSystem _uiStateSystem;
        [SerializeField] private ShoppingManager _shoppingManager;
        [SerializeField] private DialogueVisuals _dialogueVisuals;
        [SerializeField] private QuestVisuals _questVisuals;
        [SerializeField] private CinemachineCamera _cam;
        [SerializeField] private SurfacingSystem _surfacingSystem;

        public override void InstallBindings()
        {
            Container.Bind<SurfacingSystem>().FromInstance(_surfacingSystem).AsSingle();
            Container.Bind<Player>().FromInstance(_player).AsSingle();
            Container.Bind<PlayerInput>().FromInstance(_playerInput).AsSingle();
            Container.Bind<HouseManager>().FromInstance(_houseManager).AsSingle();
            Container.Bind<PlayerConfig>().FromInstance(_playerConfig).AsSingle();
            Container.Bind<UIStateSystem>().FromInstance(_uiStateSystem).AsSingle();
            Container.Bind<ShoppingManager>().FromInstance(_shoppingManager).AsSingle();
            Container.Bind<CinemachineCamera>().FromInstance(_cam).AsSingle();
            Container.Bind<DialogueVisuals>().FromInstance(_dialogueVisuals).AsSingle();
            Container.Bind<QuestVisuals>().FromInstance(_questVisuals).AsSingle();
            Container.Bind<SurfacingSystem>().FromInstance(null).AsSingle();
            Container.Bind<NPCMethods>().FromNew().AsSingle();
            Container.Bind<ShoppingSystem>().FromNew().AsSingle();
            Container.Bind<PlacementSystem>().FromNew().AsSingle();
            Container.Bind<DialogueSystem>().FromNew().AsSingle();
            Container.Bind<QuestSystem>().FromNew().AsSingle();
            Container.Bind<DailySystem>().FromNew().AsSingle();
            
            Container.Bind<List<IAbility>>()
                .FromInstance(_playerConfig.Abilities)
                .AsSingle();
        }
    }
}
