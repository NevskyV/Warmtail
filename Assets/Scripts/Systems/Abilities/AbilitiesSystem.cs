using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Systems.Abilities
{
    [Serializable]
    public class AbilitiesSystem
    {
        private PlayerConfig _config;
        private ComboSystem _comboSystem;
        private List<WarmthAbility> _allAbilities;
        private List<WarmthAbility> _activeAbilities;
        private int _selectedIndex;
        
        [Inject]
        public void Construct(PlayerConfig config, GlobalData globalData, PlayerInput input, ComboSystem comboSystem)
        {
            _config = config;
            _comboSystem = comboSystem;
            _allAbilities = _config.Abilities.Where(x => x.GetType() == typeof(WarmthAbility))
                .Cast<WarmthAbility>().ToList();
            
            SetupInput(input);
        }

        private void SetupInput(PlayerInput input)
        {
            input.actions["Scroll"].performed += ctx => CycleSelection(ctx.ReadValue<Vector2>().y);

            input.actions["1"].performed += _ => SelectAbility(0);
            input.actions["2"].performed += _ => SelectAbility(1);
            input.actions["3"].performed += _ => SelectAbility(2);
            input.actions["4"].performed += _ => SelectAbility(3);

            input.actions["RightMouse"].started += _ => StartCasting();
            input.actions["RightMouse"].canceled += _ => StopCasting();
        }

        private void CycleSelection(float scrollValue)
        {
            if (scrollValue > 0) SelectAbility(_selectedIndex + 1);
            else if (scrollValue < 0) SelectAbility(_selectedIndex - 1);
        }

        private void SelectAbility(int index)
        {
            if (_activeAbilities.Contains(_allAbilities[index]))
            {
                StopCasting();
                if (_activeAbilities.Count > 1) _comboSystem.DisableCombo(_activeAbilities[0], _activeAbilities[1]);
                
                _activeAbilities.Remove(_allAbilities[index]);
                StartCasting();
                return;
            }
            
            _selectedIndex = index;
            _activeAbilities.Add(_allAbilities[index]);
            if (_activeAbilities.Count > 1)
            {
                _comboSystem.SetCombo(_activeAbilities[0], _activeAbilities[1]);
            }

            StartCasting();
        }

        private void StartCasting()
        {
            _activeAbilities.ForEach(x => x.UseAbility());
        }
        
        private void StopCasting()
        {
            _activeAbilities.ForEach(x => x.StopAbility());
        }
    }
}
