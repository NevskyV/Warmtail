using UnityEngine;
using Zenject;
using Entities.NPC;
using Data.House;
using Data.Player;
using Data.NPCShop;
using Data;
using Entities.House;

namespace Systems
{
    public class ShoppingSystem
    {
        [Inject] private GlobalData _globalData;
        [Inject] private SessionSystem _sessionSystem;
        
        public void BuyItem(BuyableItemData item, Character character, bool isLast)
        {
            int shells = _globalData.Get<SavablePlayerData>().Shells;
            if (shells < item.Price) Debug.Log("не хватает ракушек!");
            else
            {
                _globalData.Edit<SavablePlayerData>(data => {data.Shells -= item.Price;});
                _sessionSystem.AddItemBought();
                Debug.Log("ракушек теперь " + _globalData.Get<SavablePlayerData>().Shells);
                if (isLast) PurchaseLastItem (character);

                _globalData.Edit<SavablePlayerData>(data =>
                {
                    if (data.Inventory == null) data.Inventory = new();
                    if (!data.Inventory.ContainsKey(item.Id)) data.Inventory[item.Id] = 0;
                    data.Inventory[item.Id]++;
                });
            }
        }
        private void PurchaseLastItem(Character character)
        {
            _globalData.Edit<NPCData>(data => {data.BoughtLastItem[character] = true;});
        }
    }
}