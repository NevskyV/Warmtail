using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Entities.Props;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Entities.Triggers
{
    [RequireComponent(typeof(Collider2D))]
    public class LightTrigger : SavableStateObject
    {
        [SerializeField] private Light2D _light;
        [SerializeField] private bool _destroyAfter;
        [SerializeField] private float _fadeTime;
        [SerializeField] private float _intensity;
        [SerializeField] private float _innerRadius;
        [SerializeField] private float _outerRadius;

        private async void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag != "Player") return;

            DOTween.To(()=> _light.intensity, x => _light.intensity = x, _intensity, _fadeTime);
            DOTween.To(()=> _light.pointLightOuterRadius, x => _light.pointLightOuterRadius = x, _outerRadius, _fadeTime);
            DOTween.To(()=> _light.pointLightInnerRadius, x => _light.pointLightInnerRadius = x, _innerRadius, _fadeTime);
            await UniTask.Delay(TimeSpan.FromSeconds(_fadeTime));
            if(_destroyAfter)ChangeState(false);
        }
    }
}