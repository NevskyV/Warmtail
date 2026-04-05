using Data;
using Data.Player;
using Interfaces;
using System.Collections.Generic;
using Systems;
using UnityEngine;
namespace Entities.Props
{
    public class FearCore : SavableStateObject, IInteractable
    {
        [SerializeField] private bool _locked = true;
        [SerializeField] private FearPortal _portalPrefab;
        [SerializeField] private FearConfig _fearConfig;
        [SerializeField] private List<GameObject> _оbjectsToDisable = new();
        [SerializeField] private int _tailId = -1;
        [SerializeField] private List<GameObject> _оbjectsToEnable = new();
        
        private void Awake()
        {
            if (_portalPrefab != null)
                _portalPrefab.gameObject.SetActive(false);
        }

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
            ChangeState(false);
            
            if (_globalData != null)
            {
                var playerData = _globalData.Get<SavablePlayerData>();
                if (!playerData.FearIds.Contains(_fearConfig.Id))
                {
                    _globalData.Edit<SavablePlayerData>(data => data.FearIds.Add(_fearConfig.Id));
                }

                if (_tailId >= 0)
                {
                    _globalData.Edit<SavablePlayerData>(data => data.ActiveTailId = _tailId);
                }
            }
        }
    }
}