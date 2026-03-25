using UnityEngine;
using Data;
using Data.Player;
using Zenject;
using System;
using System.Collections.Generic;
using TriInspector;
using UnityEngine.Serialization;

namespace Systems.Tutorial
{
    [Serializable]
    [CreateAssetMenu(fileName = "EventConfig", menuName = "Configs/EventConfig")]
    public class EventConfig : ScriptableObject
    {
        [SerializeField, ReadOnly] private string _idNod = Guid.NewGuid().ToString();
        public string IdNode => _idNod;

        [field: SerializeReference, FormerlySerializedAs("Once")] public bool Once{ get; private set; }
        [field: SerializeReference] public bool AnyScene { get; private set; }
        [Scene, HideIf(nameof(AnyScene))] public string Scene;
        
        [field: SerializeReference, FormerlySerializedAs("NextElement")] public EventConfig NextElement{ get; private set; }
        public SequenceElement Element;
        
        [TextArea,SerializeField, FormerlySerializedAs("description")] private string _description;

        [Button("Copy")]
        private void Copy()
        {
            GUIUtility.systemCopyBuffer = _idNod;
        }

        public void TaskCompleted()
        {
            EventsSystem.OnEventCompleted?.Invoke(this);
        }
    }
}
