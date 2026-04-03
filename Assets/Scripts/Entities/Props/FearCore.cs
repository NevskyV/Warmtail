using Data;
using Data.Player;
using Interfaces;
using System.Collections.Generic;
using Systems;
using UnityEngine;
using Zenject;

namespace Entities.Props
{
    public class FearCore : MonoBehaviour, IInteractable
    {
        [SerializeField] private bool _locked = true;
        [SerializeField] private FearPortal _portalPrefab;
        [SerializeField] private FearConfig _fearConfig;
        [SerializeField] private List<GameObject> _baseObjects = new();
        [SerializeField] private List<GameObject> _portalObjects = new();
        [SerializeField] private int _tailId = -1;
        
        [Inject] private GlobalData _globalData;

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
            if (_portalPrefab == null || _fearConfig == null) return;

            var portal = _portalPrefab;

            portal.gameObject.SetActive(true);
            portal.SetReverse(true);
            portal.Initialize(_baseObjects, _portalObjects);
            
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