using System.Collections.Generic;
using Data.Nodes;
using TriInspector;
using UnityEngine;

namespace Data
{
    public class RuntimeDialogueGraph : ScriptableObject
    {
        [field: SerializeField, ReadOnly] public string EntryNodeId {get; set;}
        [field: SerializeReference, ReadOnly] public List<RuntimeNode> AllNodes { get; set; }
        [field: SerializeField, ReadOnly] public int DialogueId { get; set; }
    }
}