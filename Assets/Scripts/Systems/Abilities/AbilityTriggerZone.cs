using System;
using System.Collections.Generic;
using Interfaces;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Systems.Abilities
{
    /// <summary>
    /// Триггер-зона для Warmable объектов.
    /// </summary>
    public class WarmableTriggerZone : MonoBehaviour
    {
        private CircleCollider2D _collider;
        private HashSet<Warmable> _objectsInRange = new();
        private Subject<Warmable> _onEnter = new();
        private Subject<Warmable> _onExit = new();
        private CompositeDisposable _disposables = new();
        
        public IObservable<Warmable> OnObjectEnter => _onEnter;
        public IObservable<Warmable> OnObjectExit => _onExit;
        public IReadOnlyCollection<Warmable> ObjectsInRange => _objectsInRange;
        
        private void Awake()
        {
            _collider = GetComponent<CircleCollider2D>();
            if (_collider == null)
            {
                _collider = gameObject.AddComponent<CircleCollider2D>();
            }
            _collider.isTrigger = true;
            
            this.OnTriggerEnter2DAsObservable()
                .Subscribe(ProcessEnter)
                .AddTo(_disposables);
                
            this.OnTriggerExit2DAsObservable()
                .Subscribe(ProcessExit)
                .AddTo(_disposables);
        }
        
        private void ProcessEnter(Collider2D col)
        {
            var obj = col.GetComponent<Warmable>();
            if (obj != null && _objectsInRange.Add(obj))
            {
                _onEnter.OnNext(obj);
            }
        }
        
        private void ProcessExit(Collider2D col)
        {
            var obj = col.GetComponent<Warmable>();
            if (obj != null && _objectsInRange.Remove(obj))
            {
                _onExit.OnNext(obj);
            }
        }
        
        public void SetRadius(float radius)
        {
            if (_collider != null)
            {
                _collider.radius = radius;
            }
        }
        
        public void SetActive(bool active)
        {
            if (_collider != null)
            {
                _collider.enabled = active;
            }
            
            if (!active)
            {
                _objectsInRange.Clear();
            }
        }
        
        private void OnDestroy()
        {
            _disposables?.Dispose();
            _onEnter?.Dispose();
            _onExit?.Dispose();
        }
    }
    
    /// <summary>
    /// Триггер-зона для IInteractable объектов.
    /// </summary>
    public class InteractableTriggerZone : MonoBehaviour
    {
        private CircleCollider2D _collider;
        private HashSet<IInteractable> _objectsInRange = new();
        private Subject<IInteractable> _onEnter = new();
        private Subject<IInteractable> _onExit = new();
        private CompositeDisposable _disposables = new();
        
        public IObservable<IInteractable> OnObjectEnter => _onEnter;
        public IObservable<IInteractable> OnObjectExit => _onExit;
        public IReadOnlyCollection<IInteractable> ObjectsInRange => _objectsInRange;
        
        private void Awake()
        {
            _collider = GetComponent<CircleCollider2D>();
            if (_collider == null)
            {
                _collider = gameObject.AddComponent<CircleCollider2D>();
            }
            _collider.isTrigger = true;
            
            this.OnTriggerEnter2DAsObservable()
                .Subscribe(ProcessEnter)
                .AddTo(_disposables);
                
            this.OnTriggerExit2DAsObservable()
                .Subscribe(ProcessExit)
                .AddTo(_disposables);
        }
        
        private void ProcessEnter(Collider2D col)
        {
            var obj = col.GetComponent<IInteractable>();
            if (obj != null && _objectsInRange.Add(obj))
            {
                _onEnter.OnNext(obj);
            }
        }
        
        private void ProcessExit(Collider2D col)
        {
            var obj = col.GetComponent<IInteractable>();
            if (obj != null && _objectsInRange.Remove(obj))
            {
                _onExit.OnNext(obj);
            }
        }
        
        public void SetRadius(float radius)
        {
            if (_collider != null)
            {
                _collider.radius = radius;
            }
        }
        
        public void SetActive(bool active)
        {
            if (_collider != null)
            {
                _collider.enabled = active;
            }
            
            if (!active)
            {
                _objectsInRange.Clear();
            }
        }
        
        private void OnDestroy()
        {
            _disposables?.Dispose();
            _onEnter?.Dispose();
            _onExit?.Dispose();
        }
    }
}
