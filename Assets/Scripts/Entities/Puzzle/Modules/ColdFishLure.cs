using Interfaces;
using Systems;
using UnityEngine;

namespace Entities.Puzzle.Modules
{
    public class ColdFishLure : Warmable
    {
        [SerializeField] private Transform _targetPosition;
        [SerializeField] private float _followSpeed = 3f;
        [SerializeField] private float _arrivalRadius = 1f;

        private ColdFishLureModule _module;
        private Transform _playerTransform;
        private bool _active;
        private bool _solved;

        public void Initialize(ColdFishLureModule module)
        {
            _module = module;
        }

        public override void WarmComplete()
        {
            _active = true;
        }

        private void Update()
        {
            if (!_active || _solved) return;

            if (_playerTransform == null)
            {
                var playerGo = GameObject.FindWithTag("Player");
                if (playerGo != null) _playerTransform = playerGo.transform;
                else return;
            }

            transform.position = Vector2.MoveTowards(transform.position,
                _playerTransform.position, _followSpeed * Time.deltaTime);

            if (_targetPosition != null)
            {
                float distToTarget = Vector2.Distance(transform.position, _targetPosition.position);
                if (distToTarget <= _arrivalRadius)
                    Complete();
            }
        }

        private void Complete()
        {
            _solved = true;
            _module?.Solve();
        }

        public override void Reset()
        {
            base.Reset();
            _active = false;
            _solved = false;
        }
    }
}
