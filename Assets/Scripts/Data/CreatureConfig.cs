using System;
using System.Collections.Generic;
using System.IO;
using Entities.Localization;
using TriInspector;
using UnityEngine;

namespace Data
{
    [Serializable]
    [CreateAssetMenu(fileName = "CreatureConfig", menuName = "Configs/CreatureConfig")]
    public class CreatureConfig : ScriptableObject
    {
        [SerializeField] private int _id;
        [SerializeField, Dropdown(nameof(GetDropdownStrings)), InfoBox("$" + nameof(NamePreview))] public string _name;
        public string NamePreview => LocalizationManager.GetStringFromKey(_name);
        [SerializeField, Dropdown(nameof(GetDropdownStrings)), InfoBox("$" + nameof(DescriptionPreview))] private string _description;
        public string DescriptionPreview => LocalizationManager.GetStringFromKey(_description);
        [SerializeField, PreviewObject] private Sprite _icon;
        
        public Sprite Icon => _icon;
        public string Name => _name;
        public string Description => _description;
        
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
    }
}