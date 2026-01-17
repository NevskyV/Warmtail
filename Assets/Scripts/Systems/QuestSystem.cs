using System.Linq;
using Data;
using Data.Player;
using Entities.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Zenject;

namespace Systems
{
    public class QuestSystem
    {
        private static GlobalData _globalData;
        private static QuestVisuals _questVisuals;
        
        public static UnityEvent<QuestData, bool> OnQuestStarted = new();
        public static UnityEvent<QuestData, bool> OnQuestEnded = new();

        [Inject]
        private void Construct(GlobalData globalData, QuestVisuals visuals)
        {
            _globalData = globalData;
            _questVisuals = visuals;
        }
        
        public static void StartQuest(QuestData data, int questState = 0)
        {
            if (data.Scene != SceneManager.GetActiveScene().path) return;

            if(!_globalData.Get<SavablePlayerData>().QuestIds.Keys.Contains(data.Id))
                _globalData.Edit<SavablePlayerData>(playerData => playerData.QuestIds.Add(data.Id, questState));

            OnQuestStarted.Invoke(data, true);
            _questVisuals.SpawnQuest(data);

            for (int i = 0; i < questState; i++)
            {
                data.Sequence[i].Actions.ForEach(x => x.Invoke());
            }

            foreach (var task in data.Sequence[questState].Tasks)
            {
                task.Activate();
                task.OnComplete += () => TryIterateSequence(data);
            }
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
                    else
                    {
                        _globalData.Edit<SavablePlayerData>(playerData => playerData.QuestIds[data.Id] = x);
                        _questVisuals.UpdateProgress(data);
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
            _questVisuals.DestroyQuest(data);
        }
    }
}