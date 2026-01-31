using Interfaces;
using UnityEngine;

namespace Systems.AbilitiesVisual
{
    public class ResonanceVisuals : IAbilityVisual
    {
        [field: SerializeReference] public int AbilityIndex { get; set; }
        [field: SerializeReference] public Sprite Icon { get; set; }
        public void StartAbility()
        {
            throw new System.NotImplementedException();
        }

        public void UsingAbility()
        {
            throw new System.NotImplementedException();
        }

        public void EndAbility()
        {
            throw new System.NotImplementedException();
        }
    }
}