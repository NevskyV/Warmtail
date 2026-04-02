using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Entities.NPC;
using Entities.PlayerScripts;
using Entities.Props;
using Interfaces;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Systems.Abilities
{
    public class InteractionAbility : IAbility, IDisposable
    {
        private static readonly int Enable = Shader.PropertyToID("_Enable");
        private static readonly int OutlineThickness = Shader.PropertyToID("_OutlineThickness");
        private static readonly int InnerOutlineThickness = Shader.PropertyToID("_InnerOutlineThickness");
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
        private GamepadRumble  _rumble;
        private AbilityTriggerZone<IInteractable> _triggerZone;
        private CompositeDisposable _disposables = new();
    
        [Inject]
        public void Construct(Player player, PlayerInput playerInput, GamepadRumble gamepadRumble)
        {
            Enabled = true;
            _player = player;
            _playerInput = playerInput;
            _rumble = gamepadRumble;
            
            _triggerZone = GetOrCreateTriggerZone<IInteractable>(player, "InteractionTrigger", _interactionRadius);
            
            _triggerZone.Wake();
            _triggerZone.OnObjectEnter.Subscribe(ObjectEnter);
            _triggerZone.OnObjectExit.Subscribe(ObjectExit);
            
            
            _playerInput.actions["Interact"].performed += Interact;
        }
    
        private AbilityTriggerZone<T> GetOrCreateTriggerZone<T>(Player player, string name, float radius) where T : class
        {
            var triggerObj = player.Rigidbody.transform.Find(name)?.gameObject;
            if (triggerObj == null)
            {
                triggerObj = new GameObject(name);
                triggerObj.transform.SetParent(player.Rigidbody.transform);
                triggerObj.transform.localPosition = Vector3.zero;
            }
            
            return new AbilityTriggerZone<T>(triggerObj, radius);
        }
    
        public void Interact(InputAction.CallbackContext context)
        {
            if (!Enabled) return;
            
            var objectsInRange = _triggerZone.ObjectsInRange;
            if (objectsInRange.Count == 0) return;
            _rumble.ShortRumble();
            IInteractable closest = null;
            float minDist = float.MaxValue;
            Vector2 playerPos = _player.Rigidbody.transform.position + _interactionOffset;
        
            foreach (var interactable in objectsInRange)
            {
                if (interactable is SpeakableCharacter speakable && speakable.Graph == null) return;
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
            _playerInput.actions["Interact"].performed -= Interact;
            _disposables?.Dispose();
        }

        private void ObjectEnter(IInteractable interactable)
        {
            if (interactable is SpeakableCharacter speakable && !speakable.Graph) return;
            
            var propertyBlock = new MaterialPropertyBlock();
            propertyBlock.SetInt(Enable, 1);
            var renderer = (interactable as MonoBehaviour)?.GetComponent<Renderer>();
            
            var outlineWidth = 0f;
            var inOutlineWidth = 0f;
            
            /*DOTween.To(() => outlineWidth, x =>{
                outlineWidth = x;
                propertyBlock.SetFloat(OutlineThickness,x);
                renderer?.SetPropertyBlock(propertyBlock);
            }, 0.005f, 0.5f);
            
            DOTween.To(() => inOutlineWidth, x =>{
                inOutlineWidth = x;
                propertyBlock.SetFloat(InnerOutlineThickness,x);
                renderer?.SetPropertyBlock(propertyBlock);
            }, 1.5f, 0.5f);
            Debug.Log("interactable: " + interactable);*/
            StartAbility?.Invoke();
        }
        
        private void ObjectExit(IInteractable interactable)
        {
            var propertyBlock = new MaterialPropertyBlock();
            propertyBlock.SetInt(Enable, 0);
            var renderer = (interactable as MonoBehaviour)?.GetComponent<Renderer>();
            
            var outlineWidth = 0.005f;
            var inOutlineWidth = 1.5f;
            
            //DOTween.To(() => outlineWidth, x =>{
            //     outlineWidth = x;
            //     propertyBlock.SetFloat(OutlineThickness,x);
            //     renderer?.SetPropertyBlock(propertyBlock);
            // }, 0f, 0.5f);
            //
            // DOTween.To(() => inOutlineWidth, x =>{
            //     inOutlineWidth = x;
            //     propertyBlock.SetFloat(InnerOutlineThickness,x);
            //     renderer?.SetPropertyBlock(propertyBlock);
            // }, 0f, 0.5f);

            if (_triggerZone.ObjectsInRange.Count == 0)
            {
                EndAbility?.Invoke();
            }
        }
    }
}
