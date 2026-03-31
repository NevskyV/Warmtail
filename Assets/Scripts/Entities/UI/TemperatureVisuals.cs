using Data;
using Data.Player;
using DG.Tweening;
using Entities.UI.SDF;
using Systems;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Entities.UI
{
    public class TemperatureVisuals : MonoBehaviour
    {
        [SerializeField] private RectTransform _separator;
        [SerializeField] private Vector2 _rotationBounds;
        [SerializeField] private SdfGroup _temperatureGroup;
        [SerializeField] private Image _temperatureImage;
        [SerializeField,] private Gradient _gradient;
        private GlobalData _globalData;
        private SdfFigure _separatorFigure;
        
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
            var savable = _globalData.Get<SavablePlayerData>();

            var temp = runtime.Temperature;
            var lerpValue = temp / TemperatureSystem.Max;
            _separator.localRotation =  Quaternion.Euler(0f, 0f, Mathf.Lerp(_rotationBounds.x, _rotationBounds.y, lerpValue));
            _temperatureGroup.GroupProperty.FillColor = _gradient.Evaluate(lerpValue);
            _temperatureImage.color = _gradient.Evaluate(lerpValue);
        }
    }
}