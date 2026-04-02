using Systems;
using Zenject;

namespace Data.Nodes
{
    public record ActionNode : RuntimeNode
    {
        public int EventInd;

        [Inject] private DialogueSystem _dialogueSystem;

        public override void Activate()
        {
            _dialogueSystem.Character.InvokeEvent(EventInd);
        
            _dialogueSystem.SetNewNode();
            _dialogueSystem.ActivateNewNode();
        }
    }
}

