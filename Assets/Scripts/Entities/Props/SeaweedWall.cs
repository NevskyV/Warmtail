using Interfaces;
using Systems;
using UnityEngine;

namespace Entities.Props
{
    public class SeaweedWall : Warmable
    {
        [SerializeField] private float _openDuration = 8f;
        [SerializeField] private Collider2D _wallCollider;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private ParticleSystem _openVfx;

        private ResettableTimer _closeTimer;
        private bool _isOpen;

        private void Awake()
        {
            _closeTimer = new ResettableTimer(_openDuration, Close);
        }

        public override void WarmComplete()
        {
            if (_isOpen) return;
            Open();
        }

        private void Open()
        {
            _isOpen = true;
            if (_wallCollider) _wallCollider.enabled = false;
            if (_spriteRenderer) _spriteRenderer.enabled = false;
            if (_openVfx) _openVfx.Play();
            _closeTimer.Start();
        }

        private void Close()
        {
            _isOpen = false;
            base.Reset();
            if (_wallCollider) _wallCollider.enabled = true;
            if (_spriteRenderer) _spriteRenderer.enabled = true;
        }

        public override void Reset()
        {
            if (!_isOpen) base.Reset();
        }
    }
}
