using UnityEngine;

namespace Entities.Puzzle.Modules
{
    public class ButtonPressTrigger : MonoBehaviour
    {
        [SerializeField] private Sprite _sprite;
        private ButtonPressModule _module;

        public void Initialize(ButtonPressModule module)
        {
            _module = module;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_module == null) return;
            if (!other.CompareTag("pushable")) return;
            GetComponent<SpriteRenderer>().sprite = _sprite;
            _module.Solve();
        }
    }
}
