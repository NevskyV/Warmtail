using UnityEngine;

namespace Interfaces
{
    public interface IAbilityVisual
    {
        public int AbilityIndex { get; set;}
        public Sprite Icon { get; }
        public void StartAbility();
        public void UsingAbility();
        public void EndAbility();
    }
}