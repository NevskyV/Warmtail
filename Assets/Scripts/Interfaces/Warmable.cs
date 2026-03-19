using Systems;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Interfaces
{
    public class Warmable : MonoBehaviour
    {
        [SerializeField] protected float _warmFactor = 0.1f;
        protected float _warmthAmount = 1;
        protected float _maxWarmthAmount = 1;
        [SerializeField] protected UnityEvent _warmEvent;
        protected ResettableTimer _timer;

        private WarmthSystem _warmthSystem;

        [Inject]
        private void Construct(WarmthSystem warmthSystem)
        {
            _warmthSystem = warmthSystem;
        }

        public virtual void Warm()
        {
            if (_warmthAmount <= 0) return;
            _warmthAmount -= _warmFactor;
            if (_warmthAmount <= 0)
                WarmComplete();
            else
            {
                _timer ??= new ResettableTimer(5, Reset);
                _timer.Start();
            }
        }

        public virtual void WarmComplete()
        {
            _warmthSystem?.AddCell();
            _warmEvent.Invoke();
        }

        public virtual void Reset()
        {
            _warmthAmount = _maxWarmthAmount;
        }

        public void SetWarmEvent(UnityAction action) => _warmEvent.AddListener(action);
    }
}
