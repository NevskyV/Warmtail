using System;
using Interfaces;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Systems.Tasks
{
    public class TriggerTask : ITask
    {
        public bool Completed { get; set; }
        public Action OnComplete { get; set; }
        [SerializeField] private Collider2D _prefab;
        [SerializeField] private Vector2 _position;

        public void Activate()
        {
            var observable = Object.Instantiate(_prefab, _position, Quaternion.identity);
            observable.OnTriggerEnter2DAsObservable().Subscribe(c =>
            {
                if (c.CompareTag("Player"))
                {
                    Completed = true;
                    OnComplete?.Invoke();
                    Object.Destroy(observable.gameObject);
                }
            });
        }
    }
}