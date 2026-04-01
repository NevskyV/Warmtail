using Data;
using Data.Player;
using Interfaces;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Entities.Props
{
    public class FearCore : MonoBehaviour, IInteractable
    {
        [SerializeField] private bool _locked = true;
        [SerializeField] private FearConfig _fearConfig;
        [SerializeField] private List<GameObject> _оbjectsToDisable = new();
        [SerializeField] private List<GameObject> _оbjectsToEnable = new();
        
        [Inject] private GlobalData _globalData;
        
        public void Unlock()
        {
            _locked = false;
        }
        public void Interact()
        {
            if (_locked) return;
            if (_fearConfig == null) return;

            foreach (var obj in _оbjectsToDisable)
            {
                obj.SetActive(false);
            }
            
            foreach (var obj in _оbjectsToEnable)
            {
                obj.SetActive(true);
            }
            
            
            if (_globalData != null)
            {
                var playerData = _globalData.Get<SavablePlayerData>();
                if (!playerData.FearIds.Contains(_fearConfig.Id))
                {
                    _globalData.Edit<SavablePlayerData>(data => data.FearIds.Add(_fearConfig.Id));
                }
            }
        }
    }
}