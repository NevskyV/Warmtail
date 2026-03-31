using System.Collections.Generic;
using UnityEngine;
using System;
using Data.House;

namespace Data.NPCShop
{
    [CreateAssetMenu(fileName = "NPC Shop Data", menuName = "Configs/NPC Shop Data")]
    public class NPCInfoForShop : ScriptableObject
    {
        public Character Character;
        public int LevelCount;
        public List<ShopItem> ShopItemList;
    }
    [Serializable]
    public class ShopItem
    {
        public HouseItemData Item;
        public int NeedLevel;
        public bool IsLast;
    }
}
