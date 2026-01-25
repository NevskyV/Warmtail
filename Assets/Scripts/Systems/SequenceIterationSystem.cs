using System;
using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Systems
{
    public class SequenceIterationSystem
    {
        public static void TryIterateSequence(List<SequenceElement> data, List<int> currentState, QuestType type, Action<int> onStateChangedAction)
        {
            if (currentState >= data.Count) return;
            var element = data[currentState];
            if (element.Tasks.Count != 0 && !element.Tasks.TrueForAll(x => x.Completed)) 
                return;

            foreach (var x in element.Actions)
            {
                x.Invoke();
            }
            
            if (type == QuestType.Parallel) currentState.Add();
            else if (type == QuestType.Serial) currentState[0] ++;

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