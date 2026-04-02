using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data.Nodes
{
    [Serializable]
    public abstract record RuntimeNode
    {
        public string NodeId;
        [SerializeReference] public List<string> NextNodeIds = new();
        public abstract void Activate();
    }
}