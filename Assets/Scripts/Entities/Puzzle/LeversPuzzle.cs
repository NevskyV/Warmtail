using System.Collections.Generic;
using Entities.Probs;
using Rng = UnityEngine.Random;
using UnityEngine.Events;
using UnityEngine;
using Interfaces;
using System;

namespace Entities.Puzzle
{
    public class LeversPuzzle : SavableStateObject, IPuzzle
    {
        [SerializeField] private GameObject _leverPref;
        [SerializeField] private Transform[] _leversPositions;
        [SerializeField] private int _leverCount;
        [SerializeField] private int _leverActive;

        public static Action<string> OnPuzzleSolved = delegate {};
        public UnityEvent OnSolved = new();

        void Awake()
        {
            Lever.OnTurnedon.AddListener(AddLevers);
            Lever.OnTurnedoff.AddListener(DecreaseLevers);
        }

        private void DecreaseLevers()
        {
            _leverActive--;
            if (_leverActive < 0) _leverActive = 0;
        }
        private void AddLevers()
        {
            _leverActive++;
            if (_leverActive == _leverCount) Solve();
        }

        public void Start()
        {
            List<Transform> leversPositions = new (_leversPositions);
            for (int i = 0; i < _leverCount; i ++)
            {
                int j = Rng.Range(0, leversPositions.Count);
                Instantiate (_leverPref, leversPositions[j].position, Quaternion.identity, transform);
                leversPositions.RemoveAt(j);
            }
        }
        public void Reset()
        {
        }
        public void Solve()
        {
            OnSolved.Invoke();
            OnPuzzleSolved?.Invoke(Id);
            Debug.Log("LevesPuzzle выполнено");
            Invoke("DestroyPuzzle", 0.5f);
        }

        private void DestroyPuzzle()
        {
            ChangeState(false);
        }
    }
}