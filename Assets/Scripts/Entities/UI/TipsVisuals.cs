using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Entities.Props;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Entities.UI
{
    public class TipsVisuals : SavableStateObject
    {
        [Serializable]
        public struct InputData
        {
            public InputActionReference Action;
            public GameObject UI;
        }

        [SerializeField] private float _fadeTime = 2f;
        [SerializeField] private SerializedDictionary<string, List<InputData>> _tips;
        
        [Inject] private PlayerInput _playerInput;
        private HashSet<InputAction> _lastInputActions = new ();

        private void Start()
        {
            _playerInput.onControlsChanged += OnControlsChanged;
        }

        private void OnControlsChanged(PlayerInput input)
        {
            foreach (var lastInputAction in _lastInputActions)
            {
                foreach (var (scheme, data) in _tips)
                {
                    HideTip(lastInputAction, scheme);
                }
                ShowTip(lastInputAction);
            }
        }

        public async void ShowTip(InputAction reference)
        {
            print(_playerInput.currentControlScheme);
            var inputData = _tips[_playerInput.currentControlScheme].Find(x => x.Action.action == reference);
            inputData.UI.SetActive(true);
            _lastInputActions.Add(inputData.Action);
            await UniTask.WaitUntil(inputData.Action.action.IsPressed);
            HideTip(reference);
        }
        
        public void HideTip(InputAction reference)
        {
            var inputData = _tips[_playerInput.currentControlScheme].Find(x => x.Action.action == reference);
            inputData.UI.SetActive(false);
            _lastInputActions.Remove(inputData.Action);
        }
        
        public void HideTip(InputAction reference, string controlScheme)
        {
            var inputData = _tips[controlScheme].Find(x => x.Action.action == reference);
            inputData.UI.SetActive(false);
        }
    }
}