using Interfaces;
using UnityEngine;

namespace Entities.Puzzle.Modules
{
    public class ChainCrab : Warmable
    {
        [SerializeField] private Animator _animator;
        private static readonly int WarmHash = Animator.StringToHash("Warm");

        private int _index;
        private CrabChainModule _module;
        private bool _triggered;

        public void Initialize(int index, CrabChainModule module)
        {
            _index = index;
            _module = module;
        }

        public override void WarmComplete()
        {
            if (_triggered) return;
            TriggerChain();
        }

        public void TriggerChain()
        {
            _triggered = true;
            if (_animator != null) _animator.SetTrigger(WarmHash);
            _module?.ReportCrabWarmed(_index);
        }

        public override void Reset()
        {
            base.Reset();
            _triggered = false;
        }
    }
}
