using Systems;
using Zenject;

namespace Data.Nodes
{
    public record TextNode : RuntimeNode
    {
        public string Text; 
        public Character Character;
        public CharacterEmotion Emotion;
        public string DisplayName;
    
        [Inject] private DialogueSystem _dialogueSystem;

        public override void Activate()
        {
            _dialogueSystem.Visuals.RequestNewLine(this);
            _dialogueSystem.SetNewNode();
        }
    }
}