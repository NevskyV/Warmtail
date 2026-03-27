using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Player;
using UnityEngine;
using Zenject;

namespace Entities.UI
{
    public class FearCollectionUI : MonoBehaviour
    {
        [SerializeField] private FearCardView _cardPrefab;
        [SerializeField] private Transform _contentRoot;
        [SerializeField] private List<FearConfig> _configs = new();

        [Inject] private GlobalData _globalData;

        private readonly List<FearCardView> _spawned = new();

        private void OnEnable()
        {
            Rebuild();
        }

        public void Rebuild()
        {
            if (_cardPrefab == null || _contentRoot == null || _globalData == null) return;

            for (int i = 0; i < _spawned.Count; i++)
            {
                if (_spawned[i] != null) Destroy(_spawned[i].gameObject);
            }
            _spawned.Clear();

            var data = _globalData.Get<SavablePlayerData>();
            var collected = data.FearIds ?? new List<int>();
            var activeId = data.ActiveFearId;
            
            var configs = _configs != null && _configs.Count > 0
                ? _configs.Where(c => c != null).ToList()
                : Resources.LoadAll<FearConfig>(string.Empty).Where(c => c != null).ToList();

            for (int i = 0; i < configs.Count; i++)
            {
                var config = configs[i];
                var view = Instantiate(_cardPrefab, _contentRoot);
                _spawned.Add(view);

                bool isCollected = collected.Contains(config.Id);
                bool isActive = config.Id == activeId;

                view.Bind(config, isCollected, isActive, () =>
                {
                    _globalData.Edit<SavablePlayerData>(d => d.ActiveFearId = config.Id);
                    RefreshActiveState();
                });
            }
        }

        private void RefreshActiveState()
        {
            var activeId = _globalData.Get<SavablePlayerData>().ActiveFearId;
            for (int i = 0; i < _spawned.Count; i++)
            {
                var v = _spawned[i];
                if (v == null) continue;
            }

            Rebuild();
        }
    }
}

