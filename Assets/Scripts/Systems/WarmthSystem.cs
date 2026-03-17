using Data;
using Data.Player;
using UnityEngine;
using Zenject;

namespace Systems
{
    public class WarmthSystem
    {
        public float WarmthConsumptionMultiplier { get; set; } = 1f;

        public int MaxCells => _globalData.Get<SavablePlayerData>().Stars;
        public bool HasCells => _globalData.Get<RuntimePlayerData>().CurrentCells > 0;

        private GlobalData _globalData;
        private int _lastStars;

        [Inject]
        private void Construct(GlobalData globalData)
        {
            _globalData = globalData;
            _lastStars = _globalData.Get<SavablePlayerData>().Stars;
            _globalData.Edit<RuntimePlayerData>(data =>
            {
                data.CurrentCells = _lastStars;
                data.CurrentCellProgress = 0f;
            });
            _globalData.SubscribeTo<SavablePlayerData>(OnStarsChanged);
        }

        private void OnStarsChanged()
        {
            var stars = _globalData.Get<SavablePlayerData>().Stars;
            if (stars <= _lastStars) { _lastStars = stars; return; }

            var gained = stars - _lastStars;
            _lastStars = stars;
            _globalData.Edit<RuntimePlayerData>(data =>
            {
                data.CurrentCells = Mathf.Min(data.CurrentCells + gained, stars);
            });
        }

        public bool DrainCurrentCell(float drainPercent)
        {
            if (drainPercent <= 0f) return true;

            var runtime = _globalData.Get<RuntimePlayerData>();
            if (runtime.CurrentCells <= 0) return false;

            var effectiveDrain = drainPercent * WarmthConsumptionMultiplier;
            bool hasRemaining = true;

            _globalData.Edit<RuntimePlayerData>(data =>
            {
                data.CurrentCellProgress += effectiveDrain;
                if (data.CurrentCellProgress >= 1f)
                {
                    data.CurrentCells = Mathf.Max(data.CurrentCells - 1, 0);
                    data.CurrentCellProgress = 0f;
                    hasRemaining = data.CurrentCells > 0;
                }
            });

            return hasRemaining;
        }

        public void ConsumeCurrentCell()
        {
            var runtime = _globalData.Get<RuntimePlayerData>();
            if (runtime.CurrentCells <= 0) return;

            _globalData.Edit<RuntimePlayerData>(data =>
            {
                data.CurrentCells = Mathf.Max(data.CurrentCells - 1, 0);
                data.CurrentCellProgress = 0f;
            });
        }

        public bool ConsumeCells(int count)
        {
            if (_globalData.Get<RuntimePlayerData>().CurrentCells < count) return false;

            _globalData.Edit<RuntimePlayerData>(data =>
            {
                data.CurrentCells = Mathf.Max(data.CurrentCells - count, 0);
                data.CurrentCellProgress = 0f;
            });
            return true;
        }

        public void AddCell()
        {
            var max = MaxCells;
            _globalData.Edit<RuntimePlayerData>(data =>
            {
                data.CurrentCells = Mathf.Min(data.CurrentCells + 1, max);
            });
        }
    }
}
