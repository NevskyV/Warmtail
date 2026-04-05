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
            if (!other.gameObject.CompareTag("Pushable")) return;
            GetComponent<Collider2D>().enabled = false;
            other.gameObject.GetComponent<Collider2D>().enabled = false;
            other.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            _module.Solve();
        }
    }
}
