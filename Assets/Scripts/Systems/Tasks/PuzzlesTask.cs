using System;
using Interfaces;
using Data;
using Entities.Puzzle;
using UnityEngine;

namespace Systems.Tasks
{
    public class PuzzlesTask : ITask
    {
        public bool Completed { get; set; }
        public Action OnComplete { get; set; }
        [SerializeField] private string[] _puzzleId; 
        private EventsData _eventsData;

        public void SetEventsData(EventsData data)
        {
            _eventsData = data;
        }


        public void Activate()
        {
            DrawPuzzle.OnPuzzleSolved += MarkComplete;
            LeversPuzzle.OnPuzzleSolved += MarkComplete;
            RayPuzzle.OnPuzzleSolved += MarkComplete;
            GearsPuzzle.OnPuzzleSolved += MarkComplete;
        }

        private void MarkComplete(string id)
        {
            bool b = false;
            foreach (string s in _puzzleId)
                if (s == id) b = true;
            if (!b) return;
            
            Completed = true;
            OnComplete?.Invoke();
            DrawPuzzle.OnPuzzleSolved -= MarkComplete;
            LeversPuzzle.OnPuzzleSolved -= MarkComplete;
            RayPuzzle.OnPuzzleSolved -= MarkComplete;
            GearsPuzzle.OnPuzzleSolved -= MarkComplete;
        }
    }
}