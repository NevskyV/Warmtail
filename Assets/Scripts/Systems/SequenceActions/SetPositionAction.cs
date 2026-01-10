using System;
using Interfaces;
using UnityEngine;
using Entities.Probs;
using Entities.Core;

namespace Systems.SequenceActions
{
    public class SetPositionAction : ISequenceAction
    {
        [SerializeField] private string _id;
        [SerializeField] private Vector2 _newPosition;

        public void Invoke()
        {
            SavableObjectsResolver.FindObjectById<SavableStateObject>(_id).transform.position = _newPosition;
        }
    }
}