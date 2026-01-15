using System;
using System.Collections.Generic;
using Interfaces;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Systems.Abilities
{
    /// <summary>
    /// Базовый класс для триггер-зон. Содержит всю общую логику.
    /// Дочерние классы переопределяют только GetComponentFromCollider для указания типа.
    /// </summary>
    public abstract class AbilityTriggerZoneBase : MonoBehaviour
    {
        protected CircleCollider2D Collider;
        protected CompositeDisposable Disposables = new();
        
        protected virtual void Awake()
        {
            Collider = GetComponent<CircleCollider2D>();
            if (Collider == null)
            {
                Collider = gameObject.AddComponent<CircleCollider2D>();
            }
            Collider.isTrigger = true;
            
            this.OnTriggerEnter2DAsObservable()
                .Subscribe(OnTriggerEnterInternal)
                .AddTo(Disposables);
                
            this.OnTriggerExit2DAsObservable()
                .Subscribe(OnTriggerExitInternal)
                .AddTo(Disposables);
        }
        
        protected abstract void OnTriggerEnterInternal(Collider2D col);
        protected abstract void OnTriggerExitInternal(Collider2D col);
        
        public void SetRadius(float radius)
        {
            if (Collider != null)
            {
                Collider.radius = radius;
            }
        }
        
        public virtual void SetActive(bool active)
        {
            if (Collider != null)
            {
                Collider.enabled = active;
            }
        }
        
        protected virtual void OnDestroy()
        {
            Disposables?.Dispose();
        }
    }
    
    /// <summary>
    /// Триггер-зона для Warmable объектов.
    /// </summary>
    public class WarmableTriggerZone : AbilityTriggerZoneBase
    {
        private HashSet<Warmable> _objectsInRange = new();
        private Subject<Warmable> _onEnter = new();
        private Subject<Warmable> _onExit = new();
        
        public IObservable<Warmable> OnObjectEnter => _onEnter;
        public IObservable<Warmable> OnObjectExit => _onExit;
        public IReadOnlyCollection<Warmable> ObjectsInRange => _objectsInRange;
        
        protected override void OnTriggerEnterInternal(Collider2D col)
        {
            var obj = col.GetComponent<Warmable>();
            if (obj != null && _objectsInRange.Add(obj))
            {
                _onEnter.OnNext(obj);
            }
        }
        
        protected override void OnTriggerExitInternal(Collider2D col)
        {
            var obj = col.GetComponent<Warmable>();
            if (obj != null && _objectsInRange.Remove(obj))
            {
                _onExit.OnNext(obj);
            }
        }
        
        public override void SetActive(bool active)
        {
            base.SetActive(active);
            if (!active)
            {
                _objectsInRange.Clear();
            }
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            _onEnter?.Dispose();
            _onExit?.Dispose();
        }
    }
    
    /// <summary>
    /// Триггер-зона для IInteractable объектов.
    /// </summary>
    public class InteractableTriggerZone : AbilityTriggerZoneBase
    {
        private HashSet<IInteractable> _objectsInRange = new();
        private Subject<IInteractable> _onEnter = new();
        private Subject<IInteractable> _onExit = new();
        
        public IObservable<IInteractable> OnObjectEnter => _onEnter;
        public IObservable<IInteractable> OnObjectExit => _onExit;
        public IReadOnlyCollection<IInteractable> ObjectsInRange => _objectsInRange;
        
        protected override void OnTriggerEnterInternal(Collider2D col)
        {
            var obj = col.GetComponent<IInteractable>();
            if (obj != null && _objectsInRange.Add(obj))
            {
                _onEnter.OnNext(obj);
            }
        }
        
        protected override void OnTriggerExitInternal(Collider2D col)
        {
            var obj = col.GetComponent<IInteractable>();
            if (obj != null && _objectsInRange.Remove(obj))
            {
                _onExit.OnNext(obj);
            }
        }
        
        public override void SetActive(bool active)
        {
            base.SetActive(active);
            if (!active)
            {
                _objectsInRange.Clear();
            }
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            _onEnter?.Dispose();
            _onExit?.Dispose();
        }
    }
}
