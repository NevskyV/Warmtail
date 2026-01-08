using System.Collections.Generic;
using UnityEngine;
using System;
using Zenject;
using Data;
using Data.House;
using Data.Player;
using Entities.House;

namespace Systems
{
    public class PlacementSystem
    {
        public Dictionary<int, int> InventoryCurrent;
        private List<PairForHouseItem> _houseItemsEditingInfo = new();
        public static Action OnApplyed = delegate {};
        public static Action OnApplyedAll = delegate {};
        public static Action OnCanceledAll = delegate {};
        public static Action OnUIDraggableUpdated = delegate {};
        public static Action OnResetedInventory = delegate {};
        private GlobalData _globalData;
        private HouseManager _houseManager;

        [Inject] private DiContainer _diContainer;
#region base
        [Inject]
        private void Construct(GlobalData globalData, HouseManager houseManager)
        {
            _houseManager = houseManager;
            _globalData = globalData;

            if (_globalData.Get<SavablePlayerData>().Inventory == null)
                _globalData.Edit<SavablePlayerData>(data => data.Inventory = new());
            ResetInventory();
            
            var houseItems = _globalData.Get<HouseData>().PlacedHouseItems;
            foreach (PairForHouseItem item in houseItems)
            {
                DraggableObject gm = InstantiateDraggableObject(_houseManager._itemsHouseIdConf.IdsForHouseItemsData[item.HouseItemDataId].ItemPref, new Vector2(item.PositionX, item.PositionY), true);
                gm.transform.position = new Vector2(item.PositionX, item.PositionY);
            }
        }
        public DraggableObject InstantiateDraggableObject(DraggableObject draggableObject, Vector2 pos, bool isItemConfirmed)
        {
            DraggableObject obj = _diContainer.InstantiatePrefab(draggableObject).GetComponent<DraggableObject>();
            if (pos.x != Vector2.positiveInfinity.x) obj.transform.position = pos;
            obj.Initialize(isItemConfirmed);
            return obj;
        }
#endregion
#region inventory
        public void AddItemToInventory(int itemId)
        {
            if (!InventoryCurrent.ContainsKey(itemId)) InventoryCurrent[itemId] = 0;
            InventoryCurrent[itemId] --;
            OnUIDraggableUpdated?.Invoke();
        }
        public void RemoveItemFromInventory(int itemId)
        {
            InventoryCurrent[itemId] ++;
            OnUIDraggableUpdated?.Invoke();
        }
        public void ApplyItemInventory(int itemId, int how)
        {
            _globalData.Edit<SavablePlayerData>(data => 
            {
                if (!data.Inventory.ContainsKey(itemId)) data.Inventory[itemId] = 0;
                data.Inventory[itemId] += how;
            });
            OnApplyed?.Invoke();
        }
        
        public void ResetInventory()
        {
            InventoryCurrent = new(_globalData.Get<SavablePlayerData>().Inventory);
            OnResetedInventory?.Invoke();
        }
#endregion
#region apply and cancel

        public void AddEditingItem(int idInArray, Vector2 currentPos)
        {
            _globalData.Edit<HouseData>(houseData =>
            {
                houseData.PlacedHouseItems.Add(new (idInArray, currentPos.x, currentPos.y));
            });
        }
        public void ReplacePositionEditingItem(int idInArray, Vector2 posOnConfirmedState, Vector2 currentPos)
        {
            _globalData.Edit<HouseData>(houseData =>
            {
                for (int ind = 0; ind < houseData.PlacedHouseItems.Count; ind++) {
                    if (houseData.PlacedHouseItems[ind].HouseItemDataId == idInArray && houseData.PlacedHouseItems[ind].PositionX == posOnConfirmedState.x && houseData.PlacedHouseItems[ind].PositionY == posOnConfirmedState.y) {
                        houseData.PlacedHouseItems[ind] = new PairForHouseItem(idInArray, currentPos.x, currentPos.y);
                        break;
                    }
                }
            });
        }
        public void RemoveEditingItem(int idInArray, Vector2 posOnConfirmedState)
        {
            _globalData.Edit<HouseData>(houseData =>
            {
                for (int ind = 0; ind < houseData.PlacedHouseItems.Count; ind++) {
                    if (houseData.PlacedHouseItems[ind].HouseItemDataId == idInArray && houseData.PlacedHouseItems[ind].PositionX == posOnConfirmedState.x && houseData.PlacedHouseItems[ind].PositionY == posOnConfirmedState.y) {
                        houseData.PlacedHouseItems.RemoveAt(ind);
                        break;
                    }
                }
            });
        }
        public void ApplyAllEditing()
        {
            OnApplyedAll?.Invoke();
        }
        public void CancelAll()
        {
            OnCanceledAll?.Invoke();
        }
#endregion
    }
}