using System.Collections.Generic;
using Data;
using Entities.Props;
using Entities.UI;
using Interfaces;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Entities.Triggers
{
    public class MonologueTrigger : SavableStateObject, IEventInvoker
    {
        [SerializeField] private RuntimeDialogueGraph _graph;
        [field : SerializeField] public List<UnityEvent> Actions { get; set; }
        [SerializeField] private bool _needToShow = true;
        [Inject] private MonologueVisuals _monologueVisuals;
        [Inject] private GlobalData _data;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _monologueVisuals.StartMonologue(_graph, this, _needToShow);
                ChangeState(false);
            }
        }
    }
}