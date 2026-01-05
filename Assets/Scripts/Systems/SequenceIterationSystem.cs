using System;
using System.Collections.Generic;
using Data;
using Data.Player;
using UnityEngine;

namespace Systems
{
    public class SequenceIterationSystem
    {
        public static void TryIterateSequence(List<SequenceElement> data, int currentState, Action<int> onStateChangedAction)
        {
            if (currentState >= data.Count) return;
            var element = data[currentState];
            if (element.Tasks.Count != 0 && !element.Tasks.TrueForAll(x => x.Completed)) 
                return;

            foreach (var x in element.Actions)
            {
                x.Invoke();
            }
            
            currentState++;
            onStateChangedAction.Invoke(currentState);
            Debug.Log($"currentState {currentState}");
            if (currentState == data.Count) return;
            Debug.Log($"Tasks.Count {data[currentState].Tasks.Count}");
            if (data[currentState].Tasks.Count > 0)
            {
                foreach (var task in data[currentState].Tasks)
                {
                    Debug.Log($"Activated task {task}");
                    task.Activate();
                    task.OnComplete += () => TryIterateSequence(data, currentState, onStateChangedAction);
                }
            }
            else
                TryIterateSequence(data, currentState, onStateChangedAction);
        }
    }
}