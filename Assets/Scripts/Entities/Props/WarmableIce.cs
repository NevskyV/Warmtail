using DG.Tweening;
using Interfaces;
using Systems;
using UnityEngine;
using UnityEngine.U2D;

namespace Entities.Props
{
    public class WarmableIce : Warmable
    {
        private static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");
        [SerializeField] private SpriteRenderer _renderer;
        
        private Tween _tween;
        private MaterialPropertyBlock _propertyBlock;

        private void Start()
        {
            _propertyBlock = new();
            Reset();
        }
        
        public override void Warm()
        {
            UpdateRenderer((_maxWarmthAmount - _warmthAmount) * 1.0f / _maxWarmthAmount,
                (_maxWarmthAmount - _warmthAmount + _warmFactor) * 1.0f / _maxWarmthAmount);
            base.Warm();
        }

        public override void WarmComplete()
        {
            _timer.Stop();
            ChangeState(false);
        }

        public override void Reset()
        {
            UpdateRenderer((_maxWarmthAmount - _warmthAmount) * 1.0f / _maxWarmthAmount, 0);
            base.Reset();
        }
        
        private async void UpdateRenderer(float lastAmount, float newAmount)
        {
            if (!_renderer) return;
            _tween?.Pause();
            _propertyBlock.SetFloat(DissolveAmount, newAmount);
            _renderer.SetPropertyBlock(_propertyBlock);
            _tween = DOTween.To(() => lastAmount, x =>{
                if (!_renderer)
                {
                    _tween?.Pause();
                    return;
                }

                lastAmount = x;
                _propertyBlock.SetFloat(DissolveAmount, x);
                _renderer.SetPropertyBlock(_propertyBlock);
            }, newAmount, 0.5f);
            await _tween.AsyncWaitForCompletion();
        }
    }
}