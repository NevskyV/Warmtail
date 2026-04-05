using Interfaces;
using Systems;
using UnityEngine;

namespace Entities.Puzzle.Modules
{
    public class TimedDashLever : MonoBehaviour, IDestroyable
    {
        [SerializeField] private float _availableDuration = 5f;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Sprite _activeSprite;
        [SerializeField] private Sprite _inactiveSprite;

        private TimedLeverModule _module;
        private ResettableTimer _deactivateTimer;
        private bool _available = true;
        private bool _solved;

        public void Initialize(TimedLeverModule module)
        {
            _module = module;
            _deactivateTimer = new ResettableTimer(_availableDuration, Deactivate);
            _deactivateTimer.Start();
            SetSprite(true);
        }

        public void DestroyObject()
        {
            if (!_available || _solved) return;
            _solved = true;
            _deactivateTimer?.Stop();
            SetSprite(false);
            _module?.Solve();
        }

        private void Deactivate()
        {
            _available = false;
            SetSprite(false);
        }

        private void SetSprite(bool active)
        {
            if (_spriteRenderer == null) return;
            _spriteRenderer.sprite = active ? _activeSprite : _inactiveSprite;
        }
    }
}
