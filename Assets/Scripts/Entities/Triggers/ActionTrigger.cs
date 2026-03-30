using Entities.Props;
using Interfaces;
using UnityEngine;

namespace Entities.Triggers
{
    public class ActionTrigger : SavableStateObject
    {
        [SerializeField] private bool _destroyAfter;
        [SerializeReference] private ISequenceAction _event;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _event.Invoke();
                if (_destroyAfter)
                {
                    ChangeState(false);
                }
            }
        }
    }
}