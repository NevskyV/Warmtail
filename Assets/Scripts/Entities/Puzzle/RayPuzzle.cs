using Interfaces;
using UnityEngine.Events;
using UnityEngine;
using System;
using Entities.Props;

namespace Entities.Puzzle
{
    public class RayPuzzle : SavableStateObject, IPuzzle
    {
        public static Action<string> OnPuzzleSolved = delegate {};
        public UnityEvent OnSolved = new();

        [SerializeField] private string _eventsName;

        public void Start()
        {
        }
        public void Reset()
        {
        }
        public void Solve()
        {
            OnPuzzleSolved?.Invoke(_eventsName);
            OnSolved.Invoke();
            Debug.Log("RayPuzzle выполнено");
            Invoke("DestroyPuzzle", 0.5f);
        }

        private void DestroyPuzzle()
        {
            ChangeState(false);
        }
    }
}
