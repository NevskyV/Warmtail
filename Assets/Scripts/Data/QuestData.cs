using System;
using System.Collections.Generic;
using Entities.Localization;
using Interfaces;
using TriInspector;
using UnityEngine;

namespace Data
{
    [Serializable]
    [CreateAssetMenu(fileName = "QuestData", menuName = "Configs/QuestData")]
    public class QuestData : ScriptableObject
    {
        [field: SerializeField,Title("Quest Id"),HideLabel, InfoBox("$" + nameof(HeaderPreview))] public int Id { get; private set; }
        public string HeaderPreview => LocalizationManager.GetStringFromKey("quest_header_" + Id);
        public QuestType QuestType;
        [field: SerializeField] public bool IsPermanent { get; private set; } = true;
        [field: SerializeField] public uint Reward { get; private set; } = 2;
        [field: Title("Scene Settings"), SerializeField, Scene] public string Scene { get; private set; }
        [field: SerializeField, Dropdown(nameof(_layers))] public int Layer { get; private set; }
        
        private int[] _layers = {0,1,2};
        [field: Title("Actions"), SerializeReference] 
        public List<ISequenceAction> OnStart { get; private set; }
        [field: SerializeReference] public List<ISequenceAction> OnComplete{ get; private set; }
        [field: SerializeReference] public List<ISequenceAction> OnFail { get; private set; }
        [field: Title("Sequence"),SerializeField] public List<SequenceElement> Sequence { get; private set; }
        
        
    }

    [Serializable]
    [DeclareHorizontalGroup("horizontal")]
    [DeclareBoxGroup("horizontal/one", Title = "Action")]
    [DeclareBoxGroup("horizontal/two", Title = "Conditions")]
    public struct SequenceElement
    {
        [SerializeReference, HideLabel, Group("horizontal/one")] public List<ISequenceAction> Actions;
        [SerializeReference, HideLabel, Group("horizontal/two")] public List<ITask> Tasks;
    }

    public enum QuestType { Serial, Parallel };
}