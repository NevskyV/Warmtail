using System.Collections.Generic;
using System.Linq;
using Data.Player;
using Systems.Abilities;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Entities.UI
{
    public class AbilitiesUI : MonoBehaviour
    {
        [Title("Images to fill")]
        [SerializeField] private Image[] _images;

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
            foreach (var ability in _warmthAbilities)
            {
                _images[_warmthAbilities.IndexOf(ability)].transform.parent.gameObject.SetActive(show && ability.InUse);
                _images[_warmthAbilities.IndexOf(ability)].sprite = ability.Visual.Icon;
            }
        }
        
        private void SelectAbility(int index)
        {
            for(int i = 0;  i < _images.Length; i++)
            {
                _images[i].transform.parent.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1);
            }
            _images[index].transform.parent.GetComponent<RectTransform>().localScale = new Vector3(1.3f, 1.3f, 1);
        }

        private void ConfirmAbility(int index)
        {
            _images[index].transform.localScale = _images[index].transform.localScale == new Vector3(1.3f, 1.3f, 1)?  new Vector3(1, 1, 1) : new Vector3(1.3f, 1.3f, 1);
        }

        private void Cast(List<int> warmthAbilities)
        {
        }
        
        private void StopCast(List<int> warmthAbilities)
        {
        }
        
        private void AddAbility(int index)
        {
            ShowAbilities();
        }
    }
}
