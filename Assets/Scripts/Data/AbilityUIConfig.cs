using System.Collections.Generic;
using System.IO;
using Entities.Localization;
using TriInspector;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "AbilityUIConfig", menuName = "Configs/AbilityUIConfig")]
    public class AbilityUIConfig : ScriptableObject
    {
        [field: SerializeReference] public AbilityType Type { get; private set; }
        [field: SerializeReference,  Dropdown(nameof(GetDropdownStrings))] public string Name { get; private set; }
        [field: SerializeReference, Dropdown(nameof(GetDropdownStrings))] public string Description { get; private set; }
        [field: SerializeReference, PreviewObject] public Sprite Sprite { get; private set; }
        
        
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