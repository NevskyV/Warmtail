using System;
using System.Collections.Generic;
using Data;
using Data.Player;
using Entities.UI;
using Interfaces;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Entities.Triggers
{
    public class CheckStarsTrigger : MonoBehaviour, IEventInvoker
    {

        [SerializeField] private RuntimeDialogueGraph _dialogueGraph;
        [Inject] private MonologueVisuals _monologueVisuals;
        [Inject] private GlobalData _globalData;
        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && 
                _globalData.Get<SavablePlayerData>().Stars == 1)
            {
                _monologueVisuals.StartMonologue(_dialogueGraph, this, true);
                gameObject.SetActive(false);
            }
        }

        [field : SerializeField]  public List<UnityEvent> Actions { get; set; }
    }
}