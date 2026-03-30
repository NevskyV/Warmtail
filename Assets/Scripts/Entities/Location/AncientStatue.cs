using Cysharp.Threading.Tasks;
using Data;
using Entities.NPC;
using Entities.Props;
using Entities.Triggers;
using Interfaces;
using UnityEngine;
using Zenject;

namespace Entities.Location
{
    public class AncientStatue : SavableStateObject, IInteractable
    {
        [SerializeField] private int _statueId;
        [SerializeField] private string _dialogueVarName;
        [SerializeField] private Sprite _activeSprite;
        [SerializeField] private LightTrigger _lightTrigger;
        [SerializeField] private SpeakableCharacter _teo;
        [SerializeField] private RuntimeDialogueGraph _graph;
        private const int _effectTime = 180;
        [Inject] private GlobalData _globalData;
        public bool CanInteract { get; private set; } = true;

        private void Start()
        {
            if (_globalData.Get<DialogueVarData>().Variables.Find(x => x.Name == "interactWTeo").Value == "true")
            {
                CanInteract = true;
            }
            if (CanInteract && _globalData.Get<WorldData>().ActivatedStatues.Contains(_statueId))
            {
                EnableEffect();
                CanInteract = false;
            }
        }
        
        public void Interact()
        {
            if (!CanInteract) return;
            int value = 0;
            _globalData.Edit<DialogueVarData>(data =>
            {
                var variable = data.Variables.Find(x => x.Name == _dialogueVarName);
                value = int.Parse(variable.Value);
                data.Variables.Find(x => x.Name == _dialogueVarName).Value = (value+1).ToString();
            });
            _globalData.Edit<WorldData>(data => data.ActivatedStatues.Add(_statueId));
            CanInteract = false;
            EnableEffect();
            if (value+1 == 7)
            {
                _teo.Graph = _graph;
            }
        }

        private void EnableEffect()
        {
            _lightTrigger.gameObject.SetActive(true);
            GetComponent<SpriteRenderer>().sprite = _activeSprite;
        }
    }
}