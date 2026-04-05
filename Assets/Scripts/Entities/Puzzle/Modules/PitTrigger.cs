using UnityEngine;

namespace Entities.Puzzle.Modules
{
    public class PitTrigger : MonoBehaviour
    {
        private PitModule _module;

        public void Initialize(PitModule module)
        {
            _module = module;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_module == null) return;
            if (!other.CompareTag("Pushable")) return;
            _module.Solve();
        }
    }
}
