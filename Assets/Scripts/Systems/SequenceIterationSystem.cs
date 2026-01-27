using System;
using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Systems
{
    public class SequenceIterationSystem
    {
        public static void TryIterateSequence(List<SequenceElement> data, int taskNum, QuestType type, Action<List<int>> onStateChangedAction)
        {
            TryIterateSequence(data, new List<int>(1){taskNum}, 0, type, onStateChangedAction);
        }
        public static void TryIterateSequence(List<SequenceElement> data, List<int> currentState, int taskNum, QuestType type, Action<List<int>> onStateChangedAction)
        {
            if ((type == QuestType.Serial && currentState[0] >= data.Count) ||
                (type == QuestType.Parallel && currentState.Count >= data.Count)) return;

            var element = data[currentState[0]];
            if (type == QuestType.Parallel) element = data[taskNum];

            if (element.Tasks.Count != 0 && !element.Tasks.TrueForAll(x => x.Completed)) 
                return;

            foreach (var x in element.Actions)
            {
                x.Invoke();
            }

            if (type == QuestType.Serial) currentState[0] ++;
            if (type == QuestType.Parallel) currentState.Add(taskNum);

            onStateChangedAction.Invoke(currentState);

            Debug.Log($"currentState {currentState[taskNum]}");
            if ((type == QuestType.Serial && currentState[0] >= data.Count) ||
                (type == QuestType.Parallel && currentState.Count >= data.Count)) return;
            Debug.Log($"Tasks.Count {data[currentState[taskNum]].Tasks.Count}");

            if (type == QuestType.Serial)
            {
                if (data[currentState[0]].Tasks.Count > 0)
                {
                    foreach (var task in data[currentState[0]].Tasks)
                    {
                        Debug.Log($"Activated task {task}");
                        task.Activate();
                        task.OnComplete += () => TryIterateSequence(data, currentState, 0, type, onStateChangedAction);
                    }
                }
                else
                    TryIterateSequence(data, currentState, 0, type, onStateChangedAction);
            }
        }
    }
}