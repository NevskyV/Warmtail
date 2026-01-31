using Interfaces;
using UnityEngine;
using Entities.Core;
using Entities.NPC;

namespace Systems.SequenceActions
{
    public class SetPositionAction : ISequenceAction
    {
        [SerializeField] private string _id;
        [SerializeField] private Vector2 _newPosition;

        public void Invoke()
        {
            SavableObjectsResolver.FindObjectById<SpeakableCharacter>(_id).transform.position = _newPosition;
        }
    }
}