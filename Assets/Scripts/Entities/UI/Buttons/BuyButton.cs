using Data;
using Data.NPCShop;
using Systems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Entities.UI
{
    public class BuyButton : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _priceTmp;
        [SerializeField] private string _currencyString;

        private ShopItem _shopItem;
        private Character _character;
        [Inject] private ShoppingSystem _shoppingSystem;
        [Inject] private GlobalData _globalData;

        public void Initialize(ShopItem shopItem, Character character, bool interactable)
        {
            _button.interactable = interactable;
            if (!interactable) _icon.color = new (0.5f, 0.5f, 0.5f, 1f);
            else _icon.color = new (1f, 1f, 1f, 1f);
            _shopItem = shopItem;
            _character = character;
            SetPreferencesButton();
        }
        private void SetPreferencesButton()
        {
            _icon.sprite = _shopItem.Item.Sprite;
            _priceTmp.text = _shopItem.Item.Price.ToString() + _currencyString;
        }
        public void ClickBuy()
        {
            _shoppingSystem.BuyItem(_shopItem.Item, _character, _shopItem.IsLast);
            SetPreferencesButton();
        }
}
}
