using System;
using DG.Tweening;
using Entities.PlayerScripts;
using Entities.UI.SDF;
using Systems;
using Systems.Effects;
using TriInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Zenject;

namespace Entities.UI
{
    public class UIStateSystem : MonoBehaviour
    {
        [SerializeReference, Range(0.5f, 5f)] private float _crossFadeTime;
        
        [SerializeField] private AYellowpaper.SerializedCollections.SerializedDictionary<UIState, CanvasGroup> _canvasGroups = new();
        [SerializeField] private AYellowpaper.SerializedCollections.SerializedDictionary<UIState, UIState> _escapeTransitions = new();
        
        [Title("Pause"), SerializeField] private Animator _pauseAnimator;
        [SerializeField] private Volume _pauseVolume;
        
        private PlayerInput _playerInput;
        private PlayerAbilityController _abilityController;
        private ScreenshotSystem _screenshotSystem;
        public UIState CurrentState { get; private set; }

        [Inject]
        private void Construct(PlayerInput input, ScreenshotSystem screenshotSystem, [InjectOptional] PlayerAbilityController abilityController)
        {
            _playerInput = input;
            _abilityController = abilityController;
            _screenshotSystem = screenshotSystem;
        }

        private void ChangeObjectState(bool state)
        {
            gameObject.SetActive(state);
        }

        private void OnEnable()
        {
            if(_playerInput)
                _playerInput.actions["Escape"].performed += EscapeTransition;
            _screenshotSystem.ScreenShotState += ChangeObjectState;
        }
        private void OnDestroy()
        {
            if(_playerInput)
                _playerInput.actions["Escape"].performed -= EscapeTransition;
            _screenshotSystem.ScreenShotState -= ChangeObjectState;
        }

        private void EscapeTransition(InputAction.CallbackContext ctx)
        {
            if(_escapeTransitions.ContainsKey(CurrentState))
                SwitchCurrentStateAsync(_escapeTransitions[CurrentState]);
        }
        
        public void SwitchCurrentState(int id) => SwitchCurrentStateAsync((UIState)id);
        
        public async void SwitchCurrentStateAsync(UIState state)
        {
            if (SceneManager.GetActiveScene().name != "Start" && _abilityController != null)
            {
                switch (state)
                {
                    case UIState.Normal:
                        DOTween.To(() => _pauseVolume.weight, x => _pauseVolume.weight = x, 0, _crossFadeTime);
                        _abilityController.EnableLastAbilities();
                        if(_pauseAnimator)
                            _pauseAnimator.SetBool("InPause", false);
                        break;
                    case UIState.Pause:
                        DOTween.To(() => _pauseVolume.weight, x => _pauseVolume.weight = x, 1, _crossFadeTime);
                        _abilityController.DisableAllAbilities();
                        if(_pauseAnimator)
                            _pauseAnimator.SetBool("InPause", true);
                        break;
                }
            }

            var currentCanvas = _canvasGroups[CurrentState];
            var targetCanvas = _canvasGroups[state];
            if (currentCanvas)
            {
                currentCanvas.interactable = false;
                currentCanvas.blocksRaycasts = false;
            }

            if (targetCanvas)
            {
                targetCanvas.interactable = true;
                targetCanvas.blocksRaycasts = true;
            }
            CurrentState = state;
            
            await foreach (var (a, b) in CrossfadeEffect.CrossfadeTwins(_crossFadeTime))
            {
                if (currentCanvas) currentCanvas.alpha = a;
                if (targetCanvas) targetCanvas.alpha = b;
            }
        }
    }
    
    [Serializable]
    public enum UIState
    {
        Normal, Settings, Pause, Saves, Dialogue, Building, Shop, FearCollection
    }
}