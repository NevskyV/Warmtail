using System.Linq;
using System.Collections.Generic;
using Data;
using Data.Player;
using Entities.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Zenject;

namespace Systems
{
    public class QuestSystem
    {
        private static GlobalData _globalData;
        
        public static UnityEvent<QuestData, bool> OnQuestStarted = new();
        public static UnityEvent<QuestData, bool> OnQuestUpdated = new();
        public static UnityEvent<QuestData, bool> OnQuestEnded = new();

        [Inject]
        private void Construct(GlobalData globalData)
        {
            _globalData = globalData;
        }
        
        public static void StartQuest(QuestData data, List<int> questState = null)
        {
            if (data.Scene != SceneManager.GetActiveScene().path) return;

            if (data.QuestType == QuestType.Serial && (questState == null || questState.Count == 0)) questState = new(){0};
            if (data.QuestType == QuestType.Parallel && questState == null) questState = new();

            if (!_globalData.Get<SavablePlayerData>().QuestIds.Keys.Contains(data.Id))
            {
                _globalData.Edit<SavablePlayerData>(playerData => playerData.QuestIds.Add(data.Id, questState));
                Debug.Log("add");
            }

            OnQuestStarted.Invoke(data, true);

            if (data.QuestType == QuestType.Serial)
            {
                foreach (var task in data.Sequence[questState[0]].Tasks)
                {
                    task.Activate();
                    task.OnComplete += () => TryIterateSequence(data, questState[0]);
                }

                for (int i = 0; i < questState[0]; i ++)
                {
                    data.Sequence[i].Actions.ForEach(x => x.Invoke());
                }

                TryIterateSequence(data, questState[0]);
            }
            else if (data.QuestType == QuestType.Parallel)
            {
                for (int i = 0; i < data.Sequence.Count; i ++)
                {
                    if (questState.Contains(i)) continue;
                    var tasks = data.Sequence[i].Tasks;
                    int stepIndex = i; 

                    foreach (var task in tasks)
                    {
                        task.Activate();
                        task.OnComplete += () => TryIterateSequence(data, stepIndex);
                    }
                }

                foreach (int i in questState)
                {
                    data.Sequence[i].Actions.ForEach(x => x.Invoke());
                    TryIterateSequence(data, i);
                }
            }

        }

        public static void TryIterateSequence(QuestData data, int task)
        {
            var questIds = _globalData.Get<SavablePlayerData>().QuestIds;
            if (!questIds.Keys.Contains(data.Id)) return;
            var questState = questIds[data.Id];
            if (questState.Count > data.Sequence.Count) {EndQuest(data);}
            else
            {
                SequenceIterationSystem.TryIterateSequence(data.Sequence, questState, task, data.QuestType,
                x =>
                {
                    if ((data.QuestType == QuestType.Parallel && x.Count == data.Sequence.Count) || 
                        (data.QuestType == QuestType.Serial && x[0] == data.Sequence.Count) ){
                        EndQuest(data);
                    }
                    else
                    {
                        Debug.Log(x[0]+" "+ data.Sequence.Count);
                        _globalData.Edit<SavablePlayerData>(playerData => playerData.QuestIds[data.Id] = x);
                        OnQuestUpdated?.Invoke(data, true);
                    }
                });
            }
        }
        
        public static void EndQuest(QuestData data)
        {
            OnQuestEnded.Invoke(data, false);
            data.OnComplete.ForEach(x => x.Invoke());
            if(_globalData.Get<SavablePlayerData>().QuestIds.Keys.Contains(data.Id))
                _globalData.Edit<SavablePlayerData>(playerData=> playerData.QuestIds.Remove(data.Id));
        }
    }
}