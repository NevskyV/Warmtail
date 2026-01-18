using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Systems.Abilities;
using TriInspector;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Combo Config", menuName = "Configs/Combo Config")]
    public class ComboConfig : ScriptableObject
    {
        [field: SerializeField,TableList(ShowAlternatingBackground = true,Draggable = true, HideAddButton = false, HideRemoveButton = false, AlwaysExpanded = true)] 
        public List<ComboData> Data { get; private set; } = new();
        
        // EXAMPLE USAGE
        // private void F(ComboConfig comboConfig)
        // {
        //     comboConfig.Data[0].ReturnType(comboConfig.Data[0].FirstAbility);
        //     var s = comboConfig.Data[0].SecondMethodName;
        // }
    }

    [Serializable]
    public struct ComboData
    {
        [field:SerializeReference, Group("First ability"), HideLabel] public AbilityType FirstAbility { get;  private set; }
        [field:SerializeReference,Group("Second ability"), HideLabel] public AbilityType SecondAbility { get;  private set; }
        [field:SerializeReference,Dropdown(nameof(GetTypeMethods)),Group("First ability"), HideLabel] public string FirstMethodName { get;  private set; }
        [field:SerializeReference,Dropdown(nameof(GetTypeMethodsSecond)), Group("Second ability"), HideLabel] public string SecondMethodName{ get;  private set; }

        public Type ReturnType(AbilityType abilityType)
        {
            switch (abilityType)
            {
                case AbilityType.Movement: return typeof(PlayerMovement);
                case AbilityType.Interaction: return typeof(InteractionAbility);
                case AbilityType.Warming: return typeof(WarmingAbility);
                case AbilityType.Resonance: return typeof(ResonanceAbility);
                case AbilityType.Metabolism: return typeof(MetabolismAbility);
                case AbilityType.Dash: return typeof(DashAbility);
                default: throw new ArgumentOutOfRangeException(nameof(abilityType));
            }
        }
        
        private IEnumerable<TriDropdownItem<string>> GetTypeMethods()
        {
            TriDropdownList<string> list = new();
            List<MethodInfo> infos = ReturnType(FirstAbility).GetRuntimeMethods().ToList();
            infos.ForEach(x =>
            {
                list.Add(new TriDropdownItem<string>() { Text = x.Name, Value =  x.Name  });
            });
            return list;
        }
        
        private IEnumerable<TriDropdownItem<string>> GetTypeMethodsSecond()
        {
            TriDropdownList<string> list = new();
            List<MethodInfo> infos = ReturnType(SecondAbility).GetRuntimeMethods().ToList();
            infos.ForEach(x =>
            {
                list.Add(new TriDropdownItem<string>() { Text = x.Name, Value =  x.Name });
            });
            return list;
        }

        public enum AbilityType
        {Movement, Interaction, Warming, Resonance, Metabolism, Dash}
    }
}