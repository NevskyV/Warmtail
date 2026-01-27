using Data;
using Data.House;
using Data.Player;
using Interfaces;
using Systems;
using UnityEngine;
using Zenject;

namespace Entities.Props
{
    public class HomeItemCollectable : MonoBehaviour, IInteractable
    {
        [SerializeField] private HouseItemData _item;
        [SerializeField] private ParticleSystem _particlePrefab;
        [Inject] private GlobalData _globalData;
        
        public void Interact()
        {
            if(_particlePrefab) ObjectSpawnSystem.Spawn(_particlePrefab, transform.position);
            _globalData.Edit<SavablePlayerData>(data =>
            {
                if (data.Inventory == null) data.Inventory = new();
                if (!data.Inventory.ContainsKey(_item.Id)) data.Inventory[_item.Id] = 0;
                data.Inventory[_item.Id]++;
            });
            Destroy(gameObject);
        }
    }
}