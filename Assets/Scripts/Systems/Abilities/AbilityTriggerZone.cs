using System;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Systems.Abilities
{
    /// <summary>
    /// Универсальный компонент для управления триггер-зоной через UniRx.
    /// Отслеживает объекты типа T, входящие/выходящие из триггера.
    /// </summary>
    public class AbilityTriggerZone<T> : MonoBehaviour where T : class
    {
        private CircleCollider2D _collider;
        private HashSet<T> _objectsInRange = new();
        private Subject<T> _onEnter = new();
        private Subject<T> _onExit = new();
        private CompositeDisposable _disposables = new();
        
        public IObservable<T> OnObjectEnter => _onEnter;
        public IObservable<T> OnObjectExit => _onExit;
        public IReadOnlyCollection<T> ObjectsInRange => _objectsInRange;
        
        private void Awake()
        {
            _collider = GetComponent<CircleCollider2D>();
            if (_collider == null)
            {
                _collider = gameObject.AddComponent<CircleCollider2D>();
            }
            _collider.isTrigger = true;
            
            // Подписка на события триггера через UniRx
            this.OnTriggerEnter2DAsObservable()
                .Subscribe(ProcessEnter)
                .AddTo(_disposables);
                
            this.OnTriggerExit2DAsObservable()
                .Subscribe(ProcessExit)
                .AddTo(_disposables);
        }
        
        private void ProcessEnter(Collider2D col)
        {
            var obj = col.GetComponent<T>();
            if (obj != null && _objectsInRange.Add(obj))
            {
                _onEnter.OnNext(obj);
            }
        }
        
        private void ProcessExit(Collider2D col)
        {
            var obj = col.GetComponent<T>();
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
