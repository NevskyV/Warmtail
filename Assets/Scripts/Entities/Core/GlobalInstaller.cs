using Data;
using Entities.Localization;
using Entities.Sound;
using Systems;
using Systems.Abilities;
using Systems.DataSystems;
using Systems.Effects;
using UnityEngine;
using Zenject;

namespace Entities.Core
{
    public class GlobalInstaller : MonoInstaller
    {
        [SerializeField] private GlobalData _globalData;
        [SerializeField] private LocalizationManager _localizationManager;
        [SerializeField] private MusicStateSystem _musicSystem;
        [SerializeField] private SceneLoader _sceneLoader;

        public override void InstallBindings()
        {
            Debug.Log("set ui installer GLOBAL");
            Container.Bind<SaveSystem>().FromNew().AsSingle();
            Container.Bind<CrossfadeEffect>().FromNew().AsSingle();
            Container.Bind<ShoppingSystem>().FromNew().AsSingle();
            Container.Bind<ManualSystem>().FromNew().AsSingle();
            Container.BindInterfacesAndSelfTo<SessionSystem>().AsSingle().NonLazy();
            Container.Bind<SceneLoader>().FromInstance(_sceneLoader).AsSingle();
            Container.Bind<LocalizationManager>().FromInstance(_localizationManager).AsSingle();
            Container.Bind<GlobalData>().FromInstance(_globalData).AsSingle();
            var musicSystemObj = Instantiate(_musicSystem);
            DontDestroyOnLoad(musicSystemObj);
            Container.Inject(musicSystemObj);
            Container.Bind<MusicStateSystem>().FromInstance(musicSystemObj).AsSingle();
        }
    }
}