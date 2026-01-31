using Data;
using Data.Player;
using DG.Tweening;
using Entities.UI.SDF;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Entities.UI
{
    public class WarmthVisualUI : MonoBehaviour
    {
        [Title("UI Elements")]
        [SerializeField] private SdfFigure _arcFigure;
        [SerializeField] private float _maxValue = 2.7f;
        [SerializeField] private float _smoothing = 0.5f;

        private GlobalData _globalData;
        private Tween _tween;

        [Inject]
        private void Construct(GlobalData globalData)
        {
            _globalData = globalData;
            _globalData.SubscribeTo<RuntimePlayerData>(UpdateVisual);
            UpdateVisual();
        }

        private void UpdateVisual()
        {
            var data = _globalData.Get<SavablePlayerData>();
            var runtimeData = _globalData.Get<RuntimePlayerData>();

            if (data.Stars == 0)
            {
                if (_arcFigure == null) return;
                _arcFigure.ShapeData.ParamsA.x = 0;
            }
            else
            {
                if (_arcFigure == null) return;
                
                _tween?.Pause();
                var newAmount = runtimeData.CurrentWarmth / (data.Stars * 10.0f);
                _tween = DOTween.To(() => _arcFigure.ShapeData.ParamsA.x, x =>
                {
                    _arcFigure.ShapeData.ParamsA.x = x;
                }, newAmount * _maxValue, _smoothing);
            }
        }
    }
}
