using Interfaces;
using UnityEngine;

namespace Systems.Abilities
{
    public class ColdTailAbility
    {
        private readonly float _coolRadius;
        private readonly Rigidbody2D _playerRb;

        public ColdTailAbility(Rigidbody2D playerRb, float coolRadius = 2f)
        {
            _playerRb = playerRb;
            _coolRadius = coolRadius;
        }

        public void Tick()
        {
            if (_playerRb == null) return;
            var hits = Physics2D.OverlapCircleAll(_playerRb.position, _coolRadius);
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<Warmable>(out var warmable))
                    warmable.Reset();
            }
        }
    }
}
