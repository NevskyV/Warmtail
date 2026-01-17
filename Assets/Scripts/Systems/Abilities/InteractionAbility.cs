using System;
using Entities.PlayerScripts;
using Interfaces;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Systems.Abilities
{
    public class InteractionAbility : IAbility, IDisposable
    {
        public bool Enabled { get; set; } = true;
        public Action StartAbility { get; set; }
        public Action UsingAbility { get; set; }
        public Action EndAbility { get; set; }
        [field: SerializeReference] public IAbilityVisual Visual { get; set; }
        public Sprite Icon => null;
        [SerializeField] private float _interactionRadius = 2f;
        [SerializeField] private Vector3 _interactionOffset = Vector3.zero;
    
        private Player _player;
        private PlayerInput _playerInput;
        private InteractableTriggerZone _triggerZone;
        private CompositeDisposable _disposables = new();
    
        [Inject]
        public void Construct(Player player, PlayerInput playerInput)
        {
            Enabled = true;
            _player = player;
            _playerInput = playerInput;
        
            _triggerZone = GetOrCreateTriggerZone(player, "InteractionTrigger", _interactionRadius);
            _triggerZone.SetActive(true);
        
            _playerInput.actions["LeftMouse"].started += _ => StartAbility?.Invoke();
            _playerInput.actions["LeftMouse"].performed += Interact;
            _playerInput.actions["LeftMouse"].canceled += _ => EndAbility?.Invoke();
        }
    
        private InteractableTriggerZone GetOrCreateTriggerZone(Player player, string name, float radius)
        {
            var triggerObj = player.Rigidbody.transform.Find(name)?.gameObject;
            if (triggerObj == null)
            {
                triggerObj = new GameObject(name);
                triggerObj.transform.SetParent(player.Rigidbody.transform);
                triggerObj.transform.localPosition = Vector3.zero;
                triggerObj.AddComponent<CircleCollider2D>();
            }
        
            var zone = triggerObj.GetComponent<InteractableTriggerZone>();
            if (zone == null)
                zone = triggerObj.AddComponent<InteractableTriggerZone>();
        
            zone.SetRadius(radius);
            return zone;
        }
    
        public void Interact(InputAction.CallbackContext context)
        {
            if (!Enabled) return;
        
            var objectsInRange = _triggerZone.ObjectsInRange;
            if (objectsInRange.Count == 0) return;
        
            IInteractable closest = null;
            float minDist = float.MaxValue;
            Vector2 playerPos = _player.Rigidbody.transform.position + _interactionOffset;
        
            foreach (var interactable in objectsInRange)
            {
                if (interactable is MonoBehaviour mb)
                {
                    float dist = Vector2.Distance(playerPos, mb.transform.position);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        closest = interactable;
                    }
                }
            }
        
            if (closest != null)
            {
                closest.Interact();
                UsingAbility?.Invoke();
            }
        }
    
        public void Dispose()
        {
            _playerInput.actions["LeftMouse"].performed -= Interact;
            _disposables?.Dispose();
        }
    }
}
