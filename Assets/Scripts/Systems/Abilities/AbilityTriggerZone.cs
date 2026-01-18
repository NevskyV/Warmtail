using System;
using System.Collections.Generic;
using Interfaces;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Systems.Abilities
{
    public sealed class AbilityTriggerZone<T> where T : class
    {
        private CircleCollider2D Collider;
        private CompositeDisposable Disposables = new();
        private HashSet<T> _objectsInRange = new();
        private Subject<T> _onEnter = new();
        private Subject<T> _onExit = new();
        
        public IObservable<T> OnObjectEnter => _onEnter;
        public IObservable<T> OnObjectExit => _onExit;
        public IReadOnlyCollection<T> ObjectsInRange => _objectsInRange;
        
        private GameObject _triggerObject;
        private float _radius;
        
        public AbilityTriggerZone(GameObject triggerObj, float radius)
        {
            _triggerObject = triggerObj;
            _radius = radius;
        }

        public void Wake()
        {
            Collider = _triggerObject.GetComponent<CircleCollider2D>();
            if (Collider == null)
            {
                Collider = _triggerObject.AddComponent<CircleCollider2D>();
            }
            Collider.isTrigger = true;
            Collider.radius = _radius;
            _triggerObject.OnTriggerEnter2DAsObservable()
                .Subscribe(OnTriggerEnterInternal)
                .AddTo(Disposables);
                
            _triggerObject.OnTriggerExit2DAsObservable()
                .Subscribe(OnTriggerExitInternal)
                .AddTo(Disposables);
        }

        private void OnTriggerEnterInternal(Collider2D col)
        {
            var obj = col.GetComponent<T>();
            if (obj != null && _objectsInRange.Add(obj))
            {
                _onEnter.OnNext(obj);
            }
        }

        private void OnTriggerExitInternal(Collider2D col)
        {
            var obj = col.GetComponent<T>();
            if (obj != null && _objectsInRange.Remove(obj))
            {
                _onExit.OnNext(obj);
            }
        }
        
        public void SetActive(bool active)
        {
            if (Collider != null)
            {
                Collider.enabled = active;
            }
            if (!active)
            {
                _objectsInRange.Clear();
            }
        }

        private void OnDestroy()
        {
            Disposables?.Dispose();
            _onEnter?.Dispose();
            _onExit?.Dispose();
        }
        
    }
}
