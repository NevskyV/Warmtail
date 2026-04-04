using UnityEngine;

namespace Entities.Puzzle.Modules
{
    public class ButtonPressTrigger : MonoBehaviour
    {
        private ButtonPressModule _module;

        public void Initialize(ButtonPressModule module)
        {
            _module = module;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_module == null) return;
            if (!other.CompareTag("Pushable") && !other.CompareTag("Player")) return;
            _module.Solve();
        }
    }
}
