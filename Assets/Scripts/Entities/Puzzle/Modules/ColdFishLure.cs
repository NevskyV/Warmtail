using Data;
using Data.Player;
using Systems;
using UnityEngine;
using Zenject;

namespace Entities.Puzzle.Modules
{
    public class ColdFishLure : MonoBehaviour
    {
        [SerializeField] private Transform _targetPosition;
        [SerializeField] private float _followSpeed = 3f;
        [SerializeField] private float _arrivalRadius = 1f;
        [SerializeField] private float _attractRange = 8f;

        private ColdFishLureModule _module;
        private Transform _playerTransform;
        private bool _solved;

        [Inject] private GlobalData _globalData;

        [Inject]
        private void Construct(Entities.PlayerScripts.Player player)
        {
            _playerTransform = player.Rigidbody.transform;
        }

        public void Initialize(ColdFishLureModule module)
        {
            _module = module;
        }

        private void Update()
        {
            if (_solved) return;

            if (_playerTransform == null)
            {
                var playerGo = GameObject.FindWithTag("Player");
                if (playerGo != null) _playerTransform = playerGo.transform;
                else return;
            }

            if (_globalData == null)
            {
                _globalData = FindObjectOfType<GlobalData>();
                if (_globalData == null) return;
            }

            var data = _globalData.Get<RuntimePlayerData>();
            bool playerIsHot = data.Temperature > TemperatureSystem.Neutral;

            float distToPlayer = Vector2.Distance(transform.position, _playerTransform.position);
            bool playerNearby = distToPlayer <= _attractRange;

            if (playerIsHot && playerNearby)
            {
                transform.position = Vector2.MoveTowards(transform.position,
                    _playerTransform.position, _followSpeed * Time.deltaTime);
            }

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
    }
}
