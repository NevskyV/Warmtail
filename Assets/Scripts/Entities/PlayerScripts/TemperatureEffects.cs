using Data;
using Data.Player;
using Systems;
using UnityEngine;
using Zenject;

namespace Entities.PlayerScripts
{
    public class TemperatureEffects : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private AnimationCurve _visibilityCurve = new(
            new Keyframe(0f, 0.6f),
            new Keyframe(0.5f, 1f),
            new Keyframe(1f, 0.6f)
        );

        private GlobalData _globalData;

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
        }

        private void ApplyWidth(float t) { }
    }
}
