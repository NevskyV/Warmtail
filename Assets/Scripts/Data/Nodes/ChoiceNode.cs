using System.Collections.Generic;
using Entities.UI;
using Zenject;
using UnityEngine;

namespace Data.Nodes
{
    public class ChoiceNode : RuntimeNode
    {
        [field: SerializeField] public List<string> Choices { get; private set; } = new();
    
        [Inject] private DialogueVisuals _dialogueVisuals;

        public override void Activate()
        {
            _dialogueVisuals.ShowOptions(this, Choices.Count);
        }
    }
}
