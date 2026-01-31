using Systems;
using Systems.Abilities;

namespace Interfaces
{
    public interface IFearBuff
    {
        void Apply(WarmthSystem warmthSystem, PlayerMovement playerMovement);
        void Remove(WarmthSystem warmthSystem, PlayerMovement playerMovement);
    }
}
