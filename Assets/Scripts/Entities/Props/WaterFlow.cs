using Entities.PlayerScripts;
using UnityEngine;
using Zenject;

namespace Entities.Props
{
    public class WaterFlow : MonoBehaviour
    {
        [SerializeField] private Vector2 _flowDirection = Vector2.right;
        [SerializeField] private float _flowForce = 10f;

        private Rigidbody2D _playerRb;

        [Inject]
        private void Construct(Player player)
        {
            _playerRb = player.Rigidbody;
        }

        private void FixedUpdate()
        {
            if (_playerRb == null) return;

            var col = GetComponent<Collider2D>();
            if (col == null) return;

            if (col.OverlapPoint(_playerRb.position))
                _playerRb.AddForce(_flowDirection.normalized * _flowForce * Time.fixedDeltaTime, ForceMode2D.Force);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Player") && _playerRb != null)
                _playerRb.AddForce(_flowDirection.normalized * _flowForce, ForceMode2D.Force);

            if (other.CompareTag("Pushable") && other.attachedRigidbody != null)
                other.attachedRigidbody.AddForce(_flowDirection.normalized * _flowForce, ForceMode2D.Force);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.35f);
            var col = GetComponent<Collider2D>();
            if (col != null) Gizmos.DrawCube(transform.position, col.bounds.size);
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, _flowDirection.normalized * 2f);
        }
#endif
    }
}
