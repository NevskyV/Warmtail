using System;

namespace Entities.Puzzle.Modules
{
    [Serializable]
    public abstract class PuzzleModule
    {
        public event Action<PuzzleModule> OnSolve = delegate { };

        private bool _solved;

        public bool IsSolved => _solved;

        public virtual void Activate() { }

        public virtual void Solve()
        {
            if (_solved) return;
            _solved = true;
            OnSolve.Invoke(this);
        }

        public virtual void Reset()
        {
            _solved = false;
        }
    }
}