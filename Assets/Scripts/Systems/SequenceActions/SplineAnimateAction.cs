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
        public void Invoke()
        {
            var character = SavableObjectsResolver.FindObjectById<SplineAnimate>(_characterId);
            character.Container = SavableObjectsResolver.FindObjectById<SplineContainer>(_splineId);
            character.Restart(false);
            character.Play();
        }
    }
}