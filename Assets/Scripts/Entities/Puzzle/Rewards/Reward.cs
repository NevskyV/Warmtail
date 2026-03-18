using System;

namespace Entities.Puzzle.Rewards
{
    [Serializable]
    public abstract class Reward
    {
        public abstract void Get();
    }
}