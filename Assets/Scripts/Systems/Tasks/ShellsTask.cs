using System;
using Interfaces;
using UniRx;
using Data;
using UniRx.Triggers;
using UnityEngine;
using Entities.Probs;
using Object = UnityEngine.Object;

namespace Systems.Tasks
{
    public class ShellsTask : ITask
    {
        public bool Completed { get; set; }
        public Action OnComplete { get; set; }
        [SerializeField] private int _id;

        public void Activate()
        {
            Shell.OnShellsChanged += MarkComplete;
        }

        private void MarkComplete()
        {
            Completed = true;
            OnComplete?.Invoke();
            Shell.OnShellsChanged -= MarkComplete;
        }
    }
}