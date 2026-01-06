using UnityEngine;
using Interfaces;
using System;
using Data;
using Entities.Core;
using Entities.Probs;
using Systems;

namespace Systems.Tasks
{
    public class DecorateTask : ITask
    {
        public bool Completed { get; set; }
        public Action OnComplete { get; set; }

        public void Activate()
        {
            PlacementSystem.OnApplyed += MarkComplete;
            Debug.Log("Iraaa1");
        }

        private void MarkComplete()
        {
            Debug.Log("Iraaa2");
            Completed = true;
            OnComplete?.Invoke();
            PlacementSystem.OnApplyed -= MarkComplete;
        }
    }
}