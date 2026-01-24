using Entities.Props;
using UnityEngine;

namespace Entities.Triggers
{
    [RequireComponent(typeof(Collider2D))]
    public class MoveToTrigger : SavableStateObject
    {
        [SerializeField] private bool _destroyAfter;
        [SerializeField] private Transform _target;
        [SerializeField] private Transform _position;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _target.position = _position.position;
                if (_destroyAfter)
                {
                    ChangeState(false);
                }
            }
        }
    }
}