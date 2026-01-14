using Data;
using Data.Player;
using DG.Tweening;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Entities.UI
{
    public class WarmthVisualUI : MonoBehaviour
    {
        [Title("UI Elements")]
        [SerializeField, LabelText("Heat Fill Bar")] private Image _heatFillBar;

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
            // Проверка В НАЧАЛЕ метода
            if (_heatFillBar == null)
            {
                Debug.LogWarning("WarmthVisualUI: Heat fill bar is null!");
                return;
            }
            
            var data = _globalData.Get<SavablePlayerData>();
            var runtimeData = _globalData.Get<RuntimePlayerData>();

            if (data == null || runtimeData == null)
            {
                Debug.LogWarning("WarmthVisualUI: Player data is null!");
                return;
            }

            if (data.Stars == 0)
            {
                _heatFillBar.gameObject.SetActive(false);
                _heatFillBar.fillAmount = 0;
            }
            else
            {
                _heatFillBar.gameObject.SetActive(true);
                //TODO animation
                _tween?.Pause();
                var newAmount = runtimeData.CurrentWarmth / (data.Stars * 10.0f);
                _tween = _heatFillBar.DOFillAmount(newAmount, 1f);
            }
        }
    }
}
