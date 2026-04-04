using DG.Tweening;
using Interfaces;
using UnityEngine;

namespace Entities.Puzzle.Modules
{
    public class RotatingGear : Warmable
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private float _targetAngle = 360f;
        [SerializeField] private float _rotateDuration = 1.5f;

        private GearCircleModule _module;
        private bool _solved;

        public void Initialize(GearCircleModule module)
        {
            _module = module;
            if (_spriteRenderer)
            {
                var c = _spriteRenderer.color;
                c.a = 0.5f;
                _spriteRenderer.color = c;
            }
        }

        public override void WarmComplete()
        {
            if (_solved) return;
            _solved = true;

            if (_spriteRenderer)
            {
                var c = _spriteRenderer.color;
                c.a = 1f;
                _spriteRenderer.color = c;
                _spriteRenderer.transform
                    .DOLocalRotate(new Vector3(0, 0, _targetAngle), _rotateDuration, RotateMode.FastBeyond360)
                    .OnComplete(() => _module?.Solve());
            }
            else
            {
                _module?.Solve();
            }
        }

        public override void Reset()
        {
            base.Reset();
            _solved = false;
            if (_spriteRenderer)
            {
                var c = _spriteRenderer.color;
                c.a = 0.5f;
                _spriteRenderer.color = c;
                _spriteRenderer.transform.DOLocalRotate(Vector3.zero, 0.5f);
            }
        }
    }
}
