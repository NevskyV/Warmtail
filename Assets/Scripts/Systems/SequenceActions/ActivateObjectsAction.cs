using System.Collections.Generic;
using Entities.Core;
using Interfaces;
using UnityEngine;

namespace Systems.SequenceActions
{
    public class ActivateObjectsAction : ISequenceAction
    {
        [SerializeField] private bool _active;
        [SerializeField] private List<string> _objectIds;
            
        public void Invoke()
        {
            _objectIds.ForEach(x =>
            {
                var obj = SavableObjectsResolver.FindObjectById(x);
                if (obj != null)
                {
                    obj.SetActive(_active);
                }
                else
                {
                    Debug.LogWarning($"ActivateObjectsAction: Object with id '{x}' not found");
                }
            });
        }
    }
}