using UnityEngine;
using Zenject;
using Entities.NPC;
using Data.House;
using Data.Player;
using Data.NPCShop;
using Data;

namespace Systems
{
    public class ShoppingSystem
    {
        [Inject] private GlobalData _globalData;
        [Inject] private ShoppingManager _shoppingManager;
        
        public void BuyItem(BuyableItemData item, Character character, bool isLast)
        {
            int shells = _globalData.Get<SavablePlayerData>().Shells;
            if (shells < item.Price) Debug.Log("не хватает ракушек!");
            else
            {
                _globalData.Edit<SavablePlayerData>(data => {data.Shells -= item.Price;});
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
            _shoppingManager.OpenNPCShop(character);
        }
    }
}
