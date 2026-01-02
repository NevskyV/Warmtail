using System.Collections.Generic;
using System.Linq;
using System;
using Data;
using Data.Player;
using Entities.UI;
using Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Systems
{
    public class QuestSystem
    {
        private static GlobalData _globalData;
        private static QuestVisuals _questVisuals;
        
        public static Action<QuestData> OnQuestStarted = delegate {};
        public static Action<QuestData> OnQuestEnded = delegate {};

        [Inject]
        private void Construct(GlobalData globalData, QuestVisuals visuals)
        {
            _globalData = globalData;
            _questVisuals = visuals;
            foreach (var id in _globalData.Get<SavablePlayerData>().QuestIds)
            {
                var quest = visuals.AllQuests.Find(x => x.Id == id.Key);
                if(quest) StartQuest(quest, id.Value);
            }
        }
        
        public static void StartQuest(QuestData data, int questState = 0)
        {
            if(!_globalData.Get<SavablePlayerData>().QuestIds.Keys.Contains(data.Id))
                _globalData.Edit<SavablePlayerData>(playerData => playerData.QuestIds.Add(data.Id, questState));

            if (data.Scene != SceneManager.GetActiveScene().path) return;

            OnQuestStarted?.Invoke(data);

            _questVisuals.SpawnQuest(data);
            for (int i = 0; i < questState; i++)
            {
                data.Sequence[i].Actions.ForEach(x => x.Invoke());
            }

            foreach (var task in data.Sequence[questState].Tasks)
            {
                task.OnComplete += () => TryIterateSequence(data);
            }

            TryIterateSequence(data);
        }

        public static void TryIterateSequence(QuestData data)
        {
            var questIds = _globalData.Get<SavablePlayerData>().QuestIds;
            if (!questIds.Keys.Contains(data.Id)) return;
            var questState = questIds[data.Id];
            if (questState >= data.Sequence.Count) EndQuest(data);
            else
            {
                SequenceIterationSystem.TryIterateSequence(data.Sequence, questState,
                x =>
                {
                    if (x == data.Sequence.Count) EndQuest(data);
                    else _globalData.Edit<SavablePlayerData>(playerData => playerData.QuestIds[data.Id] = x);
                });
            }
        }
        
        public static void EndQuest(QuestData data)
        {
            OnQuestEnded?.Invoke(data);
            data.OnComplete.ForEach(x => x.Invoke());
            if(_globalData.Get<SavablePlayerData>().QuestIds.Keys.Contains(data.Id))
                _globalData.Edit<SavablePlayerData>(playerData=> playerData.QuestIds.Remove(data.Id));
            _questVisuals.DestroyQuest(data);
        }
    }
}