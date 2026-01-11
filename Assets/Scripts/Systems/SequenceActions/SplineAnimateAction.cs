using Entities.Core;
using Interfaces;
using UnityEngine;
using UnityEngine.Splines;

namespace Systems.SequenceActions
{
    public class SplineAnimateAction: ISequenceAction
    {
        [SerializeField] private string _characterId;
        [SerializeField] private string _splineId;
        [SerializeField] private bool _move = true;
        public void Invoke()
        {
            var character = SavableObjectsResolver.FindObjectById<SplineAnimate>(_characterId);
            character.Container = SavableObjectsResolver.FindObjectById<SplineContainer>(_splineId);
            character.Restart(false);
            if (_move) character.Play();
        }
    }
}