using System.Collections.Generic;
using Rng = UnityEngine.Random;
using UnityEngine.Events;
using UnityEngine;
using Interfaces;
using System;
using Entities.Props;

namespace Entities.Puzzle
{
    public class GearsPuzzle : SavableStateObject, IPuzzle
    {
        public static UnityEvent OnReseted = new();
        public UnityEvent OnSolved = new();
        public static Action<string> OnPuzzleSolved = delegate {};
 
        [SerializeField] private GameObject _gearPref;
        [SerializeField] private Transform[] _levelsPositions;
        [SerializeField] private int _gearCount;
        [SerializeField] private string _eventsName;

        private int _activated;

        void Awake()
        {
            Gear.OnTwisted.AddListener(TwistGear);
        }

        public void TwistGear(int id)
        {
            _activated ++;
            if (id+1 != _activated) Invoke("Reset", 0.5f);
            else if (id+1 == _gearCount) Solve();
        }

        public void Start()
        {
            List<Transform> levelsPositions = new (_levelsPositions);
            for (int i = 0; i < _gearCount; i ++)
            {
                int j = Rng.Range(0, levelsPositions.Count);
                Instantiate (_gearPref, levelsPositions[j].position, Quaternion.identity, transform).GetComponent<Gear>().Initialize(i);
                levelsPositions.RemoveAt(j);
            }
        }

        public void Reset()
        {
            _activated = 0;
            OnReseted.Invoke();
        }

        public void Solve()
        {
            OnPuzzleSolved?.Invoke(_eventsName);
            OnSolved.Invoke();
            Debug.Log("GearsPuzzle выполнено");
            Invoke("DestroyPuzzle", 0.5f);
        }

        private void DestroyPuzzle()
        {
            ChangeState(false);
        }
    }
}