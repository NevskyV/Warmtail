using Data;
using Data.Player;
using Interfaces;
using UnityEngine;
using Zenject;

namespace Entities.Props
{
    public class FearCore : MonoBehaviour, IInteractable
    {
        [SerializeField] private FearPortal _portalPrefab;
            [SerializeField] private FearConfig _fearConfig;
        [SerializeField] private Transform _spawnPoint;
        
        [Inject] private GlobalData _globalData;
        
        public void Interact()
        {
            if (_portalPrefab == null || _fearConfig == null) return;
            
            Vector3 spawnPosition = _spawnPoint != null ? _spawnPoint.position : transform.position;
            var portal = Instantiate(_portalPrefab, spawnPosition, Quaternion.identity);
            
            portal.gameObject.SetActive(true);
            portal.SetReverse(true);
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
