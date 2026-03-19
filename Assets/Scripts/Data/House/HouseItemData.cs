using UnityEngine;
using Entities.House;

namespace Data.House
{
    [CreateAssetMenu(fileName = "House Item Data", menuName = "Configs/House Item Data")]
    public class HouseItemData : BuyableItemData
    {
        public DraggableObject ItemPref;
    }
}
