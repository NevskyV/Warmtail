using System;
using Entities.Props;
using Interfaces;
using UnityEngine;

namespace Systems.Tasks
{
    public class ShellsTask : ITask
    {
        public bool Completed { get; set; }
        public Action OnComplete { get; set; }

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