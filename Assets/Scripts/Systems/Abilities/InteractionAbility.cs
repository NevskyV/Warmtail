using System;
using Entities.PlayerScripts;
using Interfaces;
using Systems.Abilities;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class InteractionAbility : IAbility, IDisposable
{
    public bool Enabled { get; set; } = true;
    public Action StartAbility { get; set; }
    public Action UsingAbility { get; set; }
    public Action EndAbility { get; set; }
    [field: SerializeReference] public IAbilityVisual Visual { get; set; }
    public Sprite Icon => null; // InteractionAbility не отображается в UI
    [SerializeField] private float _interactionRadius = 2f;
    [SerializeField] private Vector3 _interactionOffset = Vector3.zero;
    
    private Player _player;
    private PlayerInput _playerInput;
    private AbilityTriggerZone<IInteractable> _triggerZone;
    private CompositeDisposable _disposables = new();
    
    [Inject]
    public void Construct(Player player, PlayerInput playerInput)
    {
        Enabled = true;
        _player = player;
        _playerInput = playerInput;
        
        // Создать триггер-зону
        _triggerZone = GetOrCreateTriggerZone(player, "InteractionTrigger", _interactionRadius);
        if (_triggerZone != null)
        {
            _triggerZone.SetActive(true); // Всегда активна
        }
        else
        {
            Debug.LogError("InteractionAbility: Trigger zone is null!");
        }
        
        _playerInput.actions["LeftMouse"].started += _ => StartAbility?.Invoke();
        _playerInput.actions["LeftMouse"].performed += Interact;
        _playerInput.actions["LeftMouse"].canceled += _ => EndAbility?.Invoke();
    }
    
    private AbilityTriggerZone<IInteractable> GetOrCreateTriggerZone(Player player, string name, float radius)
    {
        if (player == null || player.transform == null)
        {
            Debug.LogError($"InteractionAbility: Player or Player.transform is null!");
            return null;
        }
        
        var triggerObj = player.transform.Find(name)?.gameObject;
        if (triggerObj == null)
        {
            triggerObj = new GameObject(name);
            triggerObj.transform.SetParent(player.transform);
            triggerObj.transform.localPosition = Vector3.zero;
            triggerObj.AddComponent<CircleCollider2D>();
        }
        
        var zone = triggerObj.GetComponent<AbilityTriggerZone<IInteractable>>();
        if (zone == null)
            zone = triggerObj.AddComponent<AbilityTriggerZone<IInteractable>>();
        
        if (zone != null)
        {
            zone.SetRadius(radius);
        }
        else
        {
            Debug.LogError($"InteractionAbility: Failed to create trigger zone!");
        }
        
        return zone;
    }
    
    public void Interact(InputAction.CallbackContext context)
    {
        if (!Enabled || _triggerZone == null) return;
        
        var objectsInRange = _triggerZone.ObjectsInRange;
        if (objectsInRange == null || objectsInRange.Count == 0) return;
        
        // Взять ближайший объект
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
