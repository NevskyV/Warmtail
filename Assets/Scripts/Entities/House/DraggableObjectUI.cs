using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Zenject;
using System;
using Systems;
using Data.House;
using Data;

namespace Entities.House
{
    public class DraggableObjectUI : MonoBehaviour
    {
        [SerializeField] private HouseItemData _houseItemData;
        [SerializeField] private TMP_Text countTMP;
        [SerializeField] private GameObject _block;
        [SerializeField] private string _unitOfMasure;
        private DraggableObject _itemCopyingObject;
        private Vector2 _startClickPosition;
        
        [Inject] private PlacementSystem _placementSystem;
        [Inject] private GlobalData _globalData;

        void Awake()
        {
            PlacementSystem.OnUIDraggableUpdated += SetPreferencesUI;
            PlacementSystem.OnResetedInventory += SetPreferencesUI;
        }
        void OnDestroy()
        {
            PlacementSystem.OnUIDraggableUpdated -= SetPreferencesUI;
            PlacementSystem.OnResetedInventory -= SetPreferencesUI;
        }

        private void OnEnable()
        {
            SetPreferencesUI();
        }
        private void SetPreferencesUI()
        {
            int count = 0;
            if (_placementSystem.InventoryCurrent.ContainsKey(_houseItemData.Id))
                count = _placementSystem.InventoryCurrent[_houseItemData.Id];
            countTMP.text = count.ToString() + _unitOfMasure;
            _block.SetActive(count <= 0);
        }

        public void PointerDownItem()
        {
            _itemCopyingObject = null;
            _startClickPosition = Mouse.current.position.ReadValue();
        }
        public void HoldItem()
        {
            if (Math.Abs(_startClickPosition.y - Mouse.current.position.ReadValue().y) < 50) 
            {
                if (_itemCopyingObject)
                {
                    _itemCopyingObject.RemoveFromInventory();
                    Destroy(_itemCopyingObject.gameObject);
                    SetPreferencesUI();
                }
            }
            else
            {
                if (!_itemCopyingObject)
                {
                    _itemCopyingObject = _placementSystem.InstantiateDraggableObject(_houseItemData.ItemPref, Vector2.positiveInfinity, false);
                    _itemCopyingObject.AddToInventory();
                    SetPreferencesUI();
                }
                Vector2 pos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                _itemCopyingObject.transform.position = pos;
            }
        }
    }
}
