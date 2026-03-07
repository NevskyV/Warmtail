using UnityEngine;
using Data;
using Data.Player;
using Zenject;
using System;
using System.Collections.Generic;
using TriInspector;

namespace Systems.Tutorial
{
    [Serializable]
    [CreateAssetMenu(fileName = "EventConfig", menuName = "Configs/EventConfig")]
    public class EventConfig : ScriptableObject
    {
        [SerializeField, ReadOnly] private string _idNod = Guid.NewGuid().ToString();
        public string IdNode => _idNod;

        public bool Once;
        public string Scene;
        public EventConfig NextElement;
        public SequenceElement Element;
        
        [TextArea] [SerializeField] public string description;

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
