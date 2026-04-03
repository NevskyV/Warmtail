using Interfaces;
using UnityEngine;

namespace Systems.Abilities
{
    public class HeavyTailAbility
    {
        private readonly float _destroyRadius;
        private readonly Rigidbody2D _playerRb;

        public HeavyTailAbility(Rigidbody2D playerRb, float destroyRadius = 1.2f)
        {
            _playerRb = playerRb;
            _destroyRadius = destroyRadius;
        }

        public void Tick()
        {
            if (_playerRb == null) return;
            var hits = Physics2D.OverlapCircleAll(_playerRb.position, _destroyRadius);
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<IDestroyable>(out var dest))
                    dest.DestroyObject();
            }
        }
    }
}
