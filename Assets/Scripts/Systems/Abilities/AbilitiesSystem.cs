

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
        private List<int> _activeAbilities = new();
        private List<int> _confirmedAbilities = new();
        private int _selectedIndex;

        public Action<int> OnSelect;
        public Action<int> OnConfirm;
        public Action<List<int>> OnCast;
        public Action<List<int>> OnStopCast;
        public Action<int> OnAddAbility;
        
        [Inject]
        public void Construct(PlayerConfig config, GlobalData globalData, PlayerInput input, ComboSystem comboSystem)
        {
            _config = config;
            _comboSystem = comboSystem;
            _allAbilities = _config.Abilities.OfType<WarmthAbility>().Where(x => x.Enabled).ToList();
            
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
            if (index < 0 || index >= _allAbilities.Count)
            {
                return;
            }
            
            var ability = _allAbilities[index];
            Debug.Log("ability: " + ability);
            
            _selectedIndex = index;
            OnSelect?.Invoke(index);
        }

        private void StartCasting()
        {
            _activeAbilities.Add(_selectedIndex);
            if (_activeAbilities.Count > 1)
            {
                _comboSystem.SetCombo(_allAbilities[_activeAbilities[0]], _allAbilities[_activeAbilities[1]]);
            }
            _activeAbilities.ForEach(x => _allAbilities[x].UseAbility());
            OnCast?.Invoke(_activeAbilities);
        }
        
        private void StopCasting()
        {
            OnStopCast?.Invoke(_activeAbilities);
            _activeAbilities.ForEach(x => _allAbilities[x].StopAbility());
            _activeAbilities.Clear();
        }

        public void AddAbility(WarmthAbility ability)
        {
            _allAbilities.Add(ability);
            OnAddAbility?.Invoke(_allAbilities.Count-1);
        }

        private void ConfirmAbility(int index)
        {
            if (_confirmedAbilities.Contains(index))
            {
                StopCasting();
                if (_activeAbilities.Count > 1) 
                {
                    _comboSystem.DisableCombo(_allAbilities[_activeAbilities[0]], _allAbilities[_activeAbilities[1]]);
                }
                
                _confirmedAbilities.Remove(index);
                _activeAbilities.Remove(index);
                
                if (_activeAbilities.Count > 0)
                {
                    StartCasting();
                }
            }
            else
            {
                _confirmedAbilities.Add(index);
            }
            OnConfirm?.Invoke(index);
        }
    }
}
