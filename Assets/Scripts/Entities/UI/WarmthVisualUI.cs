using System.Collections.Generic;
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
        [SerializeField] private List<SdfGroup> _groups;
        [SerializeField] private List<SdfFigure> _figures;
        [SerializeField, ColorUsage(false, true)] private Color _activeColor;
        [SerializeField, ColorUsage(false, true)] private Color _inactiveColor;
        [SerializeField] private float _activeOutline = 0.05f;
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
            if(_figures == null || _figures.Count <= 0 || !_figures[0]){ return;}
            var runtime = _globalData.Get<RuntimePlayerData>();
            var savable = _globalData.Get<SavablePlayerData>();

            for (int i = 0; i < _figures.Count; i++)
            {
                if (i < _figures.Count - savable.Stars)
                {
                    _figures[i].gameObject.SetActive(false);
                }
                else
                {
                    if (i + savable.Stars - _figures.Count < runtime.CurrentCells)
                    {
                        _groups[i].GroupProperty.FillColor = _activeColor;
                        _groups[i].GroupProperty.OutlineThickness = _activeOutline;
                    }
                    else
                    {
                        _groups[i].GroupProperty.FillColor = _inactiveColor;
                        _groups[i].GroupProperty.OutlineThickness = 0f;
                    }
                    _figures[i].gameObject.SetActive(true);
                }
            }
        }
    }
}
