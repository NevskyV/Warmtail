using System;
using Interfaces;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Systems.Tasks
{
    public class WarmTriggerTask: ITask
    {
        public bool Completed { get; set; }
        public Action OnComplete { get; set; }
        [SerializeField] private Collider2D _prefab;
        [SerializeField] private Vector2 _position;
        [TextArea] [SerializeField] private string _description;

        public void Activate()
        {
            var observable = Object.Instantiate(_prefab.GetComponent<Warmable>(), _position, Quaternion.identity);
            observable.SetWarmEvent(() =>
            {
                Completed = true;
                OnComplete?.Invoke();
                Object.Destroy(observable.gameObject);
            });
        }
    }
}