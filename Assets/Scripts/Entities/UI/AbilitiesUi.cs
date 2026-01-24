using System.Collections.Generic;
using System.Linq;
using Data.Player;
using Systems.Abilities;
using TriInspector;
using UnityEngine;
using Zenject;

namespace Entities.UI
{
    public class AbilitiesUI : MonoBehaviour
    {
        [Title("Images to fill")]
        [SerializeField] private GameObject[] _images;

        [Title("Selection")] 
        [SerializeField] private float _defaultOutWidth;
        [SerializeField] private float _selectedOutWidth;
        [SerializeField] private float _defaultInOutWidth;
        [SerializeField] private float _selectedInOutWidth;
        [Title("Confirmation")]
        [SerializeField] private float _defaultInWidth;
        [SerializeField] private float _confirmedInWidth;
        [SerializeField] private float _defaultRhombusSize;
        [SerializeField] private float _confirmedRhombusSize;
        [Title("Casting")]
        [SerializeField] private float _defaultAmplitude;
        [SerializeField] private float _activeAmplitude;
        [SerializeField] private float _defaultOpacity;
        [SerializeField] private float _activeOpacity;
        
        private List<WarmthAbility> _warmthAbilities;
        private int _selectedIndex;
        private PlayerConfig _playerConfig;
        private AbilitiesSystem _abilitiesSystem;

        [Inject]
        private void Construct(PlayerConfig playerConfig, AbilitiesSystem abilitiesSystem)
        {
            _playerConfig = playerConfig;
            _abilitiesSystem =  abilitiesSystem;
            _warmthAbilities = _playerConfig.Abilities.OfType<WarmthAbility>().ToList();
            ShowAbilities();
            
            _abilitiesSystem.OnSelect += SelectAbility;
            _abilitiesSystem.OnConfirm += ConfirmAbility;
            _abilitiesSystem.OnCast += Cast;
            _abilitiesSystem.OnStopCast += StopCast;
            _abilitiesSystem.OnAddAbility += AddAbility;
        }
        
        private void OnDestroy()
        {
            _abilitiesSystem.OnSelect -= SelectAbility;
            _abilitiesSystem.OnConfirm -= ConfirmAbility;
            _abilitiesSystem.OnCast -= Cast;
            _abilitiesSystem.OnStopCast -= StopCast;
            _abilitiesSystem.OnAddAbility -= AddAbility;
        }

        private void ShowAbilities(bool show = true)
        {
            foreach (var ability in _warmthAbilities.Where(x => x.Enabled))
            {
                _images[_warmthAbilities.IndexOf(ability)].SetActive(show);
            }
        }
        
        private void SelectAbility(int index)
        {
        }

        private void ConfirmAbility(int index)
        {
        }

        private void Cast(List<int> warmthAbilities)
        {
        }
        
        private void StopCast(List<int> warmthAbilities)
        {
        }
        
        private void AddAbility(int index)
        {
        }
    }
}
