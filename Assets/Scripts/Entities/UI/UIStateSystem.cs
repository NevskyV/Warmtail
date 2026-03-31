using System;
using System.IO;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Entities.PlayerScripts;
using Systems;
using Systems.Effects;
using TriInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Entities.UI
{
    public class UIStateSystem : MonoBehaviour
    {
        private static readonly int InPause = Animator.StringToHash("InPause");
        [SerializeReference, Range(0.5f, 5f)] private float _crossFadeTime;
        
        [SerializeField] private AYellowpaper.SerializedCollections.SerializedDictionary<UIState, CanvasGroup> _canvasGroups = new();
        [SerializeField] private AYellowpaper.SerializedCollections.SerializedDictionary<UIState, Selectable> _selectables = new();
        [SerializeField] private AYellowpaper.SerializedCollections.SerializedDictionary<UIState, UIState> _escapeTransitions = new();
        [SerializeField] private CanvasGroup _warmthGroup;
        
        [Title("Pause"), SerializeField] private Animator _pauseAnimator;
        [SerializeField] private Volume _pauseVolume;
        [SerializeField] private Image _screenshot;
        
        private PlayerInput _playerInput;
        private PlayerAbilityController _abilityController;
        private ScreenshotSystem _screenshotSystem;
        public UIState CurrentState { get; private set; }
        public Action<UIState> OnStateChange { get; set; }

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
            if (state)
            {
                SwitchCurrentStateAsync(UIState.Photo).Forget();
                _screenshot.sprite = ImageLoadSystem.LoadNewSprite(_screenshotSystem.LastScreenShotPath);
            }
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
                SwitchCurrentStateAsync(_escapeTransitions[CurrentState]).Forget();
        }
        
        public void SwitchCurrentState(int id) => SwitchCurrentStateAsync((UIState)id).Forget();
        
        public async UniTask SwitchCurrentStateAsync(UIState state)
        {
            if (SceneManager.GetActiveScene().name != "Start" && _abilityController)
            {
                switch (state)
                {
                    case UIState.Normal:
                        _warmthGroup.DOFade(1, _crossFadeTime);
                        DOTween.To(() => _pauseVolume.weight, x => _pauseVolume.weight = x, 0, _crossFadeTime);
                        _abilityController.EnableLastAbilities();
                        if(_pauseAnimator)
                            _pauseAnimator.SetBool(InPause, false);
                        break;
                    case UIState.Hidden or UIState.Pause:
                        DOTween.To(() => _pauseVolume.weight, x => _pauseVolume.weight = x, 1, _crossFadeTime);
                        if (state == UIState.Pause){
                            _warmthGroup.DOFade(1, _crossFadeTime);
                            _abilityController.DisableAllAbilities();
                            if (_pauseAnimator)
                                _pauseAnimator.SetBool(InPause, true);
                        }
                        break;
                    default:
                        _abilityController.DisableAllAbilities();
                        _warmthGroup.DOFade(0, _crossFadeTime);
                        break;
                }
            }
            if(_selectables.TryGetValue(state, out var selectable)) EventSystem.current.SetSelectedGameObject(selectable.gameObject);
            
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
            OnStateChange?.Invoke(CurrentState);
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
        Normal, Settings, Pause, Dialogue, Building, Shop, Map, Bestiary, FearMenu, MusicSelection, Photo, Hidden
    }
}