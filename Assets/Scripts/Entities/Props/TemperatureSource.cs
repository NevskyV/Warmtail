using Entities.PlayerScripts;
using Entities.UI;
using Systems;
using UnityEngine;
using Zenject;

namespace Entities.Props
{
    public class TemperatureSource : SavableStateObject
    {
        [SerializeField] private float _tempPerSecond = 15f;
        [SerializeField] private float _radius = 3f;
        [SerializeField] private float _forceMultiplier = 0f;

        private TemperatureSystem _temperatureSystem;
        private Rigidbody2D _playerRb;
        [Inject] private UIStateSystem _uiStateSystem;

        [Inject]
        private void Construct(TemperatureSystem temperatureSystem, Player player)
        {
            _temperatureSystem = temperatureSystem;
            _playerRb = player.Rigidbody;
        }

        private void FixedUpdate()
        {
            if (!_playerRb || _temperatureSystem == null || _uiStateSystem.CurrentState != UIState.Normal) return;

            var dist = Vector2.Distance(_playerRb.position, (Vector2)transform.position);
            if (dist > _radius) return;

            _temperatureSystem.Modify(_tempPerSecond * Time.fixedDeltaTime);

            if (_forceMultiplier == 0f) return;

            var dir = ((Vector2)_playerRb.position - (Vector2)transform.position).normalized;
            _playerRb.AddForce(dir * _forceMultiplier);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = _tempPerSecond >= 0 ? new Color(1f, 0.3f, 0f, 0.3f) : new Color(0.3f, 0.7f, 1f, 0.3f);
            Gizmos.DrawSphere(transform.position, _radius);
        }
#endif
    }
}
