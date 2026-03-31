using System;
using Cysharp.Threading.Tasks;
using Data;
using Data.Nodes;
using DG.Tweening;
using EasyTextEffects;
using Entities.Localization;
using Interfaces;
using Systems;
using TMPro;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Entities.UI
{
    public class MonologueVisuals : MonoBehaviour, ITextVisual
    {
        [SerializeField] private float _perCharFadeTime;
        [SerializeField] private float _delayTime;
        [SerializeField] private TMP_Text _textPrefab;
        [SerializeField] private RectTransform _textBounds;
        private RectTransform _currentText;
        private LocalizationManager _localizationManager;
        private DialogueSystem _dialogueSystem;
        private UIStateSystem _uiStateSystem;
        private bool _isEnded;
        private float _currentLineDuration;
        public bool _needToShow;

        [Inject]
        private void Construct(LocalizationManager localizationManager, DialogueSystem dialogueSystem, GlobalData data,
            UIStateSystem uiStateSystem)
        {
            _localizationManager = localizationManager;
            _dialogueSystem = dialogueSystem;
            _uiStateSystem = uiStateSystem;
        }

        public void StartMonologue(RuntimeDialogueGraph graph, IEventInvoker invoker, bool needToShow)
        {
            _dialogueSystem.StartDialogue(graph, this, null, invoker);
            _needToShow = needToShow;
            ProcessDialogue();
        }
        async UniTaskVoid ProcessDialogue()
        {
            while(true){
                await UniTask.Delay(TimeSpan.FromSeconds(_currentLineDuration));
                if(_isEnded) break;
                _dialogueSystem.ActivateNewNode();
            }
        }
            
        
        public void ShowVisuals()
        {
            _isEnded = false;
            _currentText = Instantiate(_textPrefab, _textBounds).GetComponent<RectTransform>();
            _currentText.localPosition = ChooseRandomPosition();
            _uiStateSystem.SwitchCurrentStateAsync(UIState.Hidden).Forget();
        }

        public void HideVisuals()
        {
            _isEnded = true;
            Destroy(_currentText.gameObject);
            if (_needToShow)_uiStateSystem.SwitchCurrentStateAsync(UIState.Normal).Forget();
        }

        public void RequestNewLine(TextNode node)
        {
            _currentText.localPosition = ChooseRandomPosition();
            var line = LocalizationManager.GetStringFromKey("Star_"+ _dialogueSystem.DialogueGraph.DialogueId+ "_" + node.NodeId);
            
            _currentLineDuration = line.Length * _perCharFadeTime + _delayTime;
            print(line);
            _currentText.GetComponent<TMP_Text>().text = line;
            var effect = _currentText.GetComponent<TextEffect>();
            effect.Refresh();
            effect.StartManualEffects();
        }
        
        public async void RequestSingleLine(string id, string prefix = "fragment_")
        {
            var textRect = Instantiate(_textPrefab, _textBounds).GetComponent<RectTransform>();
            textRect.localPosition = ChooseRandomPosition();

            var line = LocalizationManager.GetStringFromKey(prefix + id);
            textRect.GetComponent<TMP_Text>().text = line;

            var duration = line.Length * _perCharFadeTime + _delayTime;

            var effect = textRect.GetComponent<TextEffect>();
            effect.Refresh();
            effect.StartManualEffects();

            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: this.GetCancellationTokenOnDestroy());
            }
            catch
            {
                return;
            }

            if (textRect && textRect.gameObject)
                Destroy(textRect.gameObject);
        }


        private Vector2 ChooseRandomPosition()
        {
            return new Vector2(Random.Range(_textBounds.rect.xMin, _textBounds.rect.xMax),
                Random.Range(_textBounds.rect.yMin, _textBounds.rect.yMax));
        }
        
    }
}
