using System.Collections.Generic;
using Data;
using Systems.Fears;
using UnityEngine;
using Zenject;

namespace Entities.UI
{
    public class FearCollectionUI : MonoBehaviour
    {
        [SerializeField] private List<FearConfig> _fearConfigs = new();
        [SerializeField] private FearCardView _cardPrefab;
        [SerializeField] private Transform _cardsParent;

        private FearSystem _fearSystem;
        private readonly Dictionary<int, FearCardView> _cardsById = new();
        private bool _isInitialized;

        [Inject]
        private void Construct(FearSystem fearSystem)
        {
            _fearSystem = fearSystem;
        }

        private void Start()
        {
            if (_isInitialized) return;
            _isInitialized = true;

            if (_cardPrefab == null || _cardsParent == null) return;

            _fearSystem.RegisterConfigs(_fearConfigs);
            BuildCards();
            UpdateActiveCard(_fearSystem.ActiveFearId);
        }

        private void BuildCards()
        {
            _cardsById.Clear();
            foreach (var config in _fearConfigs)
            {
                if (config == null) continue;
                var card = Instantiate(_cardPrefab, _cardsParent);
                card.Initialize(config, OnCardClicked);
                _cardsById[config.Id] = card;
            }
        }

        private void OnCardClicked(FearConfig config)
        {
            if (config == null) return;
            _fearSystem.SetActiveFear(config);
            UpdateActiveCard(_fearSystem.ActiveFearId);
        }

        private void UpdateActiveCard(int activeId)
        {
            foreach (var card in _cardsById)
            {
                card.Value.SetActive(card.Key == activeId);
            }
        }
    }
}
