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
            if (_objectIds == null || _objectIds.Count == 0) return;
            
            foreach (var id in _objectIds)
            {
                var obj = SavableObjectsResolver.FindObjectById(id);
                if (obj != null)
                {
                    obj.SetActive(_active);
                }
                else
                {
                    Debug.LogWarning($"ActivateObjectsAction: Object with ID '{id}' not found!");
                }
            }
        }
    }
}