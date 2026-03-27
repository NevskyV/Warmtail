using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Data.House;
using Zenject;
using Systems;
using Entities.UI;

namespace Entities.House
{
    public class DraggableObject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
#region base
        [SerializeField] private GameObject _buttonsForConfirmed;
        [SerializeField] private GameObject _buttonsForEditing;
        [SerializeField] private GameObject _child;
        [SerializeField] private HouseItemData _houseItemData;
        private PlacementSystem _placementSystem;
        private InputAction _leftClickAction;
        private BoxCollider2D _boxCollider;
        private UIStateSystem _uiStateSystem;
        private Vector2 _posObjectOnPointerDown;
        private Vector2 _posMouseOnPointerDown;
        private Vector2 _posObjectOnLastInside;
        private Vector2 _posObjectOnConfirmedState;
        private bool _isClickedNow = true;
        private bool _isPlacementing;
        private bool _isMenuEnabled;
        private bool _isConfirmed = true;
        private bool _isVisible = true;

        void Awake()
        {
            PlacementSystem.OnApplyedAll += ApplyEditing;
            PlacementSystem.OnCanceledAll += CancelEdited;
            
            _boxCollider = GetComponent<BoxCollider2D>();
        }
        void OnDestroy()
        {
            PlacementSystem.OnApplyedAll -= ApplyEditing;
            PlacementSystem.OnCanceledAll -= CancelEdited;
        }

        [Inject]
        private void Construct(PlacementSystem placementSystem, PlayerInput input, UIStateSystem uiStateSystem)
        {
            _leftClickAction = input.actions.FindAction("Building");
            _placementSystem = placementSystem;
            _uiStateSystem = uiStateSystem;
        }
        public void Initialize(bool isConfirmed)
        {
            _posObjectOnConfirmedState = (isConfirmed ? transform.position : Vector2.positiveInfinity);
            _isConfirmed = isConfirmed;
            _isPlacementing = !_isConfirmed;
        }
        
        private void StartBuild()
        {
            if (_uiStateSystem.CurrentState != UIState.Building)
            {
                _placementSystem.ResetInventory();
                _uiStateSystem.SwitchCurrentStateAsync(UIState.Building);
            }
        }
#endregion
#region interaction
        private void Update()
        {
            if (_leftClickAction.WasReleasedThisFrame())
            {
                if (!_isClickedNow) DisableMenu();
                else HandlePointerUp();
                _isClickedNow = false;
            }

            Vector2 v2 = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            if (_isPlacementing && Vector2.Distance(_posMouseOnPointerDown, v2) > 0.3f)
            {
                if (v2.x > 35.06f && v2.y > -20.48f) 
                {
                    if (_posObjectOnLastInside.x > 35.06f) v2.y = -20.48f;
                    else v2.x = 35.06f;
                }

                if (v2.x < 9.54f) v2.x = 9.54f;
                if (v2.y < -31.14f) v2.y = -31.14f;
                if (v2.y > 4.86f) v2.y = 4.86f;
                if (v2.x > 45.26f) v2.x = 45.26f;

                _posObjectOnLastInside = v2;

                transform.position = _posObjectOnPointerDown + (v2 - _posMouseOnPointerDown);
                StartBuild();
            }
        }
        
        public void OnPointerDown(PointerEventData pointerEventData)
        {
            _isPlacementing = true;
            _posMouseOnPointerDown = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            _posObjectOnPointerDown = transform.position;
            _isClickedNow = true;
            Debug.Log("ira draggable disable 1");
            DisableMenu();
        }
        public void OnPointerUp(PointerEventData pointerEventData)
        {
            Debug.Log("ira draggable 1");
            HandlePointerUp();
        }
        private void HandlePointerUp()
        {
            Vector2 pos = transform.position;
            if (_posObjectOnConfirmedState != pos) {
                if (_isConfirmed) DisableMenu();
                _isConfirmed = false;
            }
            DisableMenu();
            EnableMenu();
            Debug.Log("ira draggable 2");
            _isPlacementing = false;
        }
        private void DisableMenu()
        {
            Debug.Log("ira draggable disable 2");
            if (!_isMenuEnabled) return;
            Debug.Log("ira draggable 3");
            _isMenuEnabled = false;
            _buttonsForConfirmed.SetActive(false);
            _buttonsForEditing.SetActive(false);
        }
        private void EnableMenu()
        {
            Debug.Log("ira draggable 3");
            if (_isMenuEnabled) return;
            Debug.Log("ira draggable 4");
            _isMenuEnabled = true;
            if (_isConfirmed) _buttonsForConfirmed.SetActive(true);
            else _buttonsForEditing.SetActive(true);
        }
#endregion
#region buttons methods

        public void ApplyEditing()
        {
            if (_isConfirmed) return;
            if (!_isVisible) 
            {
                ApplyInventory(+1);
                _placementSystem.RemoveEditingItem(_houseItemData.Id, _posObjectOnConfirmedState);
                Destroy(gameObject);
            }
            else
            {
                if (_posObjectOnConfirmedState.x == Vector2.positiveInfinity.x)
                {
                    ApplyInventory(-1);
                    _placementSystem.AddEditingItem(_houseItemData.Id, transform.position);
                }
                else _placementSystem.ReplacePositionEditingItem(_houseItemData.Id, _posObjectOnConfirmedState, transform.position);
                _posObjectOnConfirmedState = transform.position;
                _isConfirmed = true;
            }
        }
        public void CancelEdited()
        {
            if (_isConfirmed) return;
            if (_posObjectOnConfirmedState.x == Vector2.positiveInfinity.x) 
            {
                RemoveFromInventory();
                Destroy(gameObject);
            }
            else {
                if (!_isVisible) AddToInventory();
                _isConfirmed = true;
                transform.position = _posObjectOnConfirmedState;
                _child.SetActive(true);
                _boxCollider.enabled = true;
                _isVisible = true;
            }
        }
        public void RemoveObject()
        {
            if (_placementSystem.InventoryCurrent[_houseItemData.Id] <= 0)
            {
                StartBuild();
                _placementSystem.RemoveEditingItem(_houseItemData.Id, _posObjectOnConfirmedState);
                RemoveFromInventory();
                ApplyInventory(+1);
                Destroy(gameObject);
            }
            else
            {
                _isConfirmed = false;
                _child.SetActive(false);
                _boxCollider.enabled = false;
                _isVisible = false;
                StartBuild();
                RemoveFromInventory();
            }
        }

        private void ApplyInventory(int how) => _placementSystem.ApplyItemInventory(_houseItemData.Id, how);
        public void AddToInventory() => _placementSystem.AddItemToInventory(_houseItemData.Id);
        public void RemoveFromInventory() => _placementSystem.RemoveItemFromInventory(_houseItemData.Id);
    }
#endregion
}
