using Data;
using Interfaces;
using UnityEngine;
using Entities.Core;
using Entities.Probs;
using Entities.UI;

namespace Systems.SequenceActions
{
    public class NotificationAction : ISequenceAction
    {
        [SerializeField] private NotificationPopup _popup;
        [SerializeField] private string _systemId;
        
        public void Invoke()
        {
            SavableObjectsResolver.FindObjectById<PopupSystem>(_systemId).ShowPopup(_popup);
        }
    }
}