using Data;
using Entities.Probs;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Entities.Triggers
{
    [RequireComponent(typeof(Collider2D))]
    public class UnityEventTrigger : SavableStateObject
    {
        [SerializeField] private bool _destroyAfter;
        public UnityEvent _event;
        [Inject] private GlobalData _data;
        
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