using Data;
using Systems.Abilities;
using Zenject;

namespace Systems
{
    public class ComboSystem
    {
        public WarmthAbility AbilityType1;
        public WarmthAbility AbilityType2;
        [Inject] private ComboConfig _comboConfig;

        public void SetCombo(WarmthAbility ability1, WarmthAbility ability2)
        {
            AbilityType1 = ability1;
            AbilityType2 = ability2;
            foreach (var data in _comboConfig.Data)
            {
                SetAbilityMethod(ability1, data);
                SetAbilityMethod(ability2, data);
            }
        }

        public void DisableCombo(WarmthAbility ability1, WarmthAbility ability2)
        {
            ability1.MethodName = ability1.BaseMethodName;
            ability2.MethodName = ability2.BaseMethodName;
            AbilityType1 = null;
            AbilityType2 = null;
        }

        private void SetAbilityMethod(WarmthAbility ability, ComboData data)
        {
            if (data.ReturnType(data.FirstAbility) == ability.GetType())
            {
                ability.MethodName = data.FirstMethodName;
            }
            else if (data.ReturnType(data.SecondAbility) == ability.GetType())
            {
                ability.MethodName = data.SecondMethodName;
            }
        }
    }
}