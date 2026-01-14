using Data;
using Data.Nodes;
using EasyTextEffects;
using EasyTextEffects.Effects;
using Entities.Localization;
using Interfaces;
using Systems;
using TMPro;
using TriInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;

namespace Entities.UI
{
    public enum FrameType{Box, Bubble}
    
    [DeclareBoxGroup("BoxFrame")]
    public class DialogueVisuals : MonoBehaviour, ITextVisual
    {
        [Title("Settings")] 
        [SerializeReference] private CharacterHolder _holder;
        [SerializeReference] private AudioSource _audioSource;
        [SerializeReference] private Effect_Composite _effect;
        
        [GroupNext("BoxFrame")]
        [SerializeField, LabelText("Object")] 
        private GameObject _boxObject;
        [SerializeField, LabelText("Text")] 
        private TMP_Text _boxText;
        [SerializeField, LabelText("Name")] 
        private TMP_Text _boxName;
        [SerializeField, LabelText("Character Image")] 
        private Image _boxImage;
        [SerializeField, LabelText("Options Group")]
        private Transform _boxOptionsGroup;
        [SerializeField, LabelText("Option Prefab")]
        private DialogueOptionUI _boxOptionsPrefab;
        
        private DiContainer _diContainer;
        private DialogueSystem _system;
        private GlobalData _globalData;
        private PlayerInput _input;
        private UIStateSystem _uiStateSystem;
        
        private TextEffect _boxNameEffect;
        private TextEffect _boxTextEffect;
        
        private LocalizationManager _localizationManager;
        public bool IsComplete { get; set; }

        [Inject]
        private void Construct(DiContainer container, PlayerInput input, LocalizationManager localization,
            DialogueSystem dialogueSystem, GlobalData globalData, UIStateSystem uiStateSystem)
        {
            _diContainer = container;
            _input = input;
            _system = dialogueSystem;
            _globalData = globalData;
            _localizationManager = localization;
            _uiStateSystem = uiStateSystem;

            _input.actions.FindAction("Space").performed += RequestNewNode;
            _boxNameEffect = _boxName.GetComponent<TextEffect>();
            _boxTextEffect = _boxText.GetComponent<TextEffect>();
            _boxTextEffect.globalEffects[0].onEffectCompleted.AddListener(() => IsComplete = true);
        }
        
        private void RequestNewNode(InputAction.CallbackContext _)
        {
            if (!IsComplete)
            {
                ChangeEffectSpeed();
                return;
            }
            _system.ActivateNewNode();
        }

        public void ShowVisuals()
        {
            _uiStateSystem.SwitchCurrentStateAsync(UIState.Dialogue);
        }
        
        public void HideVisuals()
        {
            if (_uiStateSystem.CurrentState != UIState.Shop)
                _uiStateSystem.SwitchCurrentStateAsync(UIState.Normal);
        }
        
        public void RequestNewLine(TextNode node)
        {
            IsComplete = false;
            var character = _holder.Characters.Find(x => x.Character == node.Character);
            print(character.Character);
            string text = LocalizationManager.GetStringFromKey(node.Character + "_" + _system.DialogueGraph.DialogueId + "_" + node.NodeId);
            
            if (_audioSource != null)
            {
                _audioSource.clip = character.Sound;
                _audioSource.Play();
            }

            var displayName = node.DisplayName == "" ? 
                LocalizationManager.GetStringFromKey(node.Character.ToString()) : node.DisplayName;
            if(displayName == "Player")
            {
                displayName = _globalData.Get<DialogueVarData>().Variables.Find(var => var.Name == "playerName").Value;
            }
            _boxName!.text = displayName;
            _boxNameEffect.Refresh();
            if(character.EmotionSprites.ContainsKey(node.Emotion))
                _boxImage.sprite = character.EmotionSprites[node.Emotion];
            _boxText.text = text;
            _boxTextEffect.globalEffects[0].effect = _effect;
            _boxTextEffect.Refresh();
            _boxTextEffect.StartOnStartEffects();
        }
        
        public async void ShowOptions(RuntimeNode node, int choiceCount)
        {
            _input.SwitchCurrentActionMap("UI");
            for (int i = 0; i < choiceCount; i++)
            {
                var text = LocalizationManager.GetStringFromKey(
                    $"Player_{_system.DialogueGraph.DialogueId}_{node.NodeId}_{i}");
                var boxObj = await InstantiateAsync(_boxOptionsPrefab, _boxOptionsGroup);
                boxObj[0].GetComponentInChildren<TMP_Text>().text = text;
                _diContainer.Inject(boxObj[0]);
            }
            _boxText.gameObject.SetActive(false);
            _boxName.gameObject.SetActive(false);
            _boxImage.gameObject.SetActive(false);
            
            _boxOptionsGroup.gameObject.SetActive(true);
        }

        public void ChooseOption(int i)
        {
            _input.SwitchCurrentActionMap("Dialogue");
            for (int j = _boxOptionsGroup.childCount - 1; j >= 0; j--)
            {
                Destroy(_boxOptionsGroup.GetChild(j).gameObject);
            }
            _boxText.gameObject.SetActive(true);
            _boxName.gameObject.SetActive(true);
            _boxImage.gameObject.SetActive(true);
            _boxOptionsGroup.gameObject.SetActive(false);
            
            _system.SetNewNode(i);
            _system.ActivateNewNode();
        }

        public void ChangeEffectSpeed()
        {
            _boxTextEffect.StopOnStartEffects();
            IsComplete = true;
        }

        public void OnDisable()
        {
            ChangeEffectSpeed();
        }
    }
}