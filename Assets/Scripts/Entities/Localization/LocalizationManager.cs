using System.Collections.Generic;
using System.IO;
using System.Text;
using Data;
using R3;
using TriInspector;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace Entities.Localization
{
    public class LocalizationManager : MonoBehaviour
    {
        private static Dictionary<string, string[]> _localizedText = new();
        private static string[] _headers;
        private static string SPREADSHEET_ID = "1PEswgSetu71j068EhzqhuEPMUwGHdLbFa2sXZ9EeWAg";
        public static readonly Dictionary<string, string> NameToGid = new ()
        {
            {"UI", "1087436388"},
            {"Player", "1556233291"},
            {"Tertilus", "2008482062"},
            {"Finix", "1520681221"},
            {"Octoboss", "324218113"},
            {"Skyper", "1314801844"},
            {"Jelica", "739200791"},
            {"Star", "317498044"},
            {"Fragments", "1106492096"},
            {"Quests", "2071454227"}
        };

        public static ReactiveProperty<Language> CurrentLanguage { get; } = new(Language.ru);

        private static GlobalData _globalData;
        
        [Inject]
        private void Construct(GlobalData globalData)
        {
            _globalData = globalData;
        }

        [Button("Pull Table")]
        private void PullTable() => LoadLocalizationTable();
        
        public static void LoadLocalizationTable()
        {
            List<string> loaded = new();
            foreach (var tableName in NameToGid.Keys)
            {
                var req = UnityWebRequest.Get(
                    $"https://docs.google.com/spreadsheets/d/{SPREADSHEET_ID}/export?format=tsv&gid={NameToGid[tableName]}");
                req.downloadHandler = new DownloadHandlerBuffer();
                var op = req.SendWebRequest();

                op.completed += _ =>
                {
                    if (loaded.Contains(tableName)) return;
                    loaded.Add(tableName);

                    var folder = Path.Combine(Application.streamingAssetsPath, "Localization");
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                    var path = Path.Combine(folder, tableName + ".tsv");

                    var data = req.downloadHandler.data;
                    File.WriteAllBytes(path, data);

#if UNITY_EDITOR
                    UnityEditor.AssetDatabase.Refresh();
#endif

                    Debug.Log("Loaded " + tableName + " -> " + path);
                };
            }

            SetValuesForTextsId();
        }
#if UNITY_EDITOR        
        public void OnValidate()
        {
            SetValuesForTextsId();
        }
#endif
        public void Start()
        {
            SetValuesForTextsId();
        }
        
        public static void SetValuesForTextsId()
        {
            foreach (var tableName in NameToGid.Keys)
            {
                var table = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "Localization", $"{tableName}.tsv"));
                
                string[] lines = table.Split('\n');
                _headers = lines[0].Split("\t");
                for (int i = 1; i < lines.Length; i++)
                {
                    string[] translations = lines[i].Split("\t");
                    string keyId = translations[0];
                    _localizedText[keyId] = translations;
                }
            }
        }
        
        public static string GetStringFromKey(string key)
        {
            if (_localizedText[key] == null) return "Given key is not in dictionary!";
            return ParseVars(_localizedText[key][((int)CurrentLanguage.Value)+1]);
        }
        
        private static string ParseVars(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            StringBuilder result = new StringBuilder(input.Length);
            StringBuilder key = new StringBuilder(32);
            bool inKey = false;
            
            foreach (char c in input)
            {
                if (c == '{')
                {
                    inKey = true;
                    key.Clear();
                    continue;
                }
                if (c == '}' && inKey)
                {
                    inKey = false;
                    
                    if (_globalData.Get<DialogueVarData>().Variables.Exists(
                            x => x.Name == key.ToString()))
                    {
                        result.Append(_globalData.Get<DialogueVarData>().Variables.Find(
                            x => x.Name == key.ToString()).Value);
                    }
                    else result.Append('{').Append(key).Append('}');
                    continue;
                }
                if (inKey) key.Append(c);
                else result.Append(c);

            }
            return result.ToString();
        }
    }
    
    public enum Language
    {
        ru, en
    }
}