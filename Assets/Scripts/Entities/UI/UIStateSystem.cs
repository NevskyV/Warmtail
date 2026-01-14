using System;
using AYellowpaper.SerializedCollections;
using Entities.PlayerScripts;
using Systems.Effects;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Zenject;

namespace Entities.UI
{
    public class UIStateSystem : MonoBehaviour
    {
        [SerializeReference, Range(0.5f, 5f)] private float _crossFadeTime;
        [SerializeField] private SerializedDictionary<UIState, CanvasGroup> _canvasGroups = new();
        [SerializeField] private SerializedDictionary<UIState, UIState> _escapeTransitions = new();

        [SerializeField] private Player _player;
        [SerializeField] private PlayerInput _playerInput;
        private PlayerAbilityController _abilityController;
        public UIState CurrentState { get; private set; }

        [Inject]
        private void Construct(PlayerInput input, [InjectOptional] PlayerAbilityController abilityController)
        {
            _playerInput = input;
            _abilityController = abilityController;
        }

        private void OnEnable()
        {
            if(_playerInput)
                _playerInput.actions["Escape"].performed += EscapeTransition;
        }
        private void OnDisable()
        {
            if(_playerInput)
                _playerInput.actions["Escape"].performed -= EscapeTransition;
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
                        _abilityController.EnableLastAbilities();
                        break;
                    case UIState.Pause:
                        _abilityController.DisableAllAbilities();
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
        Normal, Settings, Pause, Saves, Dialogue, Building, Shop
    }
}