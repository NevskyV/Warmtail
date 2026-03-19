using System;
using Interfaces;
using Systems;
using UnityEngine;
using Zenject;

namespace Entities.House
{
    public class StarBattery : MonoBehaviour, IInteractable
    {
        [Inject] private WarmthSystem _warmthSystem;
        
        public void Interact()
        {
            for (var i = 0; i < (_warmthSystem.MaxCells - _warmthSystem.CurrentCells); i++)
            {
                _warmthSystem.AddCell();
            }
        }
    }
}