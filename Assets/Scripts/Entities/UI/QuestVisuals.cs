using System;
using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Data;
using Data.Player;
using EasyTextEffects;
using Entities.Localization;
using Systems.SequenceActions;
using Systems.Environment;
using Systems;
using TMPro;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Entities.UI
{
    public class QuestVisuals : MonoBehaviour
    {
        [Title("Settings")]
        [SerializeField] private List<QuestData> _allQuests;
        [SerializeField] private Camera _cam;

        [SerializeField, Dropdown(nameof(GetDropdownStrings))]
        private string _correctLayerState;
        [SerializeField, Dropdown(nameof(GetDropdownStrings))]
        private string _incorrectLayerState;
        [Title("UI")]
        [SerializeField] private GameObject _questPrefab;
        [SerializeField] private RectTransform _questHud;
        [SerializeField, PreviewObject] private Sprite _mapMarkSprite;
        [SerializeField] private float _completeAnimationDuration = 1.5f;
        private SerializedDictionary<QuestData, List<MarksVisuals.Mark>> _createdMarks;
        private SerializedDictionary<QuestData, GameObject> _createdQuests;

        public List<QuestData> AllQuests => _allQuests;

        private DiContainer _diContainer;
        private GlobalData _globalData;
        private SurfacingSystem _surfacingSystem;
        private MarksVisuals _marksVisuals;

        [Inject]
        private void Construct(DiContainer diContainer, GlobalData globalData,
            SurfacingSystem surfacingSystem, MarksVisuals marksVisuals)
        {
            _diContainer = diContainer;
            _globalData = globalData;
            _surfacingSystem = surfacingSystem;
            _marksVisuals = marksVisuals;
            _createdMarks = new();
            _createdQuests = new();
            StickAction.OnStickTaked += StickQuest;
        }

        private void Start()
        {
            QuestSystem.OnQuestStarted.AddListener((data, b) => SpawnQuest(data));
            QuestSystem.OnQuestUpdated.AddListener((data, b) => UpdateProgress(data));
            QuestSystem.OnQuestEnded.AddListener((data, b) => DestroyQuest(data));
            var ids = _globalData.Get<SavablePlayerData>().QuestIds;
            foreach (var id in ids)
            {
                var quest = AllQuests.Find(x => x.Id == id.Key);
                if(quest) QuestSystem.StartQuest(quest, id.Value);
                print("QUEST ID" + id.Key);
            }
        }

        public async void SpawnQuest(QuestData data)
        {
            if (data == null) return;
            if (_createdQuests.ContainsKey(data) && _createdMarks.ContainsKey(data)) return;
            var newQuest = _diContainer.InstantiatePrefab(_questPrefab, _questHud).transform;
            if (!newQuest) return;
            await UniTask.Delay(200);
            
            newQuest.GetChild(0).GetComponent<LocalizedText>().SetNewKey("quest_header_" + data.Id);
            newQuest.GetChild(1).GetComponent<LocalizedText>().SetNewKey("quest_desc_" + data.Id);
            
            newQuest.GetChild(0).GetComponent<TextEffect>().Refresh();
            
            _createdQuests.Add(data,newQuest.gameObject);
            if (!_createdMarks.ContainsKey(data)) _createdMarks[data] = new();
            UpdateProgress(data, newQuest);
        }

        public void StartQuest(QuestData data)
        {
            if (_createdQuests.ContainsKey(data) && _createdMarks.ContainsKey(data)) return;
            QuestSystem.StartQuest(data);
        }
        public void EndQuest(QuestData data)
        {
            if (!_createdQuests.ContainsKey(data) || !_createdMarks.ContainsKey(data)) return;
            QuestSystem.EndQuest(data);
        }

        public void UpdateProgress(QuestData data, Transform questObj = null)
        {
            var questIds = _globalData.Get<SavablePlayerData>().QuestIds;
            if (!_createdQuests.ContainsKey(data)) return;
            if (questObj == null)
            {
                questObj = _createdQuests[data].transform;
            }
            if(_surfacingSystem.CurrentLayerIndex == data.Layer){
                var allQuestCount = data.Sequence.Count;
                var questState = 0;
                
                if (questIds.Keys.Count > 0)
                {
                    if (data.QuestType == QuestType.Serial) questState = questIds[data.Id][0];
                    if (data.QuestType == QuestType.Parallel) questState = questIds[data.Id].Count;
                }

                questObj.GetChild(2).GetComponent<LocalizedText>().SetNewKey(_correctLayerState);
                questObj.GetChild(2).GetChild(0).GetComponent<TMP_Text>().text = $"{questState}/{allQuestCount}";
            }
            else
            {
                questObj.GetChild(2).GetComponent<LocalizedText>().SetNewKey(_correctLayerState);
                questObj.GetChild(2).GetChild(0).GetComponent<TMP_Text>().text = "";
            }
        }
        
        public async void DestroyQuest(QuestData data)
        {
            if (_createdMarks.ContainsKey(data))
            {
                DestroyMarks(data);
                _createdMarks.Remove(data);
            }

            if (_createdQuests.ContainsKey(data))
            {
                _createdQuests[data].transform.GetChild(3).GetComponent<TMP_Text>().text = "+" + data.Reward;
                _globalData.Edit<SavablePlayerData>(player => { player.Shells += (int)data.Reward; });
                var animator = _createdQuests[data].GetComponent<Animator>();
                animator.SetTrigger("Complete");
                await UniTask.Delay(TimeSpan.FromSeconds(_completeAnimationDuration));

                Destroy(_createdQuests[data]);
                _createdQuests.Remove(data);
            }
        }
        
        public void SpawnMarks(QuestData data, Vector2 markPos)
        {
            if (!_createdMarks.ContainsKey(data)) _createdMarks[data] = new();
            var newMark = new MarksVisuals.Mark
            {
                Sprite = _mapMarkSprite,
                WorldPosition = markPos
            };
            _createdMarks[data].Add(newMark);
            _marksVisuals.SpawnMark(newMark, true);
        }

        public void DestroyMark(QuestData data, Vector2 markPos)
        {
            var mark = _createdMarks[data].Find(x => x.WorldPosition == markPos);
            if (mark == null) return;
            _marksVisuals.DestroyMark(mark);
            _createdMarks[data].Remove(mark);
        }

        private void DestroyMarks(QuestData data)
        {
            foreach (var mark in _createdMarks[data])
            {
                _marksVisuals.DestroyMark(mark);
            }
            _createdMarks[data].Clear();
        }
        
        private IEnumerable<TriDropdownItem<string>> GetDropdownStrings()
        {
            TriDropdownList<string> list = new();
            foreach (var tableName in LocalizationManager.NameToGid.Keys)
            {
                var table = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "Localization", $"{tableName}.tsv"));
                string[] lines = table.Split('\n');
                for (int i = 1; i < lines.Length; i++)
                {
                    var key = lines[i].Split("\t")[0];
                    list.Add(new TriDropdownItem<string>{ 
                        Text = $"{tableName}/{key}", Value = key});
                }
            }

            return list;
        }

        private void StickQuest()
        {
            _globalData.Edit<DialogueVarData>(data => {
                var a = data.Variables.Find(x => x.Name == "getStick");
                int pos = data.Variables.IndexOf(a);
                data.Variables[pos].Value = "true";
            });
            StickAction.OnStickTaked -= StickQuest;
        }
    }
}