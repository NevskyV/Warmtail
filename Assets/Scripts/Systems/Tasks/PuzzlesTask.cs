using System;
using Interfaces;
using Entities.Puzzle;
using UnityEngine;
using Entities.Probs;

namespace Systems.Tasks
{
    public class PuzzlesTask : ITask
    {
        public bool Completed { get; set; }
        public Action OnComplete { get; set; }
        [SerializeField] private string _puzzleId;

        public void Activate()
        {
            Debug.Log("ira2  " + _puzzleId);
            LeversPuzzle.OnPuzzleSolved += MarkComplete;
        }

        private void MarkComplete(string id)
        {
            Debug.Log("ira1  " + id);
            if (_puzzleId != id) return;
            Completed = true;
            OnComplete?.Invoke();
            LeversPuzzle.OnPuzzleSolved -= MarkComplete;
        }
    }
}