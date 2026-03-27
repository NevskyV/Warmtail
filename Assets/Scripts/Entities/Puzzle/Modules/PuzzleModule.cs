using System;

namespace Entities.Puzzle.Modules
{
    [Serializable]
    public abstract class PuzzleModule
    {
        public event Action<PuzzleModule> OnSolve = delegate { };
        
        public virtual void Activate() { }

        public virtual void Solve()
        {
            OnSolve.Invoke(this);
        }
    }
}