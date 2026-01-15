using System;
using Cysharp.Threading.Tasks;
using Data;
using Data.Player;
using EasyTextEffects.Editor.MyBoxCopy.Extensions;
using Entities.Localization;
using Systems;
using TriInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Entities.Core
{
    public class SceneLoader : MonoBehaviour
    {
        private static readonly int EndTransition = Animator.StringToHash("EndTransition");
        [SerializeField] private Animator _animPrefab;
        [SerializeField, Unit("Milliseconds")] private int _animDuration;
        public Action SceneStartLoading;
        public Action<string> SceneLoaded;
        private AsyncOperation _asyncLoad;
        [Inject] private DiContainer _container;
        [Inject] private GlobalData _globalData;
        [Inject] private ScreenshotSystem _screenshotSystem;
        
        private void Start()
        {
            DontDestroyOnLoad(this);
        }
        
        public async void StartSceneProcess(string sceneInd)
        {
            var animator = Instantiate(_animPrefab);
            DontDestroyOnLoad(animator);
            var texts = animator.GetComponentsInChildren<LocalizedText>();
            texts.ForEach(x =>
            {
                _container.Inject(x);
                x.UpdateString();
            });
            _screenshotSystem.DisableAutoScreenshot();
            SceneStartLoading?.Invoke();
            
            await UniTask.Delay(_animDuration);
            _asyncLoad = SceneManager.LoadSceneAsync(sceneInd);
            await UniTask.WhenAll(_asyncLoad.ToUniTask(), UniTask.WaitUntil(() => _asyncLoad.allowSceneActivation));
            
            animator.SetTrigger(EndTransition);
            await UniTask.Delay(_animDuration);
            Destroy(animator);
            
            SceneLoaded?.Invoke(sceneInd);
            if (sceneInd != "Start")
            {
                _globalData.Edit<SavablePlayerData>(data => data.LastScene = sceneInd);
                _screenshotSystem.EnableAutoScreenshot();
            }
        }
    }
}