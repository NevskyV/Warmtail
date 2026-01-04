using System;
using Entities.PlayerScripts;
using Interfaces;
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
    [SerializeField] private float _interactionRadius = 2f;
    [SerializeField] private Vector3 _interactionOffset = Vector3.zero;
    
    private Player _player;
    private PlayerInput _playerInput;
    
    [Inject]
    public void Construct(Player player, PlayerInput playerInput)
    {
        _player = player;
        _playerInput = playerInput;
        _playerInput.actions["LeftMouse"].started += _ => StartAbility?.Invoke();
        _playerInput.actions["LeftMouse"].performed += Interact;
        _playerInput.actions["LeftMouse"].canceled += _ => EndAbility?.Invoke();
    }
    
    public void Interact(InputAction.CallbackContext context)
    {
        if (!Enabled) return;
        var colliders = Physics2D.OverlapCircleAll(_player.Rigidbody.transform.position + _interactionOffset, _interactionRadius);
        
        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent<IInteractable>(out var interactable))
            {
                interactable.Interact();
                UsingAbility?.Invoke();
                break;
            }
        }
    }
    
    public void Dispose()
    {
        _playerInput.actions["LeftMouse"].performed -= Interact;
    }
}
