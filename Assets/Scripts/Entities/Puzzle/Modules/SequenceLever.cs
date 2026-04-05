using Entities.Puzzle;
using Interfaces;
using Systems;
using UnityEngine;

namespace Entities.Puzzle.Modules
{
    public class SequenceLever : Warmable
    {
        [SerializeField] private float _activeDuration = 3f;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Sprite _onSprite;
        [SerializeField] private Sprite _offSprite;

        private int _index;
        private LeversSequenceModule _module;
        private ResettableTimer _deactivateTimer;
        private bool _active;

        public void Initialize(int index, LeversSequenceModule module)
        {
            _index = index;
            _module = module;
        }

        public override void WarmComplete()
        {
            if (_active) return;
            _active = true;
            if (_spriteRenderer && _onSprite) _spriteRenderer.sprite = _onSprite;
            _deactivateTimer ??= new ResettableTimer(_activeDuration, Deactivate);
            _deactivateTimer.Start();
            _module?.ReportActivated(_index);
        }

        private void Deactivate()
        {
            _active = false;
            base.Reset();
            if (_spriteRenderer && _offSprite) _spriteRenderer.sprite = _offSprite;
            _module?.ReportDeactivated();
        }

        public override void Reset()
        {
            base.Reset();
            _active = false;
            if (_spriteRenderer && _offSprite) _spriteRenderer.sprite = _offSprite;
        }
    }
}
