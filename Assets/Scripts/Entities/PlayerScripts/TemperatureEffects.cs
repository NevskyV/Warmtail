using Data;
using Data.Player;
using DG.Tweening;
using Systems;
using UnityEngine;
using Zenject;

namespace Entities.PlayerScripts
{
    public class TemperatureEffects : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private Transform _scaleTarget;
        [SerializeField] private AnimationCurve _visibilityCurve = new(
            new Keyframe(0f, 0.6f),
            new Keyframe(0.5f, 1f),
            new Keyframe(1f, 0.6f)
        );
        [SerializeField] private AnimationCurve _widthCurve = new(
            new Keyframe(0f, 0.6f),
            new Keyframe(0.5f, 1f),
            new Keyframe(1f, 1.25f)
        );
        [SerializeField] private float _scaleTweenDuration = 0.3f;

        private GlobalData _globalData;
        private Tween _scaleTween;

        [Inject]
        private void Construct(GlobalData globalData)
        {
            _globalData = globalData;
            _globalData.SubscribeTo<RuntimePlayerData>(OnDataChanged);
            OnDataChanged();
        }

        private void OnDataChanged()
        {
            var temp = _globalData.Get<RuntimePlayerData>().Temperature;
            var t = temp / TemperatureSystem.Max;

            if (_renderer != null)
            {
                var c = _renderer.color;
                c.a = _visibilityCurve.Evaluate(t);
                _renderer.color = c;
            }

            ApplyWidth(t);
        }

        private void ApplyWidth(float t)
        {
            var target = _scaleTarget != null ? _scaleTarget : transform;
            float targetScale = _widthCurve.Evaluate(t);
            var newScale = new Vector3(targetScale, target.localScale.y, target.localScale.z);

            _scaleTween?.Kill();
            _scaleTween = target.DOScale(newScale, _scaleTweenDuration).SetEase(Ease.OutSine);
        }

        private void OnDestroy()
        {
            _scaleTween?.Kill();
        }
    }
}
