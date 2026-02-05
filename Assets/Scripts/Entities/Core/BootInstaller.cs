using Data;
using Data.Player;
using Entities.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Zenject;

namespace Entities.Core
{
    public class BootInstaller : MonoInstaller
    {
        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private InputSystemUIInputModule _uiInput;
        [SerializeField] private UIStateSystem _uiStateSystem;
        [SerializeField] private PlayableDirector _director;
        [SerializeField] private TimelineAsset _longTimeline;
        [SerializeField] private TimelineAsset _shortTimeline;
        
        public override void InstallBindings()
        {
            Container.Bind<PlayerInput>().FromInstance(_playerInput).AsSingle();
            Container.Bind<UIStateSystem>().FromInstance(_uiStateSystem).AsSingle();
            Container.Bind<InputSystemUIInputModule>().FromInstance(_uiInput).AsSingle();
        }

        [Inject]
        private void Construct(GlobalData data)
        {
            _director.Play(data.Get<RuntimePlayerData>().WasInGame ? _shortTimeline : _longTimeline);
            data.Edit<RuntimePlayerData>(playerData => playerData.WasInGame = true);
        }
    }
}