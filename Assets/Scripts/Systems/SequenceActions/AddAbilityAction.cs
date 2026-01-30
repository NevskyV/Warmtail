using Data;
using Interfaces;
using Systems.Abilities;
using UnityEngine;

namespace Systems.SequenceActions
{
    public class AddAbilityAction : ISequenceAction
    {
        [SerializeField] private AbilityType _type;
        
        public void Invoke()
        {
            AbilitiesSystem.Instance.AddAbility(_type);
        }
    }
}