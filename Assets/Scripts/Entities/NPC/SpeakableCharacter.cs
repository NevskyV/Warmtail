using System.Collections.Generic;
using System.Globalization;
using Entities.Probs;
using Entities.UI;
using Interfaces;
using Systems;
using Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Zenject;

namespace Entities.NPC
{
    public class SpeakableCharacter : SavableStateObject, IInteractable, IEventInvoker
    {
        [field: SerializeField] public RuntimeDialogueGraph Graph { get; set; }
        [field: SerializeField] public List<UnityEvent> Actions { get; set; }
        [field: SerializeField] public List<UnityEvent> SavableState { get; private set; }
        private DialogueSystem _dialogueSystem;
        private DialogueVisuals _visuals;
        private UIStateSystem _uiStateSystem;

        [Inject]
        private void Construct(DialogueSystem dialogueSystem, DialogueVisuals visuals, UIStateSystem uiStateSystem)
        {
            _dialogueSystem = dialogueSystem;
            _visuals = visuals;
            _uiStateSystem = uiStateSystem;
            if (SceneManager.GetActiveScene().name == "Home" || SceneManager.GetActiveScene().name == "HomeIra")
                Interact();
        }
        
        public void Interact()
        {
            if (!Graph || (_uiStateSystem && _uiStateSystem.CurrentState == UIState.Shop)) return;
            _dialogueSystem.StartDialogue(Graph, _visuals, Id, this);
            if (SceneManager.GetActiveScene().name == "Gameplay") Graph = null;
        }
        
        public void SetPosition(string pos)
        {
            var (x, y) = (float.Parse(pos.Split(' ')[0], CultureInfo.InvariantCulture),
                float.Parse(pos.Split(' ')[1], CultureInfo.InvariantCulture));
            transform.position = new Vector2(x,y);
        }

        public void AddNpcToHome(int character)
        {
            _globalData.Edit<NpcSpawnData>(data => data.CurrentHomeNpc = (Character)character);
        }
    }
    
}
