using Entities.UI;
using UnityEngine;
using Zenject;

namespace Entities.Props
{
    public class Ice : SavableStateObject
    {
        [Inject] private FreezeVisuals _freeze;
        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _freeze.StopAllCoroutines();
                _freeze.StartCoroutine(_freeze.StartDrain());
            }
        }
        
        public void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _freeze.StopAllCoroutines();
                _freeze.StartCoroutine(_freeze.StopDrain());
            }
        }
    }
}