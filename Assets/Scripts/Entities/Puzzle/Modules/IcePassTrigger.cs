using UnityEngine;

namespace Entities.Puzzle.Modules
{
    public class IcePassTrigger : MonoBehaviour
    {
        private RenewableIceModule _module;

        public void Initialize(RenewableIceModule module)
        {
            _module = module;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Pushable"))
                _module?.Solve();
        }
    }
}
