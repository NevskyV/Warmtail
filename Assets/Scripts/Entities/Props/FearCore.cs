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
        [SerializeField] private FearPortal _portalPrefab;
        [SerializeField] private FearConfig _fearConfig;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private List<GameObject> _baseObjects = new();
        [SerializeField] private List<GameObject> _portalObjects = new();
        
        [Inject] private GlobalData _globalData;
        
        public void Interact()
        {
            if (_portalPrefab == null || _fearConfig == null) return;
            
            Vector3 spawnPosition = _spawnPoint != null ? _spawnPoint.position : transform.position;
            var portal = Instantiate(_portalPrefab, spawnPosition, Quaternion.identity);
            
            portal.gameObject.SetActive(true);
            portal.SetReverse(true);
            portal.Initialize(_baseObjects, _portalObjects);
            portal.Activate();
            
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
