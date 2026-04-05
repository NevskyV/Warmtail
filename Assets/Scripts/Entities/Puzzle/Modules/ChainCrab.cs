using Interfaces;
using UnityEngine;

namespace Entities.Puzzle.Modules
{
    public class ChainCrab : Warmable
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private float _warmRadius = 2f;
        private static readonly int WarmHash = Animator.StringToHash("Warm");

        private int _index;
        private CrabChainModule _module;
        private bool _triggered;
        private bool _playerActivatable;

        public void Initialize(int index, CrabChainModule module, bool playerActivatable = false)
        {
            _index = index;
            _module = module;
            _playerActivatable = playerActivatable;
        }

        public override void Warm()
        {
            if (!_playerActivatable) return;
            base.Warm();
        }

        public override void WarmComplete()
        {
            if (_triggered) return;
            TriggerChain();
        }

        public void TriggerChain()
        {
            if (_triggered) return;
            _triggered = true;
            if (_animator != null) _animator.SetTrigger(WarmHash);
            _module?.ReportCrabWarmed(_index);

            var hits = Physics2D.OverlapCircleAll(transform.position, _warmRadius);
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<ChainCrab>(out var crab) && crab != this)
                    crab.TriggerChain();
            }
        }

        public override void Reset()
        {
            base.Reset();
            _triggered = false;
        }
    }
}
