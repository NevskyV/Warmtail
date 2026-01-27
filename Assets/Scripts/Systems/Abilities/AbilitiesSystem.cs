using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Data;
using Data.Player;
using EasyTextEffects.Editor.MyBoxCopy.Extensions;
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
        private GlobalData _globalData;
        private List<WarmthAbility> _allAbilities = new();
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
            _globalData = globalData;

            var openedCount = globalData.Get<SavablePlayerData>().OpenedAbilitiesCount;
            var warmthAbilities = _config.Abilities.OfType<WarmthAbility>().ToList();
            for (int i = 0; i < openedCount; i++)
            {
                warmthAbilities[i].InUse = true;
                _allAbilities.Add(warmthAbilities[i]);
            }
            for (int i = openedCount; i < warmthAbilities.Count; i++)
            {
                warmthAbilities[i].InUse = false;
            }

            SetupInput(input);
        }

        private async void SetupInput(PlayerInput input)
        {
            input.actions["Scroll"].performed += ctx => CycleSelection(ctx.ReadValue<Vector2>().y);

            input.actions["1"].performed += _ => SelectAbility(0);
            input.actions["2"].performed += _ => SelectAbility(1);
            input.actions["3"].performed += _ => SelectAbility(2);
            input.actions["4"].performed += _ => SelectAbility(3);

            input.actions["RightMouse"].started += _ => StartCasting();
            input.actions["RightMouse"].canceled += _ => StopCasting();
            input.actions["MiddleMouse"].canceled += _ => ConfirmAbility(_selectedIndex);
            await UniTask.Delay(100);
            SelectAbility(0);
        }

        private void CycleSelection(float scrollValue)
        {
            if (scrollValue > 0) SelectAbility(_selectedIndex + 1);
            else if (scrollValue < 0) SelectAbility(_selectedIndex - 1);
        }

        private void SelectAbility(int index)
        {
            if(_allAbilities.Count <= 0) return;
            if (index < 0)
            {
                index = _allAbilities.Count-1;
            } 
            else if(index >= _allAbilities.Count)
            {
                index = 0;
            }
            
            var ability = _allAbilities[index];
            Debug.Log("ability: " + ability);
            
            _selectedIndex = index;
            OnSelect?.Invoke(index);
        }

        private void StartCasting()
        {
            if (_confirmedAbilities.Count <= 0 || _allAbilities.Count <= 0) return;
            
            _activeAbilities.AddRange(_confirmedAbilities);
            if (_activeAbilities.Count > 1)
            {
                _comboSystem.SetCombo(_allAbilities[_activeAbilities[0]], _allAbilities[_activeAbilities[1]]);
            }
            _activeAbilities.ForEach(x => _allAbilities[x].UseAbility());
            OnCast?.Invoke(_activeAbilities);
        }
        
        private void StopCasting()
        {
            if (_activeAbilities.Count <= 0 || _allAbilities.Count <= 0) return;
            OnStopCast?.Invoke(_activeAbilities);
            _activeAbilities.ForEach(x => _allAbilities[x].StopAbility());
            _activeAbilities.Clear();
            SelectAbility(_selectedIndex);
        }

        public void AddAbility(WarmthAbility ability)
        {
            if (_allAbilities.Contains(ability)) return;
            _allAbilities.Add(ability);
            ability.InUse = true;
            SelectAbility(_allAbilities.Count - 1);
            OnAddAbility?.Invoke(_allAbilities.Count-1);
            _globalData.Edit<SavablePlayerData>(x => x.OpenedAbilitiesCount++);
        }
        
        public void AddAbility(AbilityType type)
        {
            switch (type)
            {
                case AbilityType.Warming: AddAbility(_config.Abilities.OfType<WarmingAbility>().First()); break;
                case AbilityType.Resonance: AddAbility(_config.Abilities.OfType<ResonanceAbility>().First()); break;
                case AbilityType.Metabolism: AddAbility(_config.Abilities.OfType<MetabolismAbility>().First()); break;
                case AbilityType.Dash: AddAbility(_config.Abilities.OfType<DashAbility>().First()); break;
            }
        }

        private void ConfirmAbility(int index)
        {
            Debug.Log(_confirmedAbilities.Contains(index));
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
