namespace Interfaces
{
    public interface IAbilityVisual
    {
        public int AbilityIndex { get; set;}
        public void StartAbility();
        public void UsingAbility();
        public void EndAbility();
    }
}