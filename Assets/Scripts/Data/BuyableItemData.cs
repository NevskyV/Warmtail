using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "BuyableItemData", menuName = "Configs/BuyableItemData")]
    public class BuyableItemData : ScriptableObject
    {
        public int Id;
        public Sprite Sprite;
        public int Price;
    }
}