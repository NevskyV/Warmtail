using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Entities.UI
{
    public class TipsVisuals : MonoBehaviour
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
        
        public async void ShowTip(InputActionReference reference)
        {
            var inputData = _tips[_playerInput.currentControlScheme].Find(x => x.Action == reference);
            inputData.UI.SetActive(true);
            await UniTask.WaitUntil(inputData.Action.action.IsPressed);
            inputData.UI.SetActive(false);
        }
    }
}