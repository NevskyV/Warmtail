using System.Collections.Generic;
using Data.Nodes;
using TriInspector;
using UnityEngine;

namespace Data
{
    public class RuntimeDialogueGraph : ScriptableObject
    {
        [field: SerializeField] public string EntryNodeId {get; set;}
        [field: SerializeReference] public List<RuntimeNode> AllNodes { get; set; }
        [field: SerializeField] public int DialogueId { get; set; }
    }
}