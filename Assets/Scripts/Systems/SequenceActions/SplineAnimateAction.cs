using Entities.Core;
using Interfaces;
using UnityEngine;
using UnityEngine.Splines;

namespace Systems.SequenceActions
{
    public class SplineAnimateAction: ISequenceAction
    {
        [SerializeField] private string _objectId;
        public void Invoke()
        {
            SavableObjectsResolver.FindObjectById<SplineAnimate>(_objectId).Restart(false);
            SavableObjectsResolver.FindObjectById<SplineAnimate>(_objectId).Play();
        }
    }
}