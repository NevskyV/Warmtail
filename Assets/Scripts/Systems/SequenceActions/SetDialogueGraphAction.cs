using Data;
using Entities.Core;
using Entities.NPC;
using Interfaces;
using UnityEngine;

namespace Systems.SequenceActions
{
    public class SetDialogueGraphAction : ISequenceAction
    {
        [SerializeField] private RuntimeDialogueGraph _graph;
        [SerializeField] private string _npcId;
        
        public void Invoke()
        {
            SavableObjectsResolver.FindObjectById<SpeakableCharacter>(_npcId).Graph = _graph;
        }
    }
}