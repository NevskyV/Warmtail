using UnityEngine;

namespace Entities.Puzzle.Modules
{
    public class ColdWallExitTrigger : MonoBehaviour
    {
        private ColdWallStickModule _module;

        public void Initialize(ColdWallStickModule module)
        {
            _module = module;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
                _module?.Solve();
        }
    }
}
