using Entities.UI;
using UnityEngine;
using Zenject;

namespace Entities.Props
{
    public class Ice : SavableStateObject
    {
        [Inject] private FreezeVisuals _freeze;
        public void TriggerEnter2D()
        {
            _freeze.StopAllCoroutines();
            _freeze.StartDrain(Id);
        }
        
        public void TriggerExit2D()
        {
            _freeze.StopAllCoroutines();
            _freeze.StopDrain(Id);
        }
    }
}