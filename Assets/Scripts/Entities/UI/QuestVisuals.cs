using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using System.IO;
using Data;
using Data.Player;
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
        [SerializeField, Tooltip("x: horizontal\ny: top\nz:bottom")] private Vector3 _markOffset;
        [SerializeField] private Camera _cam;

        [SerializeField, Dropdown(nameof(GetDropdownStrings))]
        private string _correctLayerState;
        [SerializeField, Dropdown(nameof(GetDropdownStrings))]
        private string _incorrectLayerState;
        [Title("UI")]
        [SerializeField] private GameObject _markPrefab;
        [SerializeField] private RectTransform _markHud;
        [SerializeField] private GameObject _questPrefab;
        [SerializeField] private RectTransform _questHud;

        private SerializedDictionary<QuestData, List<MarkUIData>> _createdMarks;
        private SerializedDictionary<QuestData, GameObject> _createdQuests;

        public List<QuestData> AllQuests => _allQuests;

        private DiContainer _diContainer;
        private GlobalData _globalData;
        private SurfacingSystem _surfacingSystem;

        [Inject]
        private void Construct(DiContainer diContainer, GlobalData globalData, SurfacingSystem surfacingSystem)
        {
            _diContainer = diContainer;
            _globalData = globalData;
            _surfacingSystem = surfacingSystem;
            _createdMarks = new();
            _createdQuests = new();
            StickAction.OnStickTaked += StickQuest;
        }

        private void Start()
        {
            foreach (var id in _globalData.Get<SavablePlayerData>().QuestIds)
            {
                var quest = AllQuests.Find(x => x.Id == id.Key);
                if(quest) QuestSystem.StartQuest(quest, id.Value);
            }
        }

        public void SpawnQuest(QuestData data)
        {
            if (data == null) return;
            if (_createdQuests.ContainsKey(data) && _createdMarks.ContainsKey(data)) return;
            var newQuest = _diContainer.InstantiatePrefab(_questPrefab, _questHud).transform;
            if (!newQuest) return;
            newQuest.GetChild(0).GetComponent<LocalizedText>().SetNewKey("quest_header_" + data.Id);
            newQuest.GetChild(1).GetComponent<LocalizedText>().SetNewKey("quest_desc_" + data.Id);
            _createdQuests.Add(data,newQuest.gameObject);
            if (!_createdMarks.ContainsKey(data)) _createdMarks[data] = new();
            UpdateProgress(data, newQuest);
            newQuest.parent.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputVertical();
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
            if (questObj == null)
            {
                questObj = _createdQuests[data].transform;
            }
            if(_surfacingSystem.CurrentLayerIndex == data.Layer){
                var allQuestCount = data.Sequence.Count;
                var questState = _globalData.Get<SavablePlayerData>().QuestIds[data.Id].Count;
                questObj.GetChild(2).GetComponent<LocalizedText>().SetNewKey(_correctLayerState);
                questObj.GetChild(2).GetChild(0).GetComponent<TMP_Text>().text = $"{questState}/{allQuestCount}";
            }
            else
            {
                questObj.GetChild(2).GetComponent<LocalizedText>().SetNewKey(_correctLayerState);
                questObj.GetChild(2).GetChild(0).GetComponent<TMP_Text>().text = "";
            }
        }
        
        public void DestroyQuest(QuestData data)
        {
            if (_createdQuests.ContainsKey(data)) Destroy(_createdQuests[data]);
            if (_createdMarks.ContainsKey(data)) DestroyMarks(data);
            if (_createdMarks.ContainsKey(data)) _createdMarks.Remove(data);
            if (_createdQuests.ContainsKey(data)) _createdQuests.Remove(data);
        }
        
        public void SpawnMarks(QuestData data, Vector2 markPos)
        {
            var newMark = Instantiate(_markPrefab, _markHud);
            if (!_createdMarks.ContainsKey(data)) _createdMarks[data] = new();
            _createdMarks[data].Add(new MarkUIData
            {
                Object = newMark.gameObject,
                WorldPos = markPos
            });
        }

        public void DestroyMark(QuestData data, Vector2 markPos)
        {
            var mark = _createdMarks[data].Find(x => x.WorldPos == markPos);
            if (mark == null) return;
            Destroy(mark.Object);
            _createdMarks[data].Remove(mark);
        }

        private void DestroyMarks(QuestData data)
        {
            foreach (var mark in _createdMarks[data])
            {
                Destroy(mark.Object);
            }
        }
        
        public void Update()
        {
            if (_createdMarks == null) return;
            foreach (var mark in _createdMarks)
            {
                CalculateMarksPositions(mark.Value);
            }
        }
        
        private void CalculateMarksPositions(List<MarkUIData> marks)
        {
            for (var i = 0; i < marks.Count; i++)
            {
                var screenPos = _cam.WorldToScreenPoint(marks[i].WorldPos);

                Vector2 newScreenPos = new Vector2(screenPos.x, screenPos.y);
                if (screenPos.x > Screen.width - Screen.width * _markOffset.x)
                {
                    newScreenPos.x = Screen.width - Screen.width * _markOffset.x;
                }
                else if (screenPos.x < Screen.width * _markOffset.x)
                {
                    newScreenPos.x = Screen.width * _markOffset.x;
                }
                
                if (screenPos.y > Screen.height - Screen.height * _markOffset.y)
                {
                    newScreenPos.y = Screen.height - Screen.height * _markOffset.y;
                }
                else if (screenPos.y < Screen.height * _markOffset.z)
                {
                    newScreenPos.y = Screen.height * _markOffset.z;
                }
                Vector2 toTarget = (Vector2)screenPos - newScreenPos;
                if (toTarget.sqrMagnitude > 0.0001f)
                {
                    float angle = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;
                    marks[i].Object.transform.localRotation = Quaternion.Euler(0f, 0f, angle + 90);
                }
                marks[i].Object.transform.position = newScreenPos;
            }
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