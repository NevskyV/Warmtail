using System;
using Interfaces;
using Entities.Puzzle;
using UnityEngine;

namespace Systems.Tasks
{
    public class PuzzlesTask : ITask
    {
        public bool Completed { get; set; }
        public Action OnComplete { get; set; }
        [SerializeField] private string _puzzleId;

        public void Activate()
        {
            LeversPuzzle.OnPuzzleSolved += MarkComplete;
        }

        private void MarkComplete(string id)
        {
            if (_puzzleId != id) return;
            Completed = true;
            OnComplete?.Invoke();
            LeversPuzzle.OnPuzzleSolved -= MarkComplete;
        }
    }
}