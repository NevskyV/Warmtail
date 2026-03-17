using UnityEngine;
using Data.House;
using Zenject;
using Systems;

namespace Entities.House
{
    public class GiftTutor : MonoBehaviour
    {
        [SerializeField] private HouseItemData _item;
        [SerializeField] private Character _character;
        [Inject] private ShoppingSystem _shoppingSystem;

        public void AddToInventory()
        {
            _shoppingSystem.BuyItem(_item, _character, false, 0);
        }
    }
}
