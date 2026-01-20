using System;

namespace Interfaces
{
    public interface IAbility
    {
        public bool Enabled { get; set; }
        public Action StartAbility { get; set; }
        public Action UsingAbility { get; set; }
        public Action EndAbility { get; set; }
        public IAbilityVisual Visual { get; set; }
    }
}
