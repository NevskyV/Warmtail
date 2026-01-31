using Data;
using Entities.Props;
using Systems.Abilities;
using UnityEngine;
using Zenject;

namespace Entities.Triggers
{
    public class NewAbilityTrigger : SavableStateObject
    {
        [SerializeField] private AbilityType _type;
        [Inject] private AbilitiesSystem _abilitySystem;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _abilitySystem.AddAbility(_type);
                ChangeState(false);
            }
        }
    }
}