using System;
using Cysharp.Threading.Tasks;
using Data;
using Data.Nodes;
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
        [SerializeField] private float _textFadeSpeed;
        [SerializeField] private TMP_Text _textPrefab;
        [SerializeField] private RectTransform _textBounds;
        private RectTransform _currentText;
        private LocalizationManager _localizationManager;
        private DialogueSystem _dialogueSystem;
        private bool _isEnded;

        [Inject]
        private void Construct(LocalizationManager localizationManager, DialogueSystem dialogueSystem, GlobalData data)
        {
            _localizationManager = localizationManager;
            _dialogueSystem = dialogueSystem;
        }

        public void StartMonologue(RuntimeDialogueGraph graph, IEventInvoker invoker)
        {
            _dialogueSystem.StartDialogue(graph, this, null, invoker);
            ProcessDialogue();
        }
        
        public async void ProcessDialogue()
        {
            while(true){
                await UniTask.Delay(TimeSpan.FromSeconds(_textFadeSpeed));
                if(_isEnded) break;
                _dialogueSystem.ActivateNewNode();
            }
        }
            
        
        public void ShowVisuals()
        {
            _isEnded = false;
            _currentText = Instantiate(_textPrefab, _textBounds).GetComponent<RectTransform>();
            _currentText.localPosition = ChooseRandomPosition();
        }

        public void HideVisuals()
        {
            _isEnded = true;
            Destroy(_currentText.gameObject);
        }

        public void RequestNewLine(TextNode node)
        {
            _currentText.localPosition = ChooseRandomPosition();
            _currentText.GetComponent<TMP_Text>().text = 
                LocalizationManager.GetStringFromKey("Star_"+ _dialogueSystem.DialogueGraph.DialogueId+ "_" + node.NodeId);
            _currentText.GetComponent<TextEffect>().Refresh();
        }
        
        public async void RequestSingleLine(int id)
        {
            _currentText = Instantiate(_textPrefab, _textBounds).GetComponent<RectTransform>();
            _currentText.localPosition = ChooseRandomPosition();
            _currentText.GetComponent<TMP_Text>().text = 
                LocalizationManager.GetStringFromKey("fragment_" + id);
            _currentText.GetComponent<TextEffect>().Refresh();
            await UniTask.Delay(TimeSpan.FromSeconds(_textFadeSpeed));
            Destroy(_currentText.gameObject);
        }


        private Vector2 ChooseRandomPosition()
        {
            return new Vector2(Random.Range(_textBounds.rect.xMin, _textBounds.rect.xMax),
                Random.Range(_textBounds.rect.yMin, _textBounds.rect.yMax));
        }
        
    }
}