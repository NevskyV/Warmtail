using Data;
using Data.Player;
using DG.Tweening;
using Entities.UI.SDF;
using TMPro;
using UnityEngine;
using Zenject;

namespace Entities.UI
{
    public class WarmthVisualUI : MonoBehaviour
    {
        [SerializeField] private SdfFigure _arcFigure;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private float _maxAngle = 2.7f;
        [SerializeField] private float _smoothing = 0.2f;

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
            var runtime = _globalData.Get<RuntimePlayerData>();

            if (_text != null)
                _text.text = runtime.CurrentCells.ToString();

            if (_arcFigure == null) return;

            float targetAngle = runtime.CurrentCells > 0
                ? (1f - runtime.CurrentCellProgress) * _maxAngle
                : 0f;

            _tween?.Kill();
            _tween = DOTween.To(
                () => _arcFigure.ShapeData.ParamsA.x,
                x => _arcFigure.ShapeData.ParamsA.x = x,
                targetAngle,
                _smoothing);
        }
    }
}
